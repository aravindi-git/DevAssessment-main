namespace Chinook.Services.Navigation
{
    public interface INavigationService
    {
        Task<List<NavigationItem>> GetNavigationItems(string UserId);
    }
}
