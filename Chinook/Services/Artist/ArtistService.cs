using AutoMapper;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Chinook.Services.Artist
{
    public class ArtistService : IArtistService
    {
        private readonly ChinookContext dbContext;
        private readonly IMapper mapper; 

        public ArtistService (ChinookContext _dbcontext, IMapper _mapper) {
            dbContext = _dbcontext;
            mapper = _mapper;
        }

        public async Task<List<ArtistDto>> GetAllArtists()
        {
          List<ArtistDto> artists = await dbContext.Artists
                                   .Include(a => a.Albums)
                                   .Select(a => mapper.Map<ArtistDto>(a)).ToListAsync();

          return artists; 
        }

        public async Task<ArtistDto> GetArtistById(long id)
        {
            ArtistDto  artist = await dbContext.Artists
                .Where(a => a.ArtistId == id).
                Select(a => mapper.Map<ArtistDto>(a)).FirstAsync(); 

            return artist;
        }
    }
}
