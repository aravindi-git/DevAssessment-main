namespace Chinook.Services.Album
{
    public interface IAlbumService
    {
        Task<List<AlbumDto>> GetAlbumsOfArtist(long AristId);
        Task<List<PlaylistTrack>> GetTracksOfArtist(long aristId);
    }
}
