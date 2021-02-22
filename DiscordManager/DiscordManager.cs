using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordManager.Command;
using DiscordManager.Event;
using DiscordManager.Service;
using LogManager;

namespace DiscordManager
{
  /// <summary>
  ///   It's DiscordManager Core
  /// </summary>
  public class DiscordManager : Events
  {
    private readonly BaseSocketClient _client;
    private readonly CommandManager _commandManager;
    private readonly ObjectService _objectService;

    internal DiscordManager(BuildOption option) : base(option.LogLevel)
    {
      if (option.Client == null)
      {
        var socketConfig = option.SocketConfig ?? new DiscordSocketConfig
        {
          MessageCacheSize = 100, TotalShards = option.ShardOption?.Shards,
          GatewayIntents = option.Intents
        };
        if (option.ShardOption.HasValue)
          _client = new DiscordShardedClient(option.ShardOption?.ShardIds, socketConfig);
        else
          _client = new DiscordSocketClient(socketConfig);
      }
      else
      {
        _client = option.Client;
      }

      _client.SetStatusAsync(option.BotStatus).ConfigureAwait(false);

      if (option.UseObjectService)
        _objectService = new ObjectService(this);

      if (option.UseCommandModule)
      {
        _clientLogger.DebugAsync("Load CommandModules...");
        _commandManager = new CommandManager(this, option.CommandConfig);
        _commandManager.LoadCommands();
      }
    }

    private async Task Init(string token, TokenType type)
    {
      await _clientLogger.InfoAsync("Discord Manager Initialize....").ConfigureAwait(false);
      await _clientLogger.DebugAsync("Check Internet is Available").ConfigureAwait(false);
      if (!NetworkInterface.GetIsNetworkAvailable())
        throw new ManagerException(
          "UnAvailable Internet Check Your Pc/Server Internet State");

      await _clientLogger.DebugAsync("Check Token is Validated").ConfigureAwait(false);
      try
      {
        TokenUtils.ValidateToken(type, token);
      }
      catch (Exception e)
      {
        throw new ManagerException(
          "Token is Invalid. The token must be Validated", e);
      }

      await _clientLogger.DebugAsync("Successfully Check Token").ConfigureAwait(false);
      await _clientLogger.DebugAsync("Register Events...").ConfigureAwait(false);
      RegisterEvents();

      await _clientLogger.DebugAsync("Successfully Register Events").ConfigureAwait(false);
      await _client.LoginAsync(type, token).ConfigureAwait(false);
      await _client.StartAsync().ConfigureAwait(false);

      await Task.Delay(-1);
    }

    private void RegisterEvents()
    {
      _client.Log += message =>
        _log.Invoke(new LogObject(LogLevel.INFO, message.Source, message.Message, message.Exception));
      _client.LoggedIn += async () =>
      {
        await _clientLogger.InfoAsync($"Login to {_client.GetCurrentUser().GetFullName()}").ConfigureAwait(false);
      };
    }

    public BaseSocketClient GetClient()
    {
      return _client;
    }

    public T GetClient<T>() where T : BaseSocketClient, IDiscordClient, IDisposable
    {
      return (T) _client;
    }

    public void AddObject(object obj)
    {
      _objectService.Add(obj);
    }

    public void AddObject<T>(params object[] obj)
    {
      _objectService.Add<T>(obj);
    }

    public T GetObject<T>()
    {
      return _objectService.Get<T>();
    }

    public void RemoveObject<T>()
    {
      _objectService.Remove<T>();
    }

    public void RemoveObject(object obj)
    {
      _objectService.Remove(obj);
    }

    /// <summary>
    /// Discord Manager Run with Token
    /// </summary>
    /// <param name="token"></param>
    /// <param name="type"></param>
    public void Run(string token, TokenType type = TokenType.Bot)
    {
      try
      {
        Init(token, type).GetAwaiter().GetResult();
      }
      catch (ManagerException e)
      {
        _clientLogger.CriticalAsync(e.Message, e).ConfigureAwait(false);
      }
    }
  }
}