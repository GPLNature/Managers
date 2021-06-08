using System;

namespace FileCreator
{
  [AttributeUsage(AttributeTargets.Class)]
  public class FileManage : Attribute
  {
    internal readonly string FileName;
    internal FileType FileType;

    public FileManage(string fileName, FileType fileType)
    {
      FileName = fileName;
      FileType = fileType;
    }
  }
}