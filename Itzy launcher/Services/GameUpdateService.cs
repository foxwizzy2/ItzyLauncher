using System.Net.Http;
using System.Security.Cryptography;
using System.IO;

namespace ItzyLauncher.Services;

public sealed class GameUpdateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private CancellationTokenSource? _cancellationTokenSource;

    public event EventHandler<(long BytesDownloaded, long TotalBytes)>? ProgressChanged;
    public event EventHandler<string>? StatusChanged;
    public event EventHandler<Exception>? ErrorOccurred;

    public GameUpdateService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> DownloadFileAsync(
        string downloadUrl,
        string destinationPath,
        string? expectedHash = null)
    {
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();

            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);

                using (HttpResponseMessage response = await client.GetAsync(
                    downloadUrl,
                    HttpCompletionOption.ResponseHeadersRead,
                    _cancellationTokenSource.Token))
                {
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? 0L;
                    
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream fileStream = new(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            byte[] buffer = new byte[8192];
                            long totalRead = 0L;
                            int bytesRead;

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalRead += bytesRead;

                                ProgressChanged?.Invoke(this, (totalRead, totalBytes));
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(expectedHash))
                {
                    StatusChanged?.Invoke(this, "Verifying file integrity...");
                    
                    if (!await VerifyFileHashAsync(destinationPath, expectedHash))
                    {
                        File.Delete(destinationPath);
                        throw new InvalidOperationException("File hash verification failed");
                    }
                }

                StatusChanged?.Invoke(this, "Download complete");
                return true;
            }
        }
        catch (OperationCanceledException)
        {
            StatusChanged?.Invoke(this, "Download cancelled");
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            return false;
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            return false;
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
        }
    }

    public void CancelDownload()
    {
        _cancellationTokenSource?.Cancel();
    }

    private static async Task<bool> VerifyFileHashAsync(string filePath, string expectedHash)
    {
        try
        {
            using (FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(fileStream));
                    string computedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    return computedHash == expectedHash.ToLowerInvariant();
                }
            }
        }
        catch
        {
            return false;
        }
    }

    public static async Task<string> ComputeFileHashAsync(string filePath)
    {
        try
        {
            using (FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(fileStream));
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        catch
        {
            return "";
        }
    }
}
