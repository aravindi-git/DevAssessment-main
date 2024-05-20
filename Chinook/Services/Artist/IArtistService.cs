namespace Chinook.Services.Artist
{
    public interface IArtistService
    {
        Task<List<ArtistDto>> GetAllArtists();
        Task<ArtistDto> GetArtistById (long id);
    }
}
