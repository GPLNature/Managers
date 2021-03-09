using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogManager;

namespace DiscordManager.Service
{
  internal class ObjectService
  {
    private readonly Logger _logger;
    private readonly Dictionary<string, object> _objects;

    public ObjectService(DiscordManager _manager)
    {
      _objects = new Dictionary<string, object>();
      _logger = _manager.LogManager.CreateLogger("Object Service (OS)");
    }

    public object this[string objectName] => _objects.GetValueOrDefault(objectName);

    public T Get<T>()
    {
      return (T) _objects.GetValueOrDefault(typeof(T).Name);
    }

    private bool CheckDuplicate(string name)
    {
      return _objects.ContainsKey(name);
    }

    public void Add(object? obj)
    {
      if (obj == null)
      {
        _logger.ErrorAsync("Object must be not null", new ArgumentNullException());
        return;
      }

      var type = obj.GetType();
      if (CheckDuplicate(type.Name))
      {
        _logger.ErrorAsync($"{type.Name} is overlap.");
        return;
      }

      _objects.Add(type.Name, obj);
    }

    public void Add<T>()
    {
      Add<T>(null);
    }

    public void Add<T>(object[]? args)
    {
      var type = typeof(T);
      if (CheckDuplicate(type.Name))
      {
        _logger.ErrorAsync($"{type.Name} is overlap.");
        return;
      }

      try
      {
        var info = args == null
          ? type.GetConstructor(Type.EmptyTypes)
          : type.GetConstructor(args.Select(o => o.GetType()).ToArray());
        
        var instance = info.Invoke(args);
        _objects.Add(type.Name, instance);
      }
      catch (Exception e)
      {
        _logger.CriticalAsync(e.StackTrace, e);
        throw;
      }
    }

    public void Remove<T>()
    {
      _objects.Remove(typeof(T).Name);
    }

    public void Remove(object obj)
    {
      _objects.Remove(obj.GetType().Name);
    }
  }
}