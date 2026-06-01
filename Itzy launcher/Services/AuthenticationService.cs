using System.Net.Http;
using System.Text;
using System.Text.Json;
using ItzyLauncher.Models;

namespace ItzyLauncher.Services;

public sealed class AuthenticationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private string? _authToken;
    private GameAccount? _currentAccount;

    public event EventHandler<GameAccount>? LoginSuccess;
    public event EventHandler<string>? LoginFailed;
    public event EventHandler? LogoutSuccess;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);
    public GameAccount? CurrentAccount => _currentAccount;

    public AuthenticationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> LoginAsync(string username, string password, string? serverUrl = null)
    {
        try
        {
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                var loginRequest = new { username, password };
                string json = JsonSerializer.Serialize(loginRequest);

                using (StringContent content = new(json, Encoding.UTF8, "application/json"))
                {
                    string url = $"{serverUrl ?? "http://localhost:8080"}/api/auth/login";
                    using (HttpResponseMessage response = await client.PostAsync(url, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            JsonDocument doc = JsonDocument.Parse(responseContent);

                            if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement))
                            {
                                _authToken = tokenElement.GetString();

                                // Mock account creation - replace with real server response
                                _currentAccount = new GameAccount
                                {
                                    Username = username,
                                    CharacterName = $"{username}_Hero",
                                    Level = 1,
                                    Class = "Knight",
                                    Experience = 0
                                };

                                LoginSuccess?.Invoke(this, _currentAccount);
                                return true;
                            }
                        }
                    }
                }
            }

            LoginFailed?.Invoke(this, "Invalid credentials");
            return false;
        }
        catch (Exception ex)
        {
            LoginFailed?.Invoke(this, ex.Message);
            return false;
        }
    }

    public void Logout()
    {
        _authToken = null;
        _currentAccount = null;
        LogoutSuccess?.Invoke(this, EventArgs.Empty);
    }

    public string? GetAuthToken()
    {
        return _authToken;
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;
    }
}
