namespace Chinook.Services.Album
{
    public interface IAlbumService
    {
        Task<List<AlbumDto>> GetAlbumsOfArtist(long AristId);
        Task<List<PlaylistTrackDto>> GetTracksOfArtist(long aristId);
    }
}
