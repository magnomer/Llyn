using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal static class BSPathfinderEntries
{
    internal static IReadOnlyList<string>? BSPathfinderEntriesRead(
        Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)> listing,
        IReadOnlyList<string>? suffixes,
        BSPathfinderMedium? medium,
        string folder,
        string segment)
    {
        IReadOnlyList<string> folders;
        IReadOnlyList<string> files;
        try
        {
            (folders, files) = listing(folder);
        }
        catch (Exception)
        {
            return null;
        }

        Func<string, string> name = entry => entry[(entry.LastIndexOfAny(['\\', '/']) + 1)..];
        StringComparer order = StringComparer.OrdinalIgnoreCase;
        IReadOnlyList<string>? asked = medium is not null
            ? [medium.BSPathfinderMediumChoice.CPortraitChoiceSuffix]
            : suffixes;
        IEnumerable<string> shown = asked is IReadOnlyList<string> wanted
            ? files.Select(name)
                .Where(file => wanted.Any(suffix => file.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                .Order(order)
            : [];
        IEnumerable<string> entries =
            ["..\\", .. folders.Select(name).Order(order).Select(static entry => entry + "\\"), .. shown];
        return [.. entries.Where(entry => entry.StartsWith(segment, StringComparison.OrdinalIgnoreCase))];
    }
}
