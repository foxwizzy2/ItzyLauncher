using System.Windows;
using ItzyLauncher.Services;
using ItzyLauncher.ViewModels;
using ItzyLauncher.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ItzyLauncher;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddHttpClient();

                services.AddSingleton<ConfigService>();
                services.AddSingleton<NavigationService>();
                services.AddSingleton<ThemeService>();
                services.AddSingleton<GameProcessService>();
                services.AddSingleton<GameUpdateService>();
                services.AddSingleton<ServerStatusService>();
                services.AddSingleton<AuthenticationService>();

                services.AddSingleton<MainViewModel>();
                services.AddSingleton<HomePageViewModel>();
                services.AddSingleton<AccountPageViewModel>();
                services.AddSingleton<SettingsPageViewModel>();
                
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}