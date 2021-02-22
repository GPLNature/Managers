using System;

namespace FileManager
{
  public class FileManageException : Exception
  {
    public FileManageException(string message) : base(message)
    {
    }

    public FileManageException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}