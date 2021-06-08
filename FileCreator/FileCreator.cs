using System;
using System.Collections.Generic;
using System.Reflection;

namespace FileCreator
{
  public class FileManager
  {
    private readonly Dictionary<string, FileWrapper> _loadedFile;

    public FileManager()
    {
      _loadedFile = new Dictionary<string, FileWrapper>();
    }

    private FileWrapper GetFile(MemberInfo type)
    {
      return _loadedFile.TryGetValue(type.Name, out var file) ? file : default;
    }

    private object FileLoad(string path, Type type, bool forceReload = false)
    {
      if (_loadedFile.ContainsKey(type.Name) && forceReload)
        return _loadedFile[type.Name].File;
      var fileWrapper = Creater.Loader(path, type);

      _loadedFile.TryAdd(type.Name, fileWrapper);
      return fileWrapper.File;
    }

    public void Init(string path, params Type[] types)
    {
      foreach (var type in types) FileLoad(path, type);
    }

    public T Get<T>()
    {
      return (T) GetFile(typeof(T)).File;
    }

    public T Load<T>(string path)
    {
      var file = (T) FileLoad(path, typeof(T));

      return file;
    }

    public T Reload<T>()
    {
      var type = typeof(T);
      var file = GetFile(type);
      return (T) FileLoad(file.Path, type, true);
    }

    public void Save<T>()
    {
      if (_loadedFile.TryGetValue(typeof(T).Name, out var file)) Creater.Save(file);
    }

    public void UnLoad<T>(bool save = true)
    {
      var name = typeof(T).Name;
      if (_loadedFile.TryGetValue(name, out var file) && save) Creater.Save(file);

      _loadedFile.Remove(name);
    }
  }
}