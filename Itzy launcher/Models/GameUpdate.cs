namespace ItzyLauncher.Models;

public sealed class GameUpdate
{
    public string Version { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime ReleaseDate { get; set; }
    public long FileSize { get; set; }
    public string DownloadUrl { get; set; } = "";
    public bool IsRequired { get; set; }
    public string Hash { get; set; } = "";
}

public sealed class UpdateProgress
{
    public long BytesDownloaded { get; set; }
    public long TotalBytes { get; set; }
    public double PercentComplete => TotalBytes > 0 ? (BytesDownloaded * 100.0) / TotalBytes : 0;
    public string Status { get; set; } = "Idle";
    public TimeSpan? EtaTimeRemaining { get; set; }
}
