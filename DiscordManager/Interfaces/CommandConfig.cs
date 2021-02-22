using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordManager.Command;

namespace DiscordManager.Interfaces
{
  public class CommandConfig
  {
    public Func<SocketMessage, CommandManager, Task>? CommandFunc = null;
    public string[] HelpArg = {"help"};
    public string Prefix = "!";
    public Func<SocketMessage, Permission, List<string>>? Permission = null;
  }
}