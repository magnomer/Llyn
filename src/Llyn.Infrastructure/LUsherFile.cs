using System;
using System.IO;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LUsherFile : LUsher
{
    public bool LUsherPathExist(string? path)
    {
        return path is not null && File.Exists(path);
    }

    public void LUsherPathDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public bool LUsherLockCheck(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is FileNotFoundException or DirectoryNotFoundException)
            {
                return false;
            }

            if (current is IOException or UnauthorizedAccessException)
            {
                return true;
            }
        }

        return false;
    }

    public void LUsherOpen(string target)
    {
        throw new NotSupportedException("A file usher opens no folder or address.");
    }
}
