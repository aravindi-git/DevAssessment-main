using AutoMapper;
using AutoMapper.QueryableExtensions;
using Chinook.Models;
using Chinook.Pages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Chinook.Services.Playlist
{
    public class PlaylistService : IPlaylistService
    {
        private readonly ChinookContext dbContext;
        private readonly IMapper mapper;
        private readonly string favoritePlaylistName = "My favorite tracks";

        public PlaylistService(ChinookContext _dbContext, IMapper _mapper) 
        { 
            dbContext = _dbContext;
            mapper = _mapper;
        }

        public async Task<UserPlayListDto> AddTrackToFavoritePlaylist(long trackId, string userId)
        {
            try
            {
                //First we need to check the availability of the Favorites play list. 
                var favoritePlayList = await dbContext.Playlists
                                      .Include(p => p.Tracks)
                                      .Include(p => p.UserPlaylists)
                                      .FirstOrDefaultAsync(p => p.Name!.Equals(favoritePlaylistName));

                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);

                if(track != null )
                {
                    dbContext.Tracks.Attach(track);
                }

                if (favoritePlayList != null)
                {
                    dbContext.Playlists.Attach(favoritePlayList);

                    if (favoritePlayList.Tracks != null)
                    {
                        favoritePlayList.Tracks!.Add(track);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        List<Track> tracks = [];
                        favoritePlayList.Tracks = tracks;
                        favoritePlayList.Tracks!.Add(track);
                        await dbContext.SaveChangesAsync();
                    }
                   

                    var userPlaylist = await dbContext.UserPlaylists.FirstOrDefaultAsync(up => up.PlaylistId == favoritePlayList.PlaylistId && up.UserId.Equals(userId));

                    if(userPlaylist == null )
                    {
                        dbContext.UserPlaylists.Add(new UserPlaylist() { UserId = userId, PlaylistId = favoritePlayList.PlaylistId });
                        await dbContext.SaveChangesAsync();
                    }
                   
                }
                else
                {
                    long playlistId = await CreatePlaylist(favoritePlaylistName);
                    favoritePlayList = await dbContext.Playlists.FirstAsync(p => p.PlaylistId == playlistId);

                    List<Track> tracks = [];
                    favoritePlayList.Tracks = tracks;
                    favoritePlayList.Tracks!.Add(track);

                    dbContext.UserPlaylists.Add(new UserPlaylist() { UserId = userId, PlaylistId = favoritePlayList.PlaylistId });
                    await dbContext.SaveChangesAsync();
                }

                return new UserPlayListDto() { SuccessfullyAdded = true };

            }
            catch (Exception ex)
            {
                return new UserPlayListDto() { SuccessfullyAdded = false};
            }
           
        }

        public async Task<UserPlayListDto> AddTrackToPlaylist(long playlistId, long trackId, string userId)
        {
            try
            {
                var playList = await dbContext.Playlists
                             .Include(p => p.Tracks)
                             .Include(p => p.UserPlaylists)
                             .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);
              
                if (playList != null && track != null)
                {
                    dbContext.Playlists.Attach(playList);
                    dbContext.Tracks.Attach(track);

                    if (playList.Tracks != null)
                    {
                        playList.Tracks!.Add(track);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        List<Track> tracks = [];
                        playList.Tracks = tracks;
                        playList.Tracks!.Add(track);
                        await dbContext.SaveChangesAsync();
                    }

                    var userPlaylist = await dbContext.UserPlaylists.FirstOrDefaultAsync(up => up.PlaylistId == playList.PlaylistId && up.UserId.Equals(userId));

                    if (userPlaylist == null)
                    {
                        dbContext.UserPlaylists.Add(new UserPlaylist() { UserId = userId, PlaylistId = playList.PlaylistId });
                        await dbContext.SaveChangesAsync();
                    }
                    return new UserPlayListDto() { SuccessfullyAdded = true };
                }
                else
                {
                    throw new Exception("The selected playlist or track not found");
                }
            }
            catch (Exception ex)
            {
                return new UserPlayListDto() { SuccessfullyAdded = false };
            }
        }

        public async Task<UserPlayListDto> CreatePlaylist(string playlistName, long trackId, string userId)
        {
            try
            {
                var existinglayList = await dbContext.Playlists
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(p => p.Name!.Equals(playlistName));

                if (existinglayList != null)
                {
                    return new UserPlayListDto() { Message = "This playlist is already exists", SuccessfullyAdded = false };
                }
                else
                {
                    long newPlaylistId = await CreatePlaylist(playlistName);

                    if (newPlaylistId > 0)
                    {
                        var result = await AddTrackToPlaylist(newPlaylistId, trackId, userId); 

                        if(result.SuccessfullyAdded == true)
                        {
                            return new UserPlayListDto() { Message = "Playlist is created", SuccessfullyAdded = true };
                        }  
                    }
                    return new UserPlayListDto() { Message = "Playlist is not created successfully", SuccessfullyAdded = false };
                }
            }
            catch (Exception ex)
            {

                return new UserPlayListDto() { Message = ex.Message, SuccessfullyAdded = false };
            }
        }

        public async Task<List<ExistingPlaylistDto>> GetExistingPlaylists()
        {
            try
            {
                List<ExistingPlaylistDto> existingPlaylists = await dbContext.Playlists
                                                             .Select(p => mapper.Map<ExistingPlaylistDto>(p))
                                                             .ToListAsync();
                return existingPlaylists;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred " + ex.Message);
            }

        }

        public async Task<List<MyPlaylistDto>> GetMyPlayLists(string userId)
        {
            try
            {
                var myPlaylists = await dbContext.Playlists
                                    .Include(p => p.UserPlaylists)
                                    .Where(p => p.UserPlaylists.Any(upl => upl.UserId.Equals(userId)))
                                    .Select(p => mapper.Map<MyPlaylistDto>(p))
                                    .ToListAsync();

                return myPlaylists;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred " + ex.Message);
            }
        }

        public async Task<PlaylistDto> GetTracksOfUserPlaylist(long playlistId, string userId)
        {
            PlaylistDto playlistDto = new(); 

            try
            {
                var playlist = await dbContext.Playlists
                               .Include(p => p.Tracks)
                               .ThenInclude(t => t.Album).ThenInclude(a => a.Artist)
                               .Where(p => p.PlaylistId == playlistId)
                               .FirstOrDefaultAsync();

                if (playlist !=  null)
                {

                    playlistDto = mapper.Map<PlaylistDto>(playlist);

                    if (playlist.Name.Equals(favoritePlaylistName))
                    {
                        playlistDto.Tracks = playlist.Tracks.Select(t => new PlaylistTrackDto
                        {
                            AlbumTitle = t.Album.Title,
                            ArtistName = t.Album.Artist.Name,
                            TrackId = t.TrackId,
                            TrackName = t.Name,
                            IsFavorite = t.Playlists.Any(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name.Equals(favoritePlaylistName)))
                        }).ToList();
                    }
                    else
                    {
                        playlistDto.Tracks = playlist.Tracks.Select(t => new PlaylistTrackDto
                        {
                            AlbumTitle = t.Album.Title,
                            ArtistName = t.Album.Artist.Name,
                            TrackId = t.TrackId,
                            TrackName = t.Name,
                            IsFavorite = t.Playlists.Any(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.PlaylistId == playlistId))
                        }).ToList();
                    } 
                }

                return playlistDto;

            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred " + ex.Message);
            }
            
        }

        public async Task<UserPlayListDto> RemoveTrackFromFavoritePlaylist(long trackId, string userId)
        {
            try
            {
                var favoritePlayList = await dbContext.Playlists
                                          .Include(p => p.Tracks)
                                          .Include(p => p.UserPlaylists)
                                          .FirstOrDefaultAsync(p => p.Name!.Equals(favoritePlaylistName));

                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);
              
                if (track != null && favoritePlayList != null)
                {
                    dbContext.Playlists.Attach(favoritePlayList);
                    dbContext.Tracks.Attach(track);

                    favoritePlayList.Tracks!.Remove(track);
                    await dbContext.SaveChangesAsync();
                    return new UserPlayListDto() { SuccessfullyAdded = true };
                }
                else
                {
                    //throw new Exception("The given track or relevant playlist not found.");
                    return new UserPlayListDto() { SuccessfullyAdded = false };
                }
            }
            catch (Exception ex)
            {
                return new UserPlayListDto() { SuccessfullyAdded = false };
            }
        }

        public  async Task<UserPlayListDto> RemoveTrackFromPlaylist(long playlistId, long trackId, string userId)
        {
            try
            {
                var playList = await dbContext.Playlists
                                          .Include(p => p.Tracks)
                                          .Include(p => p.UserPlaylists)
                                          .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);
                
                if (track != null && playList != null)
                {
                    dbContext.Playlists.Attach(playList);
                    dbContext.Tracks.Attach(track);

                    playList.Tracks!.Remove(track);
                    await dbContext.SaveChangesAsync();
                    return new UserPlayListDto() { SuccessfullyAdded = true };
                }
                else
                {
                    //throw new Exception("The given track or relevant playlist not found.");
                    return new UserPlayListDto() { SuccessfullyAdded = false };
                }
            }
            catch (Exception ex)
            {
                return new UserPlayListDto() { SuccessfullyAdded = false };
            }
        }

        private async  Task<long> CreatePlaylist(string playlistName)
        {
            try
            {
               
                var newPlaylist = new Models.Playlist() { Name = playlistName };

                dbContext.Playlists.Add(newPlaylist);
                await dbContext.SaveChangesAsync();

                return newPlaylist.PlaylistId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred " + ex.Message);
            }

        }
    }
}
