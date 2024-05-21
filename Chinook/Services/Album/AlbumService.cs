using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Album
{
    public class AlbumService : IAlbumService
    {
        private readonly ChinookContext dbContext;
        private readonly IMapper mapper;

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
        public async Task<List<PlaylistTrack>> GetTracksOfArtist(long aristId)
        {
            try
            {
                List<PlaylistTrack> tracks = [];

                tracks = await dbContext.Tracks
                        .Include(t => t.Album)
                        .Where(t => t.Album!.ArtistId == aristId)
                        .Select(t => mapper.Map<PlaylistTrack>(t)).ToListAsync();


                return tracks;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong " + ex.Message);
            }
        }
    }
}
