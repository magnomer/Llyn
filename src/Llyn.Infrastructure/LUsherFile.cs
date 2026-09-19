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
}
