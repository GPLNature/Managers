using System;
using System.Threading.Tasks;

namespace LogCreator
{
  public class LogManager
  {
    public Func<LogObject, Task> LoggerFunc;

    public LogManager(LogLevel logLevel)
    {
      LogLevel = logLevel;
    }

    public LogLevel LogLevel { get; }

    private async Task LogAsync(LogLevel logLevel, string source, string message, Exception exception = null)
    {
      try
      {
        if (logLevel <= LogLevel)
          await LoggerFunc.Invoke(new LogObject(logLevel, source, message, exception)).ConfigureAwait(false);
      }
      catch
      {
        // ignored
      }
    }

    private async Task LogAsync(LogLevel logLevel, string source, Exception exception)
    {
      try
      {
        if (logLevel <= LogLevel)
          await LoggerFunc.Invoke(new LogObject(logLevel, source, null, exception)).ConfigureAwait(false);
      }
      catch
      {
        // ignored
      }
    }

    public Task InfoAsync(string source, Exception exception)
    {
      return LogAsync(LogLevel.INFO, source, exception);
    }

    public Task InfoAsync(string source, string message, Exception exception = null)
    {
      return LogAsync(LogLevel.INFO, source, message, exception);
    }

    public Task ErrorAsync(string source, string message, Exception exception = null)
    {
      return LogAsync(LogLevel.ERROR, source, message, exception);
    }

    public Task DebugAsync(string source, string message, Exception exception = null)
    {
      return LogAsync(LogLevel.DEBUG, source, message, exception);
    }

    public Task CriticalAsync(string source, string message, Exception exception = null)
    {
      return LogAsync(LogLevel.CRITICAL, source, message, exception);
    }

    public Logger CreateLogger(string name)
    {
      return new(this, name);
    }
  }
}