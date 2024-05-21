using AutoMapper;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Playlist
{
    public class PlaylistService : IPlaylistService
    {
        private readonly ChinookContext dbContext;
        private readonly IMapper mapper;

        public PlaylistService(ChinookContext _dbContext, IMapper _mapper) 
        { 
            dbContext = _dbContext;
            mapper = _mapper;
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
    }
}
