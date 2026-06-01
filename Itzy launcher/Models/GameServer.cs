namespace ItzyLauncher.Models;

public sealed record GameServer(
    string Id,
    string Name,
    string Host,
    int Port,
    bool IsOnline,
    int OnlineCount,
    string Region,
    int Order)
{
    public GameServer() : this("", "", "", 0, false, 0, "", 0) { }
}
