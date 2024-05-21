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

            CreateMap<Track, PlaylistTrack>()
                    .ForMember(dest =>
                        dest.TrackId,
                        opt => opt.MapFrom(src => src.TrackId))
                    .ForMember(dest =>
                        dest.TrackName,
                        opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest =>
                        dest.AlbumTitle,
                        opt => opt.MapFrom(src => (src.Album == null) ? String.Empty : src.Album.Title))
                    .ForMember(dest =>
                        dest.ArtistName,
                        opt => opt.MapFrom(src => (src.Album == null) ? String.Empty : src.Album.Artist.Name))
                    .ForMember(dest =>
                        dest.IsFavorite, opt => opt.Ignore()); 

            CreateMap<Playlist, PlaylistDto>()
                    .ForMember(dest =>
                        dest.Name,
                        opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest =>
                        dest.Tracks,
                        opt => opt.MapFrom(src => src.Tracks));

            CreateMap<Playlist, ExistingPlaylistDto>();

            CreateMap<Playlist, MyPlaylistDto>();

            
        }
    }
}
