using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal static class BSPathfinderPath
{
    internal static string BSPathfinderPathResolve(string folder, string typed)
    {
        bool rooted = typed.StartsWith('\\') || typed.StartsWith('/') || (typed.Length > 1 && typed[1] == ':');
        string joined = rooted ? typed : string.Concat(folder, "\\", typed);
        string[] parts = joined.Split(['\\', '/']);
        int heads = joined.Length > 1 && joined[0] is '\\' or '/' && joined[1] is '\\' or '/' ? 4 : 1;
        List<string> kept = [.. parts.Take(heads)];
        foreach (string part in parts.Skip(heads))
        {
            if (part == "..")
            {
                if (kept.Count > heads)
                {
                    kept.RemoveAt(kept.Count - 1);
                }
            }
            else if (part is not ("" or "."))
            {
                kept.Add(part);
            }
        }

        string resolved = string.Join('\\', kept);
        return kept.Count <= heads ? resolved + "\\" : resolved;
    }
}
