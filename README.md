# KillLogsDiscordBot Plugin

A plugin for **SCP: Secret Laboratory** that logs kill events (e.g., "Player X killed Player Y with Z") and sends them to a specified Discord channel using a Discord bot token. Built for **EXILED 9.6.1** and **SCP:SL v14.1.1**, it integrates seamlessly.

## Features
- Logs kill events with details (attacker, victim, weapon, timestamp).
- Sends logs as Discord Embeds to a specified channel for a clean, formatted look.
- Configurable options to include/exclude SCP kills or suicides.
- Debug logging for easy troubleshooting.
- Uses Discord.Net for reliable bot integration.

## Requirements
- **SCP: Secret Laboratory** v13.2
- **EXILED** v9.201.0
- Dependencies: `Exiled.API`, `Exiled.Events`, `Assembly-CSharp`, `Newtonsoft.Json`, `Discord.Net`
- .NET Framework 4.7.2
- A Discord bot with a valid token and permissions to send messages in the target channel

## Installation

### 1. Create a Discord Bot
1. Go to the [Discord Developer Portal](https://discord.com/developers/applications).
2. Create a new application (e.g., "KillLogsBot") and add a bot under the **Bot** tab.
3. Copy the **Bot Token** (e.g., `=Token`). **Keep it secure**.
4. Under **OAuth2 > URL Generator**:
   - Select **bot** scope.
   - Grant **Send Messages** and **Embed Links** permissions.
   - Use the generated URL to invite the bot to your Discord server.
5. Enable Developer Mode in Discord (Settings > Appearance > Developer Mode).
6. Right-click the target channel (e.g., `#kill-logs`) and copy its **Channel ID** (e.g., `123456789012345678`).

### 2. Set Up the Plugin
1. **Create the Project**:
   - Open Visual Studio (or any .NET Framework 4.8-compatible IDE).
   - Create a new **Class Library** project named `KillLogsDiscordBot`.
   - Add the provided files: `Plugin.cs`, `Config.cs`, `KillLogsDiscordBot.csproj`.

2. **Install Dependencies**:
   - Ensure the following are in `%appdata%\EXILED\Plugins\dependencies`:
     - `Exiled.API.dll`
     - `Exiled.Events.dll`
     - `Assembly-CSharp.dll`
     - `Newtonsoft.Json.dll`
   - Install `Discord.Net` (v3.15.3) via NuGet:
     - In Visual Studio, go to **Tools > NuGet Package Manager > Manage NuGet Packages for Solution**.
     - Search for `Discord.Net` and install version 3.15.3.
   - Alternatively, copy `Discord.Net.dll` to `%appdata%\EXILED\Plugins\dependencies`.

3. **Build the Project**:
   - In Visual Studio, click **Build > Build Solution**.
   - Copy `KillLogsDiscordBot.dll` from `[project_path]\bin\Debug\net48` to `%appdata%\EXILED\Plugins`.

4. **Configure the Plugin**:
   - Create or edit `Config.yml` in `%appdata%\EXILED\Configs`.
   - Add the following, replacing placeholders with your bot token and channel ID:
     ```yaml
     kill_logs_discord_bot:
       is_enabled: true
       debug: true
       discord_bot_token: "YOUR_BOT_TOKEN_HERE"
       discord_channel_id: id Channel
       log_scp_kills: false
       log_suicides: true
     ```

5. **Run the Server**:
   - Restart your SCP:SL server.
   - Check the logs in `%appdata%\EXILED\Logs` for:
     ```
     KillLogsDiscordBot enabled.
     Discord bot connected successfully.
     ```

## Usage
- Start a round in SCP:SL.
- When a kill occurs (e.g., MTF kills Class-D), the plugin sends a message to the specified Discord channel, like: Player KillMTFPlayer (MTF) killed ClassDPlayer (ClassD) with GunCOM15. [2025-06-26 13:27:00]
- Suicides or non-attacker deaths (if enabled) appear as: Player DeathClassDPlayer (ClassD) committed suicide or died. [2025-06-26 13:27:00]


## Configuration Options
- `is_enabled`: Enable/disable the plugin (default: `true`).
- `debug`: Enable debug logging for troubleshooting (default: `true`).
- `discord_bot_token`: Your Discord bot token (required).
- `discord_channel_id`: The ID of the Discord channel to send logs to (required).
- `log_scp_kills`: Log kills by SCPs (default: `false`).
- `log_suicides`: Log suicides or deaths without an attacker (default: `true`).

## Troubleshooting
- **Bot not connecting**:
- Verify the `discord_bot_token` in `Config.yml`.
- Ensure the bot is invited to the server with correct permissions.
- Check logs for errors like "Failed to connect Discord bot: Invalid token".
- **Messages not appearing in Discord**:
- Confirm the `discord_channel_id` is correct.
- Ensure the bot has `Send Messages` and `Embed Links` permissions.
- Check logs for errors like "Failed to find Discord channel".
- **Plugin not loading**:
- Run `RA> plugins list` to confirm `KillLogsDiscordBot` is listed.
- Verify all dependencies are in `%appdata%\EXILED\Plugins\dependencies`.
- Ensure `Config.yml` is in `%appdata%\EXILED\Configs` with correct YAML formatting (2-space indentation, no tabs).
- **No kill logs**:
- Ensure the round is active (`Round.IsStarted`).
- Test with a non-SCP kill (e.g., MTF vs. Class-D).
- Enable `debug: true` in `Config.yml` and check logs for messages like "Sending kill log: ...".

## Compatibility
- Compatible with EXILED 9.6.2 and SCP:SL v14.1.1.
- If conflicts occur (e.g., with `Player.Dying` events), contact the developer for an event priority patch.

## Notes
- **Security**: Never share your bot token, as it controls the bot.
- **Rate Limits**: If you see "429 Too Many Requests" errors, reduce the frequency of messages (contact the developer for a delay patch).
- **Enhancements**: To add features like Discord commands (e.g., `!status`) or additional event logging (e.g., escapes), contact the developer.

## License
Licensed under [CC BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/).

## Support
For issues or feature requests, contact the developer or check the plugin's repository for updates.
## Example:
![image](https://github.com/user-attachments/assets/ba615c76-047d-4780-99e6-3cabe2975e19)
