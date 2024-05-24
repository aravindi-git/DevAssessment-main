using AutoMapper;
using Chinook.Models;
using Chinook.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Chinook.Services.Album
{
    public class AlbumService : IAlbumService
    {
        private readonly ChinookContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<AlbumService> logger;

        public AlbumService(ChinookContext _dbcontext, IMapper _mapper, ILogger<AlbumService> _logger)
        {
            dbContext = _dbcontext;
            mapper = _mapper;
            logger = _logger;
        }

        // Fetching the albums of a given Atrist. 
        public async Task<List<AlbumDto>> GetAlbumsOfArtist(long aristId)
        {
            List<AlbumDto> albums = [];
            try
            {
                albums = await dbContext.Albums
                        .Include(t => t.Tracks)
                        .Where(a => a.ArtistId == aristId)
                        .Select(a => mapper.Map<AlbumDto>(a))
                        .ToListAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
            }
            return albums;
        }

        // Fetching the tracks of a given Atrist. 
        public async Task<List<PlaylistTrackDto>> GetTracksOfArtist(long aristId, string userId)
        {
            List<PlaylistTrackDto> playlistTrackDtos = []; 
            try
            {
                var tracks = await dbContext.Tracks
                            .Include(t => t.Album)
                            .Include(t => t.Playlists)
                            .Where(t => t.Album!.ArtistId == aristId)
                            .Select(t => new
                            {
                                Track = t,
                                IsFavorite = t.Playlists.Any(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name.Equals(ChinookConstants.MyFavoriteTracks)))
                            }).ToListAsync();

                playlistTrackDtos = tracks.Select(t =>
                {
                    var dto = mapper.Map<PlaylistTrackDto>(t.Track);
                    dto.IsFavorite = t.IsFavorite;
                    return dto;
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ChinookConstants.ExceptionMessage + ex.Message);
            }
            return playlistTrackDtos;
        }
    }
}
