using ItzyLauncher.ViewModels;
using ItzyLauncher.ViewModels.Pages;

namespace ItzyLauncher.Services;

public sealed class NavigationService
{
    private readonly Dictionary<string, Func<PageViewModelBase>> _pages = new();

    public NavigationService()
    {
        Register("home", () => new HomePageViewModel());
        Register("account", () => new AccountPageViewModel());
    }

    public void Register(string pageId, Func<PageViewModelBase> factory)
    {
        _pages[pageId.ToLower()] = factory;
    }

    public PageViewModelBase Navigate(string pageId)
    {
        string key = pageId.ToLower();

        if (_pages.TryGetValue(key, out Func<PageViewModelBase>? factory))
        {
            return factory();
        }

        return new HomePageViewModel();
    }
}