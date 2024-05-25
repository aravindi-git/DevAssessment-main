namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<PlaylistDto> GetTracksOfUserPlaylist (long playlistId, string userId);

        Task<PlayListResponseDto> AddTrackToFavoritePlaylist(long trackId, string userId);

        Task<PlayListResponseDto> AddTrackToPlaylist (long playlistId, long trackId, string userId);

        Task<PlayListResponseDto> RemoveTrackFromPlaylist (long playlistId, long trackId);
        Task<PlayListResponseDto> RemoveTrackFromFavoritePlaylist(long trackId);

        Task<List<ExistingPlaylistDto>> GetExistingPlaylists ();

        List<MyPlaylistDto> GetMyPlayLists (string userId);

        Task<PlayListResponseDto> CreatePlaylist(string playlistName, long trackId, string userId);
    }
}
