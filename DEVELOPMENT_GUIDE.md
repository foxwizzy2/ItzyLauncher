# Modern MU Online Launcher - Development Guide

## Overview

This is a modern, feature-rich launcher for MU Online built with WPF and .NET 10. It provides a sleek user interface with comprehensive game management capabilities.

## Features Implemented

### 🎮 Core Gaming Features
- **Game Process Management**: Launch and manage game instances
- **Server Selection**: Multiple server support with online status
- **Game Updates**: Download and patch game files with progress tracking
- **File Integrity Verification**: SHA256 hash validation for updated files

### 🔐 Account Management
- **Authentication Service**: Login/logout functionality
- **Account Display**: Show character information, stats, and progress
- **Session Management**: Token-based authentication

### 🎨 Modern UI
- **Dark Theme**: Professional dark mode interface with accent colors
- **Responsive Design**: Adapts to different window sizes
- **Real-time Status**: Server status and player counts
- **Progress Indicators**: Visual feedback for updates and downloads

### ⚙️ Configuration
- **JSON-based Config**: Easy-to-edit configuration files
- **Server Management**: Configure multiple game servers
- **Menu Customization**: Dynamic menu buttons
- **Theme Customization**: Accent colors and branding

### 📱 Pages
1. **Home** - Main launcher with server selection and play button
2. **Account** - Character management and account stats
3. **Settings** - Launcher preferences and game settings

## Architecture

### Services
- **GameProcessService**: Manages game process lifecycle
- **GameUpdateService**: Handles downloading and patching
- **ServerStatusService**: Monitors server status
- **AuthenticationService**: Manages user authentication
- **ConfigService**: Loads configuration from JSON
- **NavigationService**: Manages page navigation
- **ThemeService**: Applies visual themes

### ViewModels (MVVM Pattern)
- **MainViewModel**: Main application logic
- **HomePageViewModel**: Home page logic
- **AccountPageViewModel**: Account page logic
- **SettingsPageViewModel**: Settings page logic

### Models
- **GameServer**: Server configuration and status
- **GameAccount**: User account information
- **GameUpdate**: Update package information
- **LauncherConfig**: Main launcher configuration
- **LoginCredentials**: Authentication credentials

## Configuration

### Creating a Config File

Create `launcher.settings.json` in the application directory:

```json
{
  "configUrl": "launcher.config.json"
}
```

Or point to a remote config:

```json
{
  "configUrl": "https://your-server.com/api/launcher/config"
}
```

### Launcher Configuration

Create `launcher.config.json` with your settings:

```json
{
  "launcherName": "ItzyMU Launcher",
  "clientVersion": "1.0.0",
  "maintenanceMode": false,
  "gameExecutablePath": "C:\\MU Online\\MU.exe",
  "gameDirectory": "C:\\MU Online",
  "theme": {
    "accentColor": "#8B5CF6",
    "backgroundImage": "",
    "logo": ""
  },
  "servers": [
    {
      "id": "us-east",
      "name": "US East",
      "host": "us-east.example.com",
      "port": 55901,
      "region": "North America",
      "order": 1
    }
  ],
  "links": {
    "website": "https://example.com",
    "discord": "https://discord.gg/example",
    "register": "https://example.com/register"
  }
}
```

## Building and Running

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026 or later (or any compatible IDE)

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

## Development Guidelines

### Adding New Services
1. Create service class in `Services/` folder
2. Implement service interface (if needed)
3. Register in `App.xaml.cs` DependencyInjection
4. Inject into ViewModel constructor

### Adding New Pages
1. Create ViewModel in `ViewModels/Pages/`
2. Create View (XAML) in `Views/Pages/`
3. Create View code-behind in `Views/Pages/`
4. Register in `App.xaml` DataTemplate and App.xaml.cs DependencyInjection
5. Add to NavigationService

### Adding UI Elements
- Use Border for rounded corners (no native CornerRadius on Button/TextBox)
- Follow color scheme: `#8B5CF6` (accent), `#171B29` (backgrounds), `#8D93A8` (text)
- Use converters for data transformations in XAML

## Available Converters

- `BoolToVisibilityConverter`: Convert bool to Visibility
- `BoolToColorConverter`: Convert bool to color
- `ServerStatusColorConverter`: Color based on server status
- `InvertBoolConverter`: Invert boolean values
- `GraphicsQualityConverter`: Convert int to quality name
- `ProgressConverter`: Convert percentage to width

## API Integration

### Authentication Endpoint
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "user",
  "password": "pass"
}

Response:
{
  "token": "jwt-token-here",
  "username": "user",
  "message": "Login successful"
}
```

### Server Status Endpoint
```
GET /health
Response: 200 OK
```

### File Download
- Files are downloaded with progress tracking
- SHA256 hash verification is performed
- Automatic retry on failure

## Game Launch Parameters

The launcher passes these parameters to the game:
```
--server {host} --port {port}
```

Customize in `MainViewModel.LaunchGame()` method.

## Debugging

### Enable Debug Output
```csharp
System.Diagnostics.Debug.WriteLine("Your message");
```

### Common Issues

**Config file not found**: 
- Create `launcher.settings.json` in the application directory
- Ensure file permissions are correct

**Game won't launch**:
- Verify `gameExecutablePath` in config is correct
- Check game is not already running
- Review debug output for error messages

**Server status showing offline**:
- Check network connectivity
- Verify server host and port in config
- Check firewall settings

## Future Enhancements

- [ ] Game auto-update on launch
- [ ] Patch notes viewer
- [ ] In-game chat integration
- [ ] Friend list
- [ ] News ticker
- [ ] Rankings display
- [ ] Account security (2FA)
- [ ] Multiple language support
- [ ] Game settings editor
- [ ] Bug report system
- [ ] Performance profiler

## Dependencies

- Microsoft.NET.Sdk (.NET 10)
- Microsoft.Extensions.Hosting (10.0.8)
- Microsoft.Extensions.Http (10.0.8)
- System.Net.Http (built-in)
- System.Text.Json (built-in)
- System.Security.Cryptography (built-in)

## License

Specify your license here.

## Contact & Support

For issues and feature requests, please visit:
- Website: https://github.com/foxwizzy2/Itzylauncher
- Discord: https://discord.gg/example

