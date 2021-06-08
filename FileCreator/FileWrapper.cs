using System;

namespace FileCreator
{
  internal class FileWrapper
  {
    public readonly string FileName;
    public readonly string Path;
    public object File;
    public FileType FileType;
    public Type Type;

    public FileWrapper(string fileName, string path, object file, FileType fileType, Type type)
    {
      FileName = fileName;
      Path = path;
      File = file;
      FileType = fileType;
      Type = type;
    }
  }
}