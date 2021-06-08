using Discord;
using Discord.WebSocket;
using DiscordCreator.Interfaces;
using LogCreator;

namespace DiscordCreator
{
  public class DiscordBuilder
  {
    internal readonly BuildOption Option;

    public DiscordBuilder()
    {
      Option = new BuildOption();
    }

    /// <summary>
    ///   Use the Command Service in DiscordCreator
    /// </summary>
    public DiscordBuilder WithCommandModule(CommandConfig? config = null)
    {
      Option.CommandConfig = config ?? new CommandConfig();
      return this;
    }

    /// <summary>
    ///   Set GatewayIntents
    /// </summary>
    public DiscordBuilder WithGatewayIntents(GatewayIntents intents)
    {
      Option.Intents = intents;
      return this;
    }

    /// <summary>
    ///   Set LogLevel(NONE,INFO, ERROR, CRITICAL,DEBUG, ALL)
    /// </summary>
    /// <param name="level">Log Level for DiscordCreator</param>
    public DiscordBuilder WithLogLevel(LogLevel level)
    {
      Option.LogLevel = level;
      return this;
    }

    /// <summary>
    ///   Can set the Status for the bot
    ///   Default Setting is <strong>Online</strong>
    ///   <example>
    ///     For Example
    ///     <code>
    ///    Builder.WithStatus(UserStatus.Online);
    /// </code>
    ///   </example>
    /// </summary>
    /// <param name="status">Status Type</param>
    public DiscordBuilder WithStatus(UserStatus status)
    {
      Option.BotStatus = status;
      return this;
    }

    /// <summary>
    ///   Can set the status message for the bot
    ///   <example>
    ///     For Example
    ///     <code>
    ///    Builder.WithActivity(new Game("Test!!"));
    /// </code>
    ///   </example>
    /// </summary>
    /// <param name="game">Status Message</param>
    public DiscordBuilder WithActivity(Game game)
    {
      Option.Game = game;
      return this;
    }

    /// <summary>
    ///   Can set the DiscordSocketConfig for the bot
    ///   <example>
    ///     For Example
    ///     <code>
    ///    Builder.UseSocketConfig(new DiscordSocketConfig {MessageCacheSize = 100});
    /// </code>
    ///   </example>
    /// </summary>
    /// <param name="config">Discord Socket Config</param>
    public DiscordBuilder UseSocketConfig(DiscordSocketConfig config)
    {
      Option.SocketConfig = config;
      return this;
    }

    /// <summary>
    ///   Can set the DiscordClient for the bot
    ///   <example>
    ///     For Example
    ///     <code>
    ///    Builder.SetClient(new DiscordSocketClient());
    /// </code>
    ///   </example>
    /// </summary>
    /// <param name="client">Discord Client</param>
    public DiscordBuilder SetClient(BaseSocketClient client)
    {
      Option.Client = client;
      return this;
    }

    public DiscordBuilder WithShard(int[] shardIds, int shards = 2)
    {
      Option.ShardOption = new ShardOption {Shards = shards, ShardIds = shardIds};
      return this;
    }

    /// <summary>
    ///   Builder to Discord Manager
    /// </summary>
    public DiscordManager Build()
    {
      return new(Option);
    }
  }
}