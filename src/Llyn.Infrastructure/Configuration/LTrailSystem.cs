using System;
using System.IO;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LTrailSystem : LTrail
{
    public string? LTrailResolve(string root, string path)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(path);

        try
        {
            if (Path.IsPathFullyQualified(path))
            {
                return Path.GetFullPath(path);
            }

            string anchored = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root))
                + Path.DirectorySeparatorChar;
            string full = Path.GetFullPath(Path.Combine(anchored, path));
            return full.StartsWith(anchored, StringComparison.OrdinalIgnoreCase) ? full : null;
        }
        catch (Exception exception) when (exception is ArgumentException or IOException)
        {
            return null;
        }
    }

    public string? LTrailRelativeResolve(string root, string path)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(path);

        string relative = Path.GetRelativePath(root, path);
        return Path.IsPathRooted(relative) || relative.StartsWith("..", StringComparison.Ordinal)
            ? null
            : relative;
    }

    public string LTrailNameRead(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        return Path.GetFileName(Path.TrimEndingDirectorySeparator(path));
    }

    public bool LTrailRootCheck(string path)
    {
        return Path.IsPathRooted(path);
    }

    public string LTrailNameNormalize(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        foreach (char barred in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(barred, '_');
        }

        return name;
    }
}
