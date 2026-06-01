using System.IO;
using System.Net.Http;
using System.Text.Json;
using ItzyLauncher.Models;

namespace ItzyLauncher.Services;

public sealed class ConfigService
{
    private const string SettingsFileName = "launcher.settings.json";

    private readonly IHttpClientFactory _httpClientFactory;

    public ConfigService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LauncherConfig> LoadAsync()
    {
        LauncherSettings settings = await LoadSettingsAsync();

        try
        {
            string json;

            if (settings.ConfigUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                HttpClient client = _httpClientFactory.CreateClient();
                json = await client.GetStringAsync(settings.ConfigUrl);
            }
            else
            {
                json = await File.ReadAllTextAsync(settings.ConfigUrl);
            }

            LauncherConfig? config = JsonSerializer.Deserialize<LauncherConfig>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return config ?? new LauncherConfig();
        }
        catch
        {
            return new LauncherConfig();
        }
    }

    private static async Task<LauncherSettings> LoadSettingsAsync()
    {
        if (!File.Exists(SettingsFileName))
        {
            LauncherSettings defaultSettings = new();

            string defaultJson = JsonSerializer.Serialize(
                defaultSettings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(SettingsFileName, defaultJson);

            return defaultSettings;
        }

        string json = await File.ReadAllTextAsync(SettingsFileName);

        return JsonSerializer.Deserialize<LauncherSettings>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new LauncherSettings();
    }
}