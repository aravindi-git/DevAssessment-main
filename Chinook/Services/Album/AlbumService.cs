using AutoMapper;
using Chinook.Models;
using Chinook.Pages;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Album
{
    public class AlbumService : IAlbumService
    {
        private readonly ChinookContext dbContext;
        private readonly IMapper mapper;
        private readonly string favoritePlaylistName = "My favorite tracks";

        public AlbumService(ChinookContext _dbcontext, IMapper _mapper)
        {
            dbContext = _dbcontext;
            mapper = _mapper;
        }

        public async Task<List<AlbumDto>> GetAlbumsOfArtist(long aristId)
        {
            try
            {
                List<AlbumDto> albums = [];

                albums = await dbContext.Albums
                        .Include(t => t.Tracks)
                        .Where(a => a.ArtistId == aristId).Select(a => mapper.Map<AlbumDto>(a)).ToListAsync();

                return albums;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong " + ex.Message);
            }
        }
        public async Task<List<PlaylistTrackDto>> GetTracksOfArtist(long aristId, string userId)
        {
            try
            {
                var tracks = await dbContext.Tracks
                            .Include(t => t.Album)
                            .Include(t => t.Playlists)
                            .Where(t => t.Album!.ArtistId == aristId)
                            .Select(t => new
                            {
                                Track = t,
                                IsFavorite = t.Playlists.Any(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name.Equals(favoritePlaylistName)))
                            }).ToListAsync();

                var playlistTrackDtos = tracks.Select(t =>
                {
                    var dto = mapper.Map<PlaylistTrackDto>(t.Track);
                    dto.IsFavorite = t.IsFavorite;
                    return dto;
                }).ToList();

                return playlistTrackDtos; 
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong " + ex.Message);
            }
        }
    }
}
