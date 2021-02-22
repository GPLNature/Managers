using System;
using System.Collections.Generic;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordManager.Command;
using DiscordManager.Interfaces;
using LogManager;

namespace DiscordManager
{
  internal class BuildOption
  {
    public static readonly string Version = typeof(BuildOption).Assembly
      .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    public UserStatus BotStatus = UserStatus.Online;
    public BaseSocketClient? Client = null;
    public CommandConfig CommandConfig;
    public Game? Game;
    public GatewayIntents? Intents = null;
    public LogLevel LogLevel = LogLevel.INFO;
    public Func<SocketMessage, Permission, List<string>>? Permmission = null;
    public DiscordSocketConfig? SocketConfig;
    public ShardOption? ShardOption = null;
    public bool UseObjectService = false;
    public bool UseCommandModule = false;
  }

  internal struct ShardOption
  {
    public int[] ShardIds;
    public int Shards;
  }
}