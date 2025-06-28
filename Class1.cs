using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using Color = Discord.Color;

namespace KillLogsDiscordBot
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "KillLogsDiscordBot";
        public override string Author => "MONCEF50G";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 6, 1);

        private DiscordSocketClient _client;
        private bool _isBotConnected;

        public override void OnEnabled()
        {
            if (string.IsNullOrEmpty(Config.DiscordBotToken) || Config.DiscordChannelId == 0)
            {
                DisplayConsoleBanner();
                Log.Error("Discord Bot Token or Channel ID is not set in Config.yml. Plugin will not function.");
                return;
            }

            InitializeDiscordBot().GetAwaiter().GetResult();

            Exiled.Events.Handlers.Player.Dying += OnPlayerDying;
            Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Log.Info("KillLogsDiscordBot enabled with Join/Leave logging.");
            base.OnEnabled();
        }
        private void DisplayConsoleBanner()
        {
            string banner = @"
  _          _ _       _ _          _____ _            
 | |__   ___| | | ___ | | | ___    |  ___| | ___   ___ 
 | '_ \ / __| | |/ _ \| | |/ __|   | |_  | |/ _ \ / __|
 | | | | (__| | | (_) | | | (__)   |  _| | | (_) | (__)
 |_| |_| \___|_|_|\___/|_|_|\___|   |_|   |_| \___/ \___|

           KillLogsDiscordBot v1.2.0 by MONCEF50G
           Licensed under CC BY-SA 3.0 : DisLab:sl
           Logs Kills, Joins, and Leaves to Discord
";
            Log.Info(banner);
        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDying;
            Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            if (_client != null)
            {
                _client.Dispose();
                _isBotConnected = false;
                Log.Info("Discord bot disconnected.");
            }
            Log.Info("KillLogsDiscordBot disabled.");
            base.OnDisabled();
        }

        private async Task InitializeDiscordBot()
        {
            try
            {
                _client = new DiscordSocketClient();
                _client.Log += async (msg) =>
                {
                    Log.Debug($"Discord: {msg.ToString()}");
                    await Task.CompletedTask;
                };

                await _client.LoginAsync(TokenType.Bot, Config.DiscordBotToken);
                await _client.StartAsync();
                _isBotConnected = true;
                Log.Info("Discord bot connected successfully.");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to connect Discord bot: {ex.Message}");
                _isBotConnected = false;
            }
        }

        private async void OnPlayerDying(DyingEventArgs ev)
        {
            if (!Config.IsEnabled || !Round.IsStarted || !_isBotConnected)
                return;

            Player victim = ev.Player;
            Player attacker = ev.Attacker;

            if (victim == null || (attacker == null && !Config.LogSuicides) || (attacker != null && attacker.Role.Side == Side.Scp && !Config.LogScpKills))
                return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string victimName = victim?.Nickname ?? "Unknown";
            string victimRole = victim?.Role.Type.ToString() ?? "Unknown";

            var embedBuilder = new EmbedBuilder()
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("KillLogsDiscordBot");

            if (attacker == null)
            {
                embedBuilder.WithTitle("Player Death")
                    .WithDescription($"**{victimName}** ({victimRole}) committed suicide or died.\nTime: {timestamp}")
                    .WithColor(Color.Red);
            }
            else
            {
                string attackerName = attacker.Nickname;
                string attackerRole = attacker.Role.Type.ToString();
                string weapon = ev.DamageHandler.Type.ToString();
                embedBuilder.WithTitle("Player Kill")
                    .WithDescription($"**{attackerName}** ({attackerRole}) killed **{victimName}** ({victimRole}) with **{weapon}**.\nTime: {timestamp}")
                    .WithColor(Color.Orange);
            }

            if (Config.Debug)
                Log.Debug($"Sending kill log: {embedBuilder.Description}");

            try
            {
                var channel = _client.GetChannel(Config.DiscordChannelId) as IMessageChannel;
                if (channel != null)
                {
                    await channel.SendMessageAsync(embed: embedBuilder.Build());
                    if (Config.Debug)
                        Log.Debug("Kill log sent to Discord successfully.");
                }
                else
                {
                    Log.Error($"Failed to find Discord channel with ID: {Config.DiscordChannelId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending Discord message: {ex.Message}");
            }
        }

        private async void OnPlayerVerified(VerifiedEventArgs ev)
        {
            if (!Config.IsEnabled || !_isBotConnected)
                return;

            Player player = ev.Player;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Player Joined")
                .WithDescription($"**{player.Nickname}** ({player.Role.Type}) joined the server.\nSteamID: {player.UserId}\nTime: {timestamp}")
                .WithColor(Color.Green)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("KillLogsDiscordBot");

            if (Config.Debug)
                Log.Debug($"Player joined: {player.Nickname} ({player.UserId})");

            try
            {
                var channel = _client.GetChannel(Config.DiscordChannelId) as IMessageChannel;
                if (channel != null)
                {
                    await channel.SendMessageAsync(embed: embedBuilder.Build());
                    if (Config.Debug)
                        Log.Debug("Join log sent to Discord successfully.");
                }
                else
                {
                    Log.Error($"Failed to find Discord channel with ID: {Config.DiscordChannelId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending join log: {ex.Message}");
            }
        }

        private async void OnPlayerLeft(LeftEventArgs ev)
        {
            if (!Config.IsEnabled || !_isBotConnected)
                return;

            Player player = ev.Player;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Player Left")
                .WithDescription($"**{player.Nickname}** ({player.Role.Type}) left the server.\nSteamID: {player.UserId}\nTime: {timestamp}")
                .WithColor(Color.Red)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("KillLogsDiscordBot");

            if (Config.Debug)
                Log.Debug($"Player left: {player.Nickname} ({player.UserId})");

            try
            {
                var channel = _client.GetChannel(Config.DiscordChannelId) as IMessageChannel;
                if (channel != null)
                {
                    await channel.SendMessageAsync(embed: embedBuilder.Build());
                    if (Config.Debug)
                        Log.Debug("Leave log sent to Discord successfully.");
                }
                else
                {
                    Log.Error($"Failed to find Discord channel with ID: {Config.DiscordChannelId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending leave log: {ex.Message}");
            }
        }
    }
}