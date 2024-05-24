namespace Chinook.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        public event  Action OnNavItemsChanged;
        private IPlaylistService playlistService {  get; set; }
        public NavigationService(IPlaylistService _playlistService) 
        {
            playlistService = _playlistService;
        }

        public async Task<List<NavigationItem>> GetNavigationItems(string UserId)
        {
            List<NavigationItem> navigations = [];
            var playLists = await playlistService.GetMyPlayLists(UserId);

            foreach (var playlist in playLists)
            {
                navigations.Add(new NavigationItem
                {
                    DisplayName = playlist.Name,
                    Url = $"Playlist/{playlist.PlaylistId}"
                });
            }

            return navigations; 
        }

        public void NotifyChange()
        {
            OnNavItemsChanged?.Invoke(); 
        }
    }
}
