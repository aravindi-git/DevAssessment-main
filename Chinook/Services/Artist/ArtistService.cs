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
            try
            {
                List<ArtistDto> artists = await dbContext.Artists
                                          .Include(a => a.Albums)
                                          .Select(a => mapper.Map<ArtistDto>(a)).ToListAsync();

                return artists;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong " + ex.Message);
            } 
        }

        public async Task<ArtistDto> GetArtistById(long id)
        {
            try
            {
                ArtistDto artist = await dbContext.Artists
                                  .Where(a => a.ArtistId == id)
                                  .Select(a => mapper.Map<ArtistDto>(a)).FirstAsync();

                return artist;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong " + ex.Message);
            }
        }
    }
}
