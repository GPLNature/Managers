using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordManager.Interfaces;
using LogCreator;

namespace DiscordManager.Command
{
  public class CommandManager
  {
    private readonly Regex _contentRegex = new(@"(""[^""]+""|[^\s""]+)");
    private readonly Func<SocketMessage, DiscordManager, string>? _customPermission;
    private readonly string[] _helpArg;
    private readonly Logger _logger;
    private readonly DiscordManager _manager;
    private readonly string _prefix;
    private IReadOnlyDictionary<Context, IReadOnlyCollection<CommandWrapper>> _commands;
    private IReadOnlyDictionary<string, MethodInfo> _helpCommands;

    internal CommandManager(DiscordManager manager, CommandConfig commandConfig)
    {
      _manager = manager;
      _logger = _manager.LogManager.CreateLogger("Command Manager (CM)");
      _customPermission = commandConfig.Permission;
      _helpArg = commandConfig.HelpArg;
      _prefix = commandConfig.Prefix;

      var client = manager.GetClient();
      if (commandConfig.CommandFunc != null)
        client.MessageReceived += async arg => { await commandConfig.CommandFunc.Invoke(arg, this); };
      else
        client.MessageReceived += ClientOnMessageReceived;
    }

    private async Task ClientOnMessageReceived(SocketMessage arg)
    {
      if (arg.Author.IsBot || arg.Author.IsWebhook)
        return;

      var content = arg.Content.Trim();

      if (content.StartsWith(_prefix))
      {
        var arguments = content.Split(' ');

        var commandName = arguments.ElementAtOrDefault(1);

        if (commandName != null)
          ExecuteCommand(arg, commandName);
      }
    }

    private KeyValuePair<Context, CommandWrapper>? GetCommand(string commandName)
    {
      if (!_commands.Any())
        return null;

      for (var i = 0; i < _commands.Count; i++)
      {
        var (key, value) = _commands.ElementAt(i);
        for (var j = 0; j < value.Count; j++)
        {
          var commandWrapper = value.ElementAt(j);
          if (commandWrapper.Contains(commandName))
            return KeyValuePair.Create(key, commandWrapper);
        }
      }

      return null;
    }

    internal void LoadCommands()
    {
      var commandLogger = _manager.LogManager.CreateLogger("Command");
      var assembly = AppDomain.CurrentDomain.GetAssemblies();
      var types = assembly.Where(s => s.EntryPoint is not null).SelectMany(s => s.GetTypes())
        .Where(p => !p.IsAbstract && p.IsClass && typeof(Context).IsAssignableFrom(p))
        .ToList();
      var commands = new Dictionary<Context, IReadOnlyCollection<CommandWrapper>>();
      var helpCommands = new Dictionary<string, MethodInfo>();
      for (var i = 0; i < types.Count; i++)
      {
        var type = types[i];
        var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
        var nameList = new List<string[]>();
        var list = new List<CommandWrapper>();
        for (var j = 0; j < methods.Length; j++)
        {
          var method = methods[j];
          if (!method.IsPublic)
            continue;
          if (Attribute.GetCustomAttribute(method, typeof(NotMapping), true) is NotMapping)
            continue;

          if (Attribute.GetCustomAttribute(method, typeof(HelpMethod), true) is HelpMethod helpMethod)
          {
            try
            {
              if (!nameList.Any(names => names.Contains(helpMethod.TargetMethod)))
                helpCommands.Add(helpMethod.TargetMethod, method);
            }
            catch (Exception e)
            {
              _logger.CriticalAsync("CM(Command Manager) Error", e);
              throw;
            }

            continue;
          }

          if (!(Attribute.GetCustomAttribute(method, typeof(CommandName), true) is CommandName commandName))
            throw new ManagerException($"{method.Name} doesn't have CommandName Attribute");

          if (commandName.Names.Any(name => nameList.Any(names => names.Contains(name))))
            throw new ManagerException($"{method.Name} has Overlap CommandName");

          var commandGroup = Attribute.GetCustomAttribute(method, typeof(CommandGroup), true) as CommandGroup;
          var botPermission =
            Attribute.GetCustomAttribute(method, typeof(RequireBotPermission), true) as RequireBotPermission;
          var usage =
            ((CommandUsage) Attribute.GetCustomAttribute(method, typeof(CommandUsage), true))?.Usage ?? Usage.ALL;

          nameList.Add(commandName.Names);
          list.Add(new CommandWrapper(commandName, usage, botPermission, method, commandGroup));
        }

        var construct = (Context) Activator.CreateInstance(type);
        construct.Initialize(_manager, commandLogger);
        commands.Add(construct, list);
      }

      _helpCommands = helpCommands;
      _commands = commands;
    }

