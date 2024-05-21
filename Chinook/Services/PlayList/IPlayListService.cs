namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<PlaylistDto> GetTracksOfUserPlaylist (long playlistId, string userId);

        Task<UserPlayListDto> AddTrackToMyFavoritePlaylist(long trackId, string userId);

        Task<UserPlayListDto> RemoveTrackFromMyFavoritePlaylist(long trackId, string userId);
    }
}
