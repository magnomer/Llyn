using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Llyn.Application;

public static class LEnsign
{
    private static readonly Dictionary<string, string?> LEnsignStore = new(StringComparer.Ordinal);

    private static int _lEnsignAge;

    public static string LEnsignKeyFormat(string language, string variety)
    {
        return string.Concat(language, "/", variety);
    }

    public static void LEnsignClear()
    {
        lock (LEnsignStore)
        {
            _lEnsignAge++;
            LEnsignStore.Clear();
        }
    }

    public static string[] LEnsignMissingRead(IEnumerable<string> keys, out int age)
    {
        ArgumentNullException.ThrowIfNull(keys);

        lock (LEnsignStore)
        {
            age = _lEnsignAge;
            return keys
                .Where(key => !LEnsignStore.ContainsKey(key))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }
    }

    public static IReadOnlyList<LEnsignRow> LEnsignPathAdd(
        int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths)
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(paths);

        List<LEnsignRow> kept = [];
        lock (LEnsignStore)
        {
            if (age != _lEnsignAge)
            {
                return kept;
            }

            for (int index = 0; index < keys.Count; index++)
            {
                string? path = paths[index];
                bool present = path is not null && File.Exists(path);
                LEnsignStore[keys[index]] = present ? path : null;
                if (present)
                {
                    kept.Add(new LEnsignRow(keys[index], path!));
                }
            }
        }

        return kept;
    }

    public static string? LEnsignPathRead(string key)
    {
        lock (LEnsignStore)
        {
            return LEnsignStore.TryGetValue(key, out string? path) ? path : null;
        }
    }

    public static void LEnsignPathDelete(string path)
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
