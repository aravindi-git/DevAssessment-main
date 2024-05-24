namespace Chinook.Services.Navigation
{
    public interface INavigationService
    {
        public event Action OnNavItemsChanged;
        Task<List<NavigationItem>> GetNavigationItems(string UserId);
        void NotifyChange();
    }
}
