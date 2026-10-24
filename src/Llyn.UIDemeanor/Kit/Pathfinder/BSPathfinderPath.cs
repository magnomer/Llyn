using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal static class BSPathfinderPath
{
    internal static string BSPathfinderPathResolve(string folder, string typed)
    {
        string[] shown = folder.Split(['\\', '/']);
        string root = folder is ['\\' or '/', '\\' or '/', ..] ? string.Join('\\', shown.Take(4))
            : shown[0].EndsWith(':') ? shown[0]
            : string.Empty;
        string joined = typed switch
        {
            ['\\' or '/', '\\' or '/', ..] or [_, ':', '\\' or '/', ..] => typed,
            ['\\' or '/', ..] => root + typed,
            [_, ':'] => typed,
            [_, ':', ..] => string.Concat(
                root.Equals(typed[..2], StringComparison.OrdinalIgnoreCase) ? folder : typed[..2],
                "\\",
                typed[2..]),
            _ => string.Concat(folder, "\\", typed),
        };
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
