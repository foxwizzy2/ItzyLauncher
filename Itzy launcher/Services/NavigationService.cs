using Microsoft.Extensions.DependencyInjection;
using ItzyLauncher.ViewModels;
using ItzyLauncher.ViewModels.Pages;

namespace ItzyLauncher.Services;

public sealed class NavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Func<PageViewModelBase>> _pages = new();

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        Register("home", () => _serviceProvider.GetRequiredService<HomePageViewModel>());
        Register("account", () => _serviceProvider.GetRequiredService<AccountPageViewModel>());
        Register("settings", () => _serviceProvider.GetRequiredService<SettingsPageViewModel>());
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

        return _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}