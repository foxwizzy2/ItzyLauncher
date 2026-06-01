namespace ItzyLauncher.Models;

public sealed class LauncherConfig
{
    public string LauncherName { get; set; } = "ItzyMU Launcher";
    public string ClientVersion { get; set; } = "1.0.0";
    public bool MaintenanceMode { get; set; }
    public string GameExecutablePath { get; set; } = "";
    public string GameDirectory { get; set; } = "";

    public LauncherTheme Theme { get; set; } = new();
    public List<LauncherButton> Buttons { get; set; } = new();
    public LauncherLinks Links { get; set; } = new();
    public List<GameServer> Servers { get; set; } = new();
}

public sealed class LauncherTheme
{
    public string AccentColor { get; set; } = "#8B5CF6";
    public string BackgroundImage { get; set; } = "";
    public string Logo { get; set; } = "";
}

public sealed class LauncherButton
{
    public string Id { get; set; } = "";
    public string Text { get; set; } = "";
    public string Icon { get; set; } = "";
    public bool Enabled { get; set; } = true;

    public string Action { get; set; } = "open_page";
    public string Value { get; set; } = "";
    public int Order { get; set; }
}

public sealed class LauncherLinks
{
    public string Website { get; set; } = "";
    public string Discord { get; set; } = "";
    public string Register { get; set; } = "";
}