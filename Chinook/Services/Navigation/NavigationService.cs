namespace Chinook.Services.Navigation
{
    public class NavigationService : INavigationService
    {
      
        private IPlaylistService playlistService {  get; set; }
        public NavigationService(IPlaylistService _playlistService) 
        {
            playlistService = _playlistService;
        }

        public async Task<List<NavigationItem>> GetNavigationItems(string UserId)
        {
            List<NavigationItem> navigations = [];
            var playLists = await playlistService.GetMyPlayLists(UserId);

            if(playLists != null && playLists.Count > 0 )
            {
                foreach (var playlist in playLists)
                {
                    navigations.Add(new NavigationItem
                    {
                        DisplayName = playlist.Name,
                        Url = $"Playlist/{playlist.PlaylistId}"
                    });
                }
            }

            return navigations; 
        }

       
    }
}
