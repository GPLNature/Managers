﻿using System;
using System.Threading.Tasks;
using LogCreator;

namespace DiscordCreator.Event
{
  public abstract class Events
  {
    internal readonly Logger _clientLogger;

    internal readonly Event<Func<LogObject, Task>> _log = new();
    internal readonly LogManager LogManager;

    internal Events(LogLevel level)
    {
      LogManager = new LogManager(level)
      {
        LoggerFunc = async msg => await _log.Invoke(msg).ConfigureAwait(false)
      };
      _clientLogger = LogManager.CreateLogger("Discord Manager (DM)");
    }

    public event Func<LogObject, Task> Log
    {
      add => _log.Add(value);
      remove => _log.Remove(value);
    }
  }
}