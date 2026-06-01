# Quick Start Guide - Modern MU Online Launcher

## 5-Minute Setup

### Step 1: Clone or Open Project
```bash
cd Itzy_launcher
```

### Step 2: Create Configuration File

Create `launcher.config.json` in the root directory:

```json
{
  "launcherName": "ItzyMU Launcher",
  "clientVersion": "1.0.0",
  "maintenanceMode": false,
  "gameExecutablePath": "C:\\Games\\MU Online\\Client.exe",
  "gameDirectory": "C:\\Games\\MU Online",
  "theme": {
    "accentColor": "#8B5CF6",
    "backgroundImage": "",
    "logo": ""
  },
  "buttons": [
    {
      "id": "home",
      "text": "Home",
      "icon": "🏠",
      "enabled": true,
      "action": "open_page",
      "value": "home",
      "order": 1
    },
    {
      "id": "account",
      "text": "Account",
      "icon": "👤",
      "enabled": true,
      "action": "open_page",
      "value": "account",
      "order": 2
    },
    {
      "id": "settings",
      "text": "Settings",
      "icon": "⚙️",
      "enabled": true,
      "action": "open_page",
      "value": "settings",
      "order": 3
    }
  ],
  "servers": [
    {
      "id": "server1",
      "name": "Main Server",
      "host": "game.example.com",
      "port": 55901,
      "region": "Global",
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

### Step 3: Update Game Path

Edit the config and set:
- `gameExecutablePath`: Full path to your game executable
- `gameDirectory`: Directory containing game files

### Step 4: Run the Launcher

```bash
dotnet run
```

Or build and run from Visual Studio.

## Features to Try

### 🎮 Launch Game
1. Select a server from the "Select Server" dropdown
2. Click the "PLAY" button
3. Game launches with server parameters

### 📊 Check Server Status
- Server online/offline status updates automatically
- Player count is displayed
- Color changes based on status

### ⚙️ Configure Settings
1. Click "Settings" in the sidebar
2. Toggle launcher preferences
3. Adjust graphics quality

### 📱 View Account Info
1. Click "Account" in the sidebar
2. See character stats
3. Check account status

## Customization Tips

### Change Colors
Edit the accent color in config:
```json
"theme": {
  "accentColor": "#FF6B6B"  // Red instead of purple
}
```

### Add Custom Buttons
Add new buttons in config:
```json
{
  "id": "news",
  "text": "News",
  "icon": "📰",
  "enabled": true,
  "action": "open_url",
  "value": "https://example.com/news",
  "order": 4
}
```

### Add More Servers
```json
{
  "id": "server2",
  "name": "PvP Server",
  "host": "pvp.example.com",
  "port": 55901,
  "region": "North America",
  "order": 2
}
```

## Troubleshooting

### Launcher Won't Start
- Ensure .NET 10 is installed: `dotnet --version`
- Check config file exists in the right location
- Review console output for errors

### Game Won't Launch
- Verify game executable path is correct
- Ensure game is not already running
- Check firewall isn't blocking the game

### Server Shows Offline
- Check network connectivity
- Verify server host/port in config
- Make sure server is actually running

### Config Not Loading
- File must be named exactly `launcher.config.json`
- Ensure JSON is valid (use online JSON validator)
- Check file permissions

## Next Steps

1. **Customize Branding**: Update `launcherName`, colors, and logo
2. **Add Real Servers**: Configure your actual game servers
3. **Set Up Authentication**: Integrate with your auth backend
4. **Add News**: Create a news endpoint for patch notes
5. **Test on Clean Machine**: Ensure all dependencies are packaged

## File Structure

```
Itzy launcher/
├── App.xaml                    # Application entry point
├── MainWindow.xaml             # Main window UI
├── Models/                     # Data models
│   ├── LauncherConfig.cs
│   ├── GameServer.cs
│   ├── GameAccount.cs
│   └── GameUpdate.cs
├── Services/                   # Business logic
│   ├── GameProcessService.cs
│   ├── GameUpdateService.cs
│   ├── ServerStatusService.cs
│   └── AuthenticationService.cs
├── ViewModels/                 # MVVM ViewModels
│   ├── MainViewModel.cs
│   └── Pages/
├── Views/                      # UI Views
│   ├── MainWindow.xaml
│   └── Pages/
│       ├── HomePageView.xaml
│       ├── AccountPageView.xaml
│       └── SettingsPageView.xaml
├── Helpers/                    # Utilities
│   ├── RelayCommand.cs
│   └── ValueConverters.cs
└── launcher.config.json        # Configuration file
```

## Support

For issues or questions:
1. Check DEVELOPMENT_GUIDE.md for detailed information
2. Review the example config file
3. Check application output for error messages
4. Verify all paths and network connectivity

Good luck with your MU Online launcher! 🚀

