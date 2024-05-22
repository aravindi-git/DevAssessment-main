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
        private readonly ILogger<ArtistService> logger;

        public ArtistService (ChinookContext _dbcontext, IMapper _mapper, ILogger<ArtistService> _logger)
        {
            dbContext = _dbcontext;
            mapper = _mapper;
            logger = _logger;
        }

        // Fetching all the Artists from the database. 
        public async Task<List<ArtistDto>> GetAllArtists()
        {
            List<ArtistDto> artists = [];
            try
            {
               artists = await dbContext.Artists
                        .Include(a => a.Albums)
                        .Select(a => mapper.Map<ArtistDto>(a))
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred: " + ex.Message); 
            }
            return artists;
        }

        // Fetching an Artist's by providing the Id. 
        public async Task<ArtistDto> GetArtistById(long id)
        {
            ArtistDto artist = new(); 
            try
            {
                artist = await dbContext.Artists
                        .Where(a => a.ArtistId == id)
                        .Select(a => mapper.Map<ArtistDto>(a))
                        .FirstAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred: " + ex.Message);
            }
            return artist;
        }
    }
}
