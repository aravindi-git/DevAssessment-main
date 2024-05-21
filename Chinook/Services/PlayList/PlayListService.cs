﻿using AutoMapper;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

        public async Task<UserPlayListDto> AddTrackToMyFavoritePlaylist(long trackId, string userId)
        {
            try
            {
               
                //First we need to check the availability of the Favorites play list. 
                var favoritePlayList = await dbContext.Playlists
                                      .Include(p => p.Tracks)
                                      .Include(p => p.UserPlaylists)
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(p => p.Name!.Equals(favoritePlaylistName)); 

                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);

                if (favoritePlayList != null && favoritePlayList.PlaylistId > 0)
                {
                    if (favoritePlayList.Tracks?.Count > 0)
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

        public async Task<PlaylistDto> GetTracksOfUserPlaylist(long playlistId, string userId)
        {
            PlaylistDto playlist = new(); 

            try
            {
                //playlist = await dbContext.Playlists
                //           .Include(p => p.Tracks)
                //           .ThenInclude(t => t.Album).ThenInclude(a => a.Artist)
                //           .Where(p => p.PlaylistId == playlistId)
                //           .Select(p => mapper.Map<PlaylistDto>(p))
                //           .FirstOrDefaultAsync();

                playlist = await dbContext.Playlists
                           .Include(p => p.Tracks)
                           .ThenInclude(t => t.Album).ThenInclude(a => a.Artist)
                           .Where(p => p.PlaylistId == playlistId)
                           .Select(p => new PlaylistDto()
                           {
                               Name = p.Name,
                               Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                               {
                                   AlbumTitle = t.Album.Title,
                                   ArtistName = t.Album.Artist.Name,
                                   TrackId = t.TrackId,
                                   TrackName = t.Name,
                                   IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "Favorites")).Any()
                               }).ToList()
                           }).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred " + ex.Message);
            }
            return playlist; 
        }

        public async Task<UserPlayListDto> RemoveTrackFromMyFavoritePlaylist(long trackId, string userId)
        {
            try
            {

                var track = await dbContext.Tracks.FirstAsync(t => t.TrackId == trackId);

                var favoritePlayList = await dbContext.Playlists
                                          .Include(p => p.Tracks)
                                          .Include(p => p.UserPlaylists)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.Name!.Equals(favoritePlaylistName));

                if (track != null && favoritePlayList != null)
                {
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

        private async  Task<long> CreatePlaylist(string playlistName)
        {
            try
            {
                long maxId = dbContext.Playlists.Max(p => p.PlaylistId);
                var newPlaylist = new Models.Playlist() { PlaylistId= maxId + 1,  Name = playlistName };

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