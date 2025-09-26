# TeamLimit Plugin for Counter-Strike 2

**TeamLimit** is a CounterStrikeSharp plugin for Counter-Strike 2 that restricts the number of players per team (Terrorist and Counter-Terrorist) to ensure balanced gameplay. When a player attempts to join a full team, they are moved to spectator mode, hear an error sound, and receive a chat message (with a cooldown to prevent spam). Perfect for PUG, MIX, or Retake servers.

## Features
- Configurable maximum players per team via `TeamLimit.json`.
- Moves players to spectator mode when they try to join a full team.
- Plays the CS2 sound `sounds/ui/weapon_cant_buy.vsnd` on failed join attempts.
- Sends a chat message (e.g., "The Terrorist team is full!") with a 10-second cooldown per team to prevent spam.
- Debug logging to the server console for troubleshooting.
- Compatible with CounterStrikeSharp v1.0.340 (September 2025).

## Installation
1. **Install Dependencies**:
   - Ensure [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) v1.0.340 or later and Metamod:Source are installed on your CS2 server.

2. **Clone or Download**:
   ```bash
   git clone https://github.com/Kenkyusha1/TeamLimit.git
   cd TeamLimit
   ```

3. **Build the Plugin**:
   ```bash
   dotnet restore
   dotnet build -c Release
   ```
   Output: `bin/Release/net8.0/TeamLimit.dll`

4. **Deploy to Server**:
   - Copy `TeamLimit.dll` to:
     ```
     game/csgo/addons/counterstrikesharp/plugins/TeamLimit/TeamLimit.dll
     ```
   - Create the `TeamLimit` folder if it doesn’t exist.

5. **Load the Plugin**:
   - Start your CS2 server.
   - Run in the server console:
     ```bash
     css_plugins load TeamLimit
     ```
   - Verify console output:
     ```
     [TeamLimit] Plugin loaded successfully
     [TeamLimit] Config loaded: MaxPlayersPerTeam = 5
     ```

## Configuration
The plugin generates `TeamLimit.json` in:
```
game/csgo/addons/counterstrikesharp/configs/plugins/TeamLimit/
```

### Default Configuration
```json
{
  "MaxPlayersPerTeam": 5
}
```

- **MaxPlayersPerTeam**: Maximum players allowed per team (default: 5).
- **Note**: The 10-second cooldown per team (Terrorist and Counter-Terrorist) is hardcoded.

### Editing the Config
- Update `MaxPlayersPerTeam` (e.g., `3` for smaller teams).
- Reload the plugin:
  ```bash
  css_plugins reload TeamLimit
  ```

## Contributing
Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/YourFeature`).
3. Commit changes (`git commit -m "Add YourFeature"`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.

## Acknowledgments
- Built with [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp).
- Inspired by community plugins like [CS2-TeamLimiter](https://github.com/Ferks-FK/CS2-TeamLimiter).
