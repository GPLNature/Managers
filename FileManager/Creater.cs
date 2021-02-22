using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace FileManager
{
  internal class Creater
  {
    private static object JsonLoad(string filePath, Type type)
    {
      File.ReadAllText(filePath, Encoding.UTF8);

      return JsonConvert.DeserializeObject(filePath, type);
    }

    private static object JsonSave(string filePath, object data, Type type)
    {
      var contents = JsonConvert.SerializeObject(data);
      File.WriteAllText(filePath, contents);
      return JsonConvert.DeserializeObject(contents, type);
    }

    private static string FilePath(string path, string fileName, FileType type)
    {
      return $"{path}/{fileName}.{type}";
    }

    public static FileWrapper Loader(string path, Type type)
    {
      if (type.GetCustomAttribute(typeof(FileManage)) is FileManage manage)
      {
        var filePath = FilePath(path, manage.FileName, manage.FileType);
        try
        {
          var fileClass = manage.FileType switch
          {
            FileType.Json => JsonLoad(filePath, type)
          };

          return new FileWrapper(type.Name, path, fileClass, manage.FileType, type);
        }
        catch (FileNotFoundException)
        {
          var createdClass = manage.FileType switch
          {
            FileType.Json => JsonSave(filePath, Activator.CreateInstance(type), type)
          };
          
          return new FileWrapper(type.Name, path, createdClass, manage.FileType, type);
        }
      }

      return default;
    }

    public static void Save(FileWrapper wrapper)
    {
      switch (wrapper.FileType)
      {
        case FileType.Json:
          JsonSave(FilePath(wrapper.Path, wrapper.FileName, wrapper.FileType), wrapper.File, wrapper.Type);
          break;
      }
    }
  }
}