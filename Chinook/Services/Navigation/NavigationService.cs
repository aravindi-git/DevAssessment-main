using Chinook.Models;

namespace Chinook.Services.Navigation
{
    public class NavigationService
    {
        // List of navigation items
        private List<NavigationItem> navigationItems = new();
        private IPlaylistService playlistService {  get; set; }

        // Event handler to invoke when the nav menu items list is changed. 
        public event EventHandler? NavMenuItemsChanged;
        public NavigationService(IPlaylistService _playlistService) 
        {
            playlistService = _playlistService;
        }

        // Fetch the playlists from the database by sending the userId as a parameter, then add playlist items as the navigation items to the list. 
        // The displayname of the item is playlist name, and Url is consisting of the playlist Id. 
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

        // We cancall this function, when adding a new item to the navigation list. And notify the Nav menu component that the navigation items are changed. 
        public void AddNavMenuItem(long playlistId, string displayName)
        {
            var navItem = new NavigationItem(displayName, $"Playlist/{playlistId}");

            if (!navigationItems.Contains(navItem))
            {
                navigationItems.Add(navItem);
            }
            NavMenuItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    // NavigationItem has display name and the url
    public record NavigationItem(string DisplayName, string Url);

}
