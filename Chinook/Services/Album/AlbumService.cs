using AutoMapper;

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
    }
}
