using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordCreator.Command;

namespace DiscordCreator.Interfaces
{
  public class CommandConfig
  {
    public Func<SocketMessage, CommandManager, Task>? CommandFunc = null;
    public string[] HelpArg = {"help"};
    public Func<SocketMessage, DiscordManager, string>? Permission = null;
    public string Prefix = "!";
  }
}