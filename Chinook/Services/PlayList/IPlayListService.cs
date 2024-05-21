namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<PlaylistDto> GetTracksOfUserPlaylist (long playlistId, string userId);

        Task<UserPlayListDto> AddTrackToMyFavoritePlayList(long trackId, string userId);

        Task<UserPlayListDto> RemoveTrackFromMyFavoritePlayList(long trackId, string userId);
    }
}
