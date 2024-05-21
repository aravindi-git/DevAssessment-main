namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<PlaylistDto> GetTracksOfUserPlaylist (long playlistId, string userId);

        Task<UserPlayListDto> AddTrackToPlaylist(long trackId, string userId);
        Task<UserPlayListDto> AddTrackToPlaylist(long playlistId, long trackId, string userId);

        Task<UserPlayListDto> RemoveTrackFromPlaylist(long trackId, string userId);

        Task<List<ExistingPlaylistDto>> GetExistingPlaylists(); 
    }
}
