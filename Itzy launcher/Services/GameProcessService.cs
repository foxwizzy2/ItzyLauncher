using System.Diagnostics;
using System.IO;

namespace ItzyLauncher.Services;

public sealed class GameProcessService
{
    private Process? _gameProcess;
    private readonly string _gameExecutablePath;

    public event EventHandler<EventArgs>? GameStarted;
    public event EventHandler<EventArgs>? GameExited;

    public bool IsGameRunning => _gameProcess?.HasExited == false;

    public GameProcessService(string gameExecutablePath = "")
    {
        _gameExecutablePath = gameExecutablePath;
    }

    public void SetGamePath(string gamePath)
    {
        if (string.IsNullOrEmpty(gamePath) || !File.Exists(gamePath))
        {
            throw new FileNotFoundException($"Game executable not found: {gamePath}");
        }

        // Store game path for later use
    }

    public async Task<bool> LaunchGameAsync(string gamePath, string gameParameters = "")
    {
        try
        {
            if (IsGameRunning)
            {
                return false; // Game already running
            }

            if (!File.Exists(gamePath))
            {
                throw new FileNotFoundException($"Game executable not found: {gamePath}");
            }

            ProcessStartInfo psi = new()
            {
                FileName = gamePath,
                Arguments = gameParameters,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(gamePath) ?? ""
            };

            _gameProcess = Process.Start(psi);

            if (_gameProcess == null)
            {
                return false;
            }

            GameStarted?.Invoke(this, EventArgs.Empty);

            _ = Task.Run(async () =>
            {
                await _gameProcess.WaitForExitAsync();
                GameExited?.Invoke(this, EventArgs.Empty);
            });

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to launch game: {ex.Message}");
            return false;
        }
    }

    public void KillGame()
    {
        if (IsGameRunning && _gameProcess != null)
        {
            try
            {
                _gameProcess.Kill();
            }
            catch
            {
                // Game already closed
            }
        }
    }

    public async Task WaitForGameExitAsync()
    {
        if (_gameProcess != null)
        {
            await _gameProcess.WaitForExitAsync();
        }
    }
}
