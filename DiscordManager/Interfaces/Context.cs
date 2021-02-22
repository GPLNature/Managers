using System;
using Discord.WebSocket;
using LogManager;

namespace DiscordManager.Interfaces
{
  public class Context
  {
    protected readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(3);
    protected DiscordManager Manager { get; private set; }

    internal DiscordManager _Manager
    {
      set
      {
        Manager = value;
        Logger = Manager.LogManager.CreateLogger("Context");
      }
    }

    protected Logger Logger { get; private set; }
    protected BaseSocketClient Client => Manager.GetClient();

    protected SocketMessage Message { get; private set; }

    internal SocketMessage _message
    {
      set => Message = value;
    }

    protected SocketSelfUser Self => Client.CurrentUser;

    /// <summary>
    ///   Get Message Author from Message
    /// </summary>
    public SocketUser Author => Message.Author;

    /// <summary>
    ///   Get Message Channel from Message
    /// </summary>
    protected ISocketMessageChannel Channel => Message.Channel;

    /// <summary>
    ///   If message from guild Not Null
    ///   Opposition is guild not null
    /// </summary>
    protected SocketGuild? Guild => (Channel as SocketGuildChannel)?.Guild;
  }
}