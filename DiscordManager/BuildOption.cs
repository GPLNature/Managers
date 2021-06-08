using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordManager.Interfaces;
using LogCreator;

namespace DiscordManager
{
  internal class BuildOption
  {
    public static readonly string Version = typeof(BuildOption).Assembly
      .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    public UserStatus BotStatus = UserStatus.Online;
    public BaseSocketClient? Client = null;
    public CommandConfig? CommandConfig;
    public Game? Game;
    public GatewayIntents? Intents = null;
    public LogLevel LogLevel = LogLevel.INFO;
    public ShardOption? ShardOption = null;
    public DiscordSocketConfig? SocketConfig;
  }

  internal struct ShardOption
  {
    public int[] ShardIds;
    public int Shards;
  }
}