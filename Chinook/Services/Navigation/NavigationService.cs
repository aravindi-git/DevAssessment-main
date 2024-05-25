using Chinook.Models;

namespace Chinook.Services.Navigation
{
    public class NavigationService
    {
        private List<NavigationItem> navigationItems = new();
        private IPlaylistService playlistService {  get; set; }

        public event EventHandler? NavMenuItemsChanged;
        public NavigationService(IPlaylistService _playlistService) 
        {
            playlistService = _playlistService;
        }

        public List<NavigationItem> GetNavigationItems(string UserId)
        {
            var playLists = playlistService.GetMyPlayLists(UserId);

            if (playLists != null && playLists.Count > 0)
            {
                foreach (var playlist in playLists)
                {
                    navigationItems.Add(new NavigationItem(playlist.Name, $"Playlist/{playlist.PlaylistId}"));
                }
            }
            return navigationItems;
        }

        public void AddNavMenuItem(long playlistId, string displayName)
        {
            navigationItems.Add(new NavigationItem (displayName, $"Playlist/{playlistId}"));
            NavMenuItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public record NavigationItem(string DisplayName, string Url);

}