    public async void ExecuteCommand(SocketMessage message, string commandName)
    {
      var task = new Task(async () =>
      {
        var valuePair = GetCommand(commandName);
        if (valuePair == null)
          return;
        var channel = message.Channel;
        var baseClass = valuePair.Value.Key;
        var command = valuePair.Value.Value;
        switch (command.Usage)
        {
          case Usage.Guild:
            if (!(channel is SocketGuildChannel))
              return;
            break;
          case Usage.DM:
            if (!(channel is SocketDMChannel))
              return;
            break;
          case Usage.ALL:
            break;
        }

        var matchesCollection = _contentRegex.Matches(message.Content);
        var matches = matchesCollection.Select(mts => mts.Value.Replace("\"", "").Trim()).Skip(2).ToArray();
        var service = command.MethodInfo;
        var perm = command.BotPermission;
        if (channel is SocketGuildChannel guildChannel && perm != null)
        {
          var missingPerms = command.CheckPermissions(guildChannel.Guild.CurrentUser);
          if (missingPerms != null)
          {
            var missingP = string.Join(", ", missingPerms);
            await channel.SendMessageAsync(
              $"Missing Bot Permission : {missingP}\nPlease Add Missing Permissions");
            return;
          }
        }

        if (matches.Length != 0 && _helpArg.Contains(matches[0]))
          if (_helpCommands.ContainsKey(command.CommandName[0]))
            service = _helpCommands[command.CommandName[0]];
        baseClass.SetMessage(message);
        var parameters = service.GetParameters();
        object?[]? param = null;
        if (parameters.Length != 0)
        {
          param = new object[parameters.Length];
          if (matches.Length != 0)
            for (var i = 0; i < parameters.Length; i++)
            {
              var parameter = parameters[i];
              var parameterType = parameter.ParameterType;
              var count = i + 1;
              if (parameterType.IsArray && count == parameters.Length)
              {
                var elementType = parameterType.GetElementType();
                if (elementType == null)
                  continue;

                bool Predicate(string item)
                {
                  try
                  {
                    _ = Convert.ChangeType(item, elementType);
                    return true;
                  }
                  catch (Exception)
                  {
                    return false;
                  }
                }

                var paramArray = matches.Skip(i)
                  .Where(Predicate)
                  .Select(item => Convert.ChangeType(item, elementType)).ToArray();
                var destinationArray = Array.CreateInstance(elementType, paramArray.Length);
                Array.Copy(paramArray, destinationArray, paramArray.Length);
                param[i] = destinationArray;
              }
              else
              {
                object? converted = null;
                if (matches.Length > i)
                {
                  var content = matches[i];
                  try
                  {
                    if (parameterType == typeof(string[]))
                      converted = matches;
                    else if (parameterType.IsEnum)
                      converted = Enum.Parse(parameterType, content);
                    else
                      converted = parameterType == typeof(string)
                        ? content
                        : Convert.ChangeType(content, parameterType);
                  }
                  catch (Exception)
                  {
                    if (parameter.HasDefaultValue)
                      converted = parameter.DefaultValue;
                  }
                }

                param[i] = converted;
              }
            }
        }

        service.Invoke(baseClass, param);
        await _logger.InfoAsync($"Command Method Execute : {service.Name}").ConfigureAwait(false);
      });
      try
      {
        task.Start();
        task.Wait();
      }
      catch
      {
        await _logger.DebugAsync("Error At Executing Command", task.Exception).ConfigureAwait(false);
      }
    }
  }
}