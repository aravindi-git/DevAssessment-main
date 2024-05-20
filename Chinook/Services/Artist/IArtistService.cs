namespace Chinook.Services.Artist
{
    public interface IArtistService
    {
        Task<List<ArtistDto>> GetAllArtists(); 
    }
}
