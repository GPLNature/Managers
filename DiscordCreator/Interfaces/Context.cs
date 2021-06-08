using System;
using Discord;
using Discord.WebSocket;
using LogCreator;

namespace DiscordCreator.Interfaces
{
  public class Context
  {
    protected readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(3);
    protected DiscordManager Manager { get; private set; }

    protected Logger Logger { get; private set; }
    protected BaseSocketClient Client => Manager.GetClient();

    protected SocketMessage Message { get; private set; }


    protected SocketSelfUser Self => Client.CurrentUser;

    /// <summary>
    ///   Get Message Author from Message
    /// </summary>
    protected SocketUser Author => Message.Author;

    /// <summary>
    ///   Get Message Channel from Message
    /// </summary>
    protected ISocketMessageChannel Channel => Message.Channel;

    /// <summary>
    ///   If message from guild Not Null
    ///   Opposition is guild not null
    /// </summary>
    protected SocketGuild? Guild => (Channel as SocketGuildChannel)?.Guild;

    internal void Initialize(DiscordManager manager, Logger logger)
    {
      Manager = manager;
      Logger = logger;

      Init();
    }

    internal void SetMessage(SocketMessage message)
    {
      Message = message;
    }

    internal IUser GetAuthor()
    {
      return Author;
    }

    protected virtual void Init()
    {
      // Empty
    }
  }
}