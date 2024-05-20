using AutoMapper;
using Chinook.Models;

namespace Chinook
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Artist, ArtistDto>();
            CreateMap<Album, AlbumDto>();
        }
    }
}
