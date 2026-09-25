using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LEnsign
{
    private readonly Dictionary<string, string?> _lEnsignStore = new(StringComparer.Ordinal);

    private readonly LUsher _lEnsignUsher;

    private int _lEnsignAge;

    public LEnsign(LUsher usher)
    {
        ArgumentNullException.ThrowIfNull(usher);

        _lEnsignUsher = usher;
    }

    public static string LEnsignKeyFormat(string language, string variety)
    {
        return string.Concat(language, "/", variety);
    }

    public void LEnsignClear()
    {
        lock (_lEnsignStore)
        {
            _lEnsignAge++;
            _lEnsignStore.Clear();
        }
    }

    public string[] LEnsignMissingRead(IEnumerable<string> keys, out int age)
    {
        ArgumentNullException.ThrowIfNull(keys);

        lock (_lEnsignStore)
        {
            age = _lEnsignAge;
            return keys
                .Where(key => !_lEnsignStore.ContainsKey(key))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }
    }

    public IReadOnlyList<LEnsignRow> LEnsignPathAdd(
        int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths)
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(paths);

        List<LEnsignRow> kept = [];
        lock (_lEnsignStore)
        {
            if (age != _lEnsignAge)
            {
                return kept;
            }

            for (int index = 0; index < keys.Count; index++)
            {
                string? path = paths[index];
                bool present = _lEnsignUsher.LUsherPathExist(path);
                _lEnsignStore[keys[index]] = present ? path : null;
                if (present)
                {
                    kept.Add(new LEnsignRow(keys[index], path!));
                }
            }
        }

        return kept;
    }

    public void LEnsignPathDelete(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        _lEnsignUsher.LUsherPathDelete(path);
    }
}
