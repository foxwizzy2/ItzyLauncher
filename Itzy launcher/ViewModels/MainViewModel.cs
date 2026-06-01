using System.Collections.ObjectModel;
using System.Diagnostics;
using ItzyLauncher.Helpers;
using ItzyLauncher.Models;
using ItzyLauncher.Services;

namespace ItzyLauncher.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly ConfigService _configService;
    private readonly NavigationService _navigationService;
    private readonly ThemeService _themeService;

    private string _launcherTitle = "Loading...";
    private string _clientVersion = "";
    private string _accentColor = "#8B5CF6";
    private string _backgroundImage = "";
    private string _logo = "";
    private bool _maintenanceMode;
    private PageViewModelBase? _currentPage;

    public string LauncherTitle
    {
        get => _launcherTitle;
        set => SetProperty(ref _launcherTitle, value);
    }

    public string ClientVersion
    {
        get => _clientVersion;
        set => SetProperty(ref _clientVersion, value);
    }

    public string AccentColor
    {
        get => _accentColor;
        set => SetProperty(ref _accentColor, value);
    }

    public string BackgroundImage
    {
        get => _backgroundImage;
        set => SetProperty(ref _backgroundImage, value);
    }

    public string Logo
    {
        get => _logo;
        set => SetProperty(ref _logo, value);
    }

    public bool MaintenanceMode
    {
        get => _maintenanceMode;
        set => SetProperty(ref _maintenanceMode, value);
    }

    public PageViewModelBase? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public ObservableCollection<MenuItemModel> MenuItems { get; } = new();

    public MainViewModel(
        ConfigService configService,
        NavigationService navigationService,
        ThemeService themeService)
    {
        _configService = configService;
        _navigationService = navigationService;
        _themeService = themeService;

        CurrentPage = _navigationService.Navigate("home");

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        LauncherConfig config = await _configService.LoadAsync();

        LauncherTitle = config.LauncherName;
        ClientVersion = config.ClientVersion;
        MaintenanceMode = config.MaintenanceMode;

        _themeService.ApplyTheme(config.Theme);

        AccentColor = config.Theme.AccentColor;
        BackgroundImage = config.Theme.BackgroundImage;
        Logo = config.Theme.Logo;

        BuildMenu(config);
    }

    private void BuildMenu(LauncherConfig config)
    {
        MenuItems.Clear();

        foreach (LauncherButton button in config.Buttons
                     .Where(x => x.Enabled)
                     .OrderBy(x => x.Order))
        {
            MenuItems.Add(new MenuItemModel
            {
                Id = button.Id,
                Text = button.Text,
                Icon = button.Icon,
                Action = button.Action,
                Value = button.Value,
                Command = new RelayCommand(_ => ExecuteButton(button))
            });
        }
    }

    private void ExecuteButton(LauncherButton button)
    {
        switch (button.Action.ToLower())
        {
            case "open_url":
                OpenUrl(button.Value);
                break;

            case "start_game":
                StartGame();
                break;

            case "open_page":
                CurrentPage = _navigationService.Navigate(button.Value);
                break;

            case "check_update":
                CheckUpdate();
                break;
        }
    }

    private static void OpenUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    private void StartGame()
    {
        Debug.WriteLine("Start Game");
    }

    private void CheckUpdate()
    {
        Debug.WriteLine("Check Update");
    }
}