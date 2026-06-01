using System.Net.Http;
using System.Text.Json;
using ItzyLauncher.Models;

namespace ItzyLauncher.Services;

public sealed class ServerStatusService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Dictionary<string, GameServer> _serverCache = new();

    public event EventHandler<List<GameServer>>? ServersUpdated;
    public event EventHandler<Exception>? ErrorOccurred;

    public ServerStatusService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<GameServer>> FetchServerStatusAsync(List<GameServer> servers)
    {
        try
        {
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                List<GameServer> updatedServers = new();

                foreach (GameServer server in servers)
                {
                    GameServer updatedServer = await CheckServerStatusAsync(client, server);
                    updatedServers.Add(updatedServer);
                    _serverCache[server.Id] = updatedServer;
                }

                ServersUpdated?.Invoke(this, updatedServers);
                return updatedServers;
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
            return servers;
        }
    }

    public async Task<GameServer> CheckServerStatusAsync(GameServer server)
    {
        try
        {
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                return await CheckServerStatusAsync(client, server);
            }
        }
        catch
        {
            return server with { IsOnline = false };
        }
    }

    private static async Task<GameServer> CheckServerStatusAsync(HttpClient client, GameServer server)
    {
        try
        {
            // Try a simple HTTP request to a health check endpoint
            string healthCheckUrl = $"http://{server.Host}:{server.Port}/health";

            using (HttpRequestMessage request = new(HttpMethod.Get, healthCheckUrl))
            {
                using (CancellationTokenSource cts = new(TimeSpan.FromSeconds(5)))
                {
                    using (HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return server with { IsOnline = true };
                        }
                    }
                }
            }
        }
        catch
        {
            // Server is offline or unreachable
        }

        return server with { IsOnline = false };
    }

    public GameServer? GetCachedServer(string serverId)
    {
        return _serverCache.TryGetValue(serverId, out GameServer? server) ? server : null;
    }

    public void ClearCache()
    {
        _serverCache.Clear();
    }
}
