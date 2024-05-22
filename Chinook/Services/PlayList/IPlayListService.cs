namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<PlaylistDto> GetTracksOfUserPlaylist (long playlistId, string userId);

        Task<UserPlayListDto> AddTrackToFavoritePlaylist(long trackId, string userId);

        Task<UserPlayListDto> AddTrackToPlaylist (long playlistId, long trackId, string userId);

        Task<UserPlayListDto> RemoveTrackFromPlaylist (long playlistId, long trackId);
        Task<UserPlayListDto> RemoveTrackFromFavoritePlaylist(long trackId);

        Task<List<ExistingPlaylistDto>> GetExistingPlaylists ();

        Task<List<MyPlaylistDto>> GetMyPlayLists (string userId);

        Task<UserPlayListDto> CreatePlaylist(string playlistName, long trackId, string userId);
    }
}
