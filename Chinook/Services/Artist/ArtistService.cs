using AutoMapper;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Chinook.Services.Artist
{
    public class ArtistService : IArtistService
    {
        private readonly ChinookContext _dbContext;
        private readonly IMapper _mapper; 

        public ArtistService (ChinookContext dbcontext, IMapper mapper) {
            _dbContext = dbcontext;
            _mapper = mapper;
        }

        public async Task<List<ArtistDto>> GetAllArtists()
        {
          List<ArtistDto> artists = await _dbContext.Artists
                                   .Include(a => a.Albums)
                                   .Select(a => _mapper.Map<ArtistDto>(a)).ToListAsync();
          return artists; 
        }
    }
}
