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
    private readonly GameProcessService _gameProcessService;
    private readonly GameUpdateService _gameUpdateService;
    private readonly ServerStatusService _serverStatusService;
    private readonly AuthenticationService _authenticationService;

    private string _launcherTitle = "Loading...";
    private string _clientVersion = "";
    private string _accentColor = "#8B5CF6";
    private string _backgroundImage = "";
    private string _logo = "";
    private bool _maintenanceMode;
    private PageViewModelBase? _currentPage;
    private string _serverStatus = "Loading...";
    private string _playerCount = "0";
    private bool _isGameRunning;
    private string _launchButtonText = "PLAY";
    private double _updateProgress;
    private bool _showUpdateProgress;
    private string _updateStatus = "";
    private GameServer? _selectedServer;
    private LauncherConfig? _config;

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

    public string ServerStatus
    {
        get => _serverStatus;
        set => SetProperty(ref _serverStatus, value);
    }

    public string PlayerCount
    {
        get => _playerCount;
        set => SetProperty(ref _playerCount, value);
    }

    public bool IsGameRunning
    {
        get => _isGameRunning;
        set => SetProperty(ref _isGameRunning, value);
    }

    public string LaunchButtonText
    {
        get => _launchButtonText;
        set => SetProperty(ref _launchButtonText, value);
    }

    public double UpdateProgress
    {
        get => _updateProgress;
        set => SetProperty(ref _updateProgress, value);
    }

    public bool ShowUpdateProgress
    {
        get => _showUpdateProgress;
        set => SetProperty(ref _showUpdateProgress, value);
    }

    public string UpdateStatus
    {
        get => _updateStatus;
        set => SetProperty(ref _updateStatus, value);
    }

    public GameServer? SelectedServer
    {
        get => _selectedServer;
        set => SetProperty(ref _selectedServer, value);
    }

    public ObservableCollection<MenuItemModel> MenuItems { get; } = new();
    public ObservableCollection<GameServer> Servers { get; } = new();

    public RelayCommand LaunchGameCommand { get; }
    public RelayCommand CheckUpdatesCommand { get; }
    public RelayCommand SelectServerCommand { get; }

    public MainViewModel(
        ConfigService configService,
        NavigationService navigationService,
        ThemeService themeService,
        GameProcessService gameProcessService,
        GameUpdateService gameUpdateService,
        ServerStatusService serverStatusService,
        AuthenticationService authenticationService)
    {
        _configService = configService;
        _navigationService = navigationService;
        _themeService = themeService;
        _gameProcessService = gameProcessService;
        _gameUpdateService = gameUpdateService;
        _serverStatusService = serverStatusService;
        _authenticationService = authenticationService;

        LaunchGameCommand = new RelayCommand(_ => LaunchGame());
        CheckUpdatesCommand = new RelayCommand(_ => CheckUpdates());
        SelectServerCommand = new RelayCommand(obj => SelectServer(obj));

        _gameProcessService.GameStarted += (s, e) => IsGameRunning = true;
        _gameProcessService.GameExited += (s, e) => { IsGameRunning = false; LaunchButtonText = "PLAY"; };

        _gameUpdateService.ProgressChanged += (s, e) =>
        {
            UpdateProgress = (e.BytesDownloaded / (double)e.TotalBytes) * 100;
        };

        _gameUpdateService.StatusChanged += (s, status) =>
        {
            UpdateStatus = status;
        };

        CurrentPage = _navigationService.Navigate("home");

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        _config = await _configService.LoadAsync();

        LauncherTitle = _config.LauncherName;
        ClientVersion = _config.ClientVersion;
        MaintenanceMode = _config.MaintenanceMode;

        _themeService.ApplyTheme(_config.Theme);

        AccentColor = _config.Theme.AccentColor;
        BackgroundImage = _config.Theme.BackgroundImage;
        Logo = _config.Theme.Logo;

        BuildMenu(_config);
        await LoadServersAsync(_config);
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

    private async Task LoadServersAsync(LauncherConfig config)
    {
        Servers.Clear();

        foreach (GameServer server in config.Servers.OrderBy(x => x.Order))
        {
            Servers.Add(server);
        }

        if (Servers.Count > 0)
        {
            SelectedServer = Servers[0];
        }

        // Refresh server statuses
        _ = RefreshServerStatusAsync();
    }

    private async Task RefreshServerStatusAsync()
    {
        List<GameServer> updatedServers = await _serverStatusService.FetchServerStatusAsync(Servers.ToList());

        for (int i = 0; i < updatedServers.Count; i++)
        {
            if (i < Servers.Count)
            {
                Servers[i] = updatedServers[i];
            }
        }

        if (SelectedServer != null && updatedServers.FirstOrDefault(x => x.Id == SelectedServer.Id) is GameServer updated)
        {
            SelectedServer = updated;
            UpdateServerStatus();
        }
    }

    private void UpdateServerStatus()
    {
        if (SelectedServer != null)
        {
            ServerStatus = SelectedServer.IsOnline ? "ONLINE" : "OFFLINE";
            PlayerCount = $"Players: {SelectedServer.OnlineCount}";
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
                LaunchGame();
                break;

            case "open_page":
                CurrentPage = _navigationService.Navigate(button.Value);
                break;

            case "check_update":
                CheckUpdates();
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

    private async void LaunchGame()
    {
        if (IsGameRunning)
        {
            _gameProcessService.KillGame();
            LaunchButtonText = "PLAY";
            return;
        }

        if (_config?.GameExecutablePath == null || string.IsNullOrEmpty(_config.GameExecutablePath))
        {
            UpdateStatus = "Game path not configured";
            return;
        }

        string gameParams = $"--server {SelectedServer?.Host ?? "localhost"} --port {SelectedServer?.Port ?? 55901}";

        bool success = await _gameProcessService.LaunchGameAsync(_config.GameExecutablePath, gameParams);

        if (success)
        {
            LaunchButtonText = "STOP";
        }
        else
        {
            UpdateStatus = "Failed to launch game";
        }
    }

    private async void CheckUpdates()
    {
        ShowUpdateProgress = true;
        UpdateStatus = "Checking for updates...";

        // Mock update check - replace with real server check
        await Task.Delay(500);

        UpdateStatus = "Your game is up to date";
        ShowUpdateProgress = false;
    }

    private void SelectServer(object? obj)
    {
        if (obj is GameServer server)
        {
            SelectedServer = server;
            UpdateServerStatus();
        }
    }
}