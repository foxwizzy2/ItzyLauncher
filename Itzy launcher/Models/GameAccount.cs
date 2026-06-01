namespace ItzyLauncher.Models;

public sealed class GameAccount
{
    public string Username { get; set; } = "";
    public string CharacterName { get; set; } = "";
    public int Level { get; set; }
    public string Class { get; set; } = "";
    public long Experience { get; set; }
}

public sealed class LoginCredentials
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}
