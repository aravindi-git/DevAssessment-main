using AutoMapper;
using AutoMapper.QueryableExtensions;
using Chinook.Models;
using Chinook.Pages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chinook.Services.Playlist
{
    public class PlaylistService : IPlaylistService
    {
        private readonly ChinookContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<PlaylistService> logger;

        public PlaylistService(ChinookContext _dbContext, IMapper _mapper, ILogger<PlaylistService> _logger) 
        { 
            dbContext = _dbContext;
            mapper = _mapper;
            logger = _logger; 
        }

        // Make a track as favorite track. 
        public async Task<PlayListResponseDto> AddTrackToFavoritePlaylist(long trackId, string userId)
        {
            long favoritePlaylistId = 0;
            bool isNewPlaylistCreated = false; 
            try
            {
                //First we need to check the availability of the Favorites playlist. 
                var favoritePlayList = await dbContext.Playlists
                                      .Include(p => p.Tracks)
                                      .Include(p => p.UserPlaylists)
                                      .FirstOrDefaultAsync(p => p.Name!.Equals(ChinookConstants.MyFavoriteTracks));

                // Here we are getting the selected track from the database. 
                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);

                //Here, Attach is used to bring the favoritePlayList and track into the context, and then their states are explicitly set to Modified to do the changes.
                if (track != null )
                {
                    dbContext.Tracks.Attach(track);
                }

                if (favoritePlayList != null)
                {
                    dbContext.Playlists.Attach(favoritePlayList);
                    favoritePlaylistId = favoritePlayList.PlaylistId;
                    isNewPlaylistCreated = false;

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
                    long playlistId = await CreatePlaylist(ChinookConstants.MyFavoriteTracks);
                    favoritePlayList = await dbContext.Playlists.FirstAsync(p => p.PlaylistId == playlistId);
                    favoritePlaylistId = playlistId;
                    isNewPlaylistCreated = true; 

                    List<Track> tracks = [];
                    favoritePlayList.Tracks = tracks;
                    favoritePlayList.Tracks!.Add(track);

                    dbContext.UserPlaylists.Add(new UserPlaylist() { UserId = userId, PlaylistId = favoritePlayList.PlaylistId });
                    await dbContext.SaveChangesAsync();
                }

                return new PlayListResponseDto() { PlaylistId= favoritePlaylistId,  SuccessfullyAdded = true, IsNewPlaylist = isNewPlaylistCreated };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
                return new PlayListResponseDto() { SuccessfullyAdded = false};
            }
           
        }

        // Add a track to a given playlist
        public async Task<PlayListResponseDto> AddTrackToPlaylist(long playlistId, long trackId, string userId)
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
                    return new PlayListResponseDto() { PlaylistId = playlistId , SuccessfullyAdded = true };
                }
                else
                {
                    throw new Exception("The selected playlist or track not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
                return new PlayListResponseDto() { SuccessfullyAdded = false };
            }
        }

        //Create a new playlist
        public async Task<PlayListResponseDto> CreatePlaylist(string playlistName, long trackId, string userId)
        {
            try
            {
                var existinglayList = await dbContext.Playlists
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(p => p.Name!.Equals(playlistName));

                // if a playlist with the same name is already available, we are not creating a new playlist to avoid duplicates. 
                if (existinglayList != null)
                {
                    return new PlayListResponseDto() {  PlaylistId = existinglayList.PlaylistId , Message = "This playlist is already exists", SuccessfullyAdded = false };
                }
                else
                {
                    long newPlaylistId = await CreatePlaylist(playlistName);

                    if (newPlaylistId > 0)
                    {
                        var result = await AddTrackToPlaylist(newPlaylistId, trackId, userId); 

                        if(result.SuccessfullyAdded == true)
                        {
                            return new PlayListResponseDto() { PlaylistId = newPlaylistId, Message = "Playlist is created", SuccessfullyAdded = true };
                        }  
                    }
                    return new PlayListResponseDto() { Message = "Playlist is not created successfully", SuccessfullyAdded = false };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
                return new PlayListResponseDto() { Message = ex.Message, SuccessfullyAdded = false };
            }
        }

        // Fetch the existing playlists
        public async Task<List<ExistingPlaylistDto>> GetExistingPlaylists()
        {
            List<ExistingPlaylistDto> existingPlaylists = []; 
            try
            {
               existingPlaylists = await dbContext.Playlists
                                  .Select(p => mapper.Map<ExistingPlaylistDto>(p))
                                  .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
            }
            return existingPlaylists;
        }

        // Fetch the list of the User Created playlists
        public async Task<List<MyPlaylistDto>> GetMyPlayLists(string userId)
        {
            List<MyPlaylistDto> myPlaylists = []; 
            try
            {
                 myPlaylists = await dbContext.Playlists
                               .Include(p => p.UserPlaylists)
                               .Where(p => p.UserPlaylists.Any(upl => upl.UserId.Equals(userId)))
                               .Select(p => mapper.Map<MyPlaylistDto>(p))
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
            }
            return myPlaylists;
        }

        // Get Tracks of the User Created playlists
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
                    playlistDto.Tracks.ForEach(t => t.IsFavorite = true); 
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
            }
            return playlistDto;
        }

        // Remove a track from the favorite playlist
        public async Task<PlayListResponseDto> RemoveTrackFromFavoritePlaylist(long trackId)
        {
            try
            {
                var favoritePlayList = await dbContext.Playlists
                                      .Include(p => p.Tracks)
                                      .Include(p => p.UserPlaylists)
                                      .FirstOrDefaultAsync(p => p.Name!.Equals(ChinookConstants.MyFavoriteTracks));

                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);
              
                if (track != null && favoritePlayList != null)
                {
                    dbContext.Playlists.Attach(favoritePlayList);
                    dbContext.Tracks.Attach(track);

                    favoritePlayList.Tracks!.Remove(track);
                    await dbContext.SaveChangesAsync();
                    return new PlayListResponseDto() { PlaylistId = favoritePlayList.PlaylistId,  SuccessfullyAdded = true };
                }
                else
                {
                    return new PlayListResponseDto() { SuccessfullyAdded = false };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
                return new PlayListResponseDto() { SuccessfullyAdded = false };
            }
        }

        // Remove a track from a playlist
        public  async Task<PlayListResponseDto> RemoveTrackFromPlaylist(long playlistId, long trackId)
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
                    return new PlayListResponseDto() { PlaylistId = playList.PlaylistId, SuccessfullyAdded = true };
                }
                else
                {
                    return new PlayListResponseDto() { SuccessfullyAdded = false };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
                return new PlayListResponseDto() { SuccessfullyAdded = false };
            }
        }

        // Create a new playlist by providing a name. 
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
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
                return 0; 
            }

        }
    }
}
