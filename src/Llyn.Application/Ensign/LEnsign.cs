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

    public void LEnsignPathAdd(
        int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths,
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(paths);
        ArgumentNullException.ThrowIfNull(store);
        if (keys.Count != paths.Count)
        {
            throw new ArgumentException("Every key needs one path.", nameof(paths));
        }

        List<LEnsignRow> kept = [];
        lock (_lEnsignStore)
        {
            if (age != _lEnsignAge)
            {
                return;
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

        if (kept.Count == 0)
        {
            return;
        }

        Action commit = store(kept, LEnsignPathDelete);
        lock (_lEnsignStore)
        {
            if (age == _lEnsignAge)
            {
                commit();
            }
        }
    }

    public void LEnsignPathDelete(string path, Exception exception)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(exception);

        lock (_lEnsignStore)
        {
            foreach (string key in _lEnsignStore
                .Where(pair => string.Equals(pair.Value, path, StringComparison.Ordinal))
                .Select(static pair => pair.Key)
                .ToArray())
            {
                _lEnsignStore.Remove(key);
            }
        }

        if (_lEnsignUsher.LUsherLockCheck(exception))
        {
            return;
        }

        _lEnsignUsher.LUsherPathDelete(path);
    }
}
