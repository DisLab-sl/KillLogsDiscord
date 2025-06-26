// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="DisLab">
// Copyright (c) MONCEF50G. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// SPDX-License-License-Identifier: CC-BY-SA-3.0

using Exiled.API.Interfaces;

namespace KillLogsDiscordBot
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string DiscordBotToken { get; set; } = "Token-Here";
        public ulong DiscordChannelId { get; set; } = Id channel here;
        public bool LogScpKills { get; set; } = false;
        public bool LogSuicides { get; set; } = true;
    }
}
