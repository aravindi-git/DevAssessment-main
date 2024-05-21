namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<PlaylistDto> GetTracksOfUserPlaylist (long playlistId, string userId); 
    }
}
