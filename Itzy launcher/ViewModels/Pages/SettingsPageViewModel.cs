using ItzyLauncher.Services;

namespace ItzyLauncher.ViewModels.Pages;

public sealed class SettingsPageViewModel : PageViewModelBase
{
    private bool _enableSounds = true;
    private bool _autoLaunch;
    private bool _keepLauncherOpen = true;
    private int _graphicsQuality = 2;
    private string _gameDirectory = "";

    public bool EnableSounds
    {
        get => _enableSounds;
        set => SetProperty(ref _enableSounds, value);
    }

    public bool AutoLaunch
    {
        get => _autoLaunch;
        set => SetProperty(ref _autoLaunch, value);
    }

    public bool KeepLauncherOpen
    {
        get => _keepLauncherOpen;
        set => SetProperty(ref _keepLauncherOpen, value);
    }

    public int GraphicsQuality
    {
        get => _graphicsQuality;
        set => SetProperty(ref _graphicsQuality, value);
    }

    public string GameDirectory
    {
        get => _gameDirectory;
        set => SetProperty(ref _gameDirectory, value);
    }

    public SettingsPageViewModel()
    {
    }
}
