using ItzyLauncher.Models;

namespace ItzyLauncher.Services;

public sealed class ThemeService
{
    public LauncherTheme CurrentTheme { get; private set; } = new();

    public void ApplyTheme(LauncherTheme theme)
    {
        CurrentTheme = theme;
    }
}