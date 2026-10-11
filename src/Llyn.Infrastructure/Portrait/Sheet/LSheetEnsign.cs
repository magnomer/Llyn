using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetEnsign
{
    public static IReadOnlyList<string> LSheetEnsignScan(LLiveryPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        List<string> found = [];
        if (page.LLiveryPageBanner.TryGetValue(page.LLiveryPageDraft.LEntryDraftLanguage, out string? own))
        {
            found.Add(own);
        }

        found.AddRange(page.LLiveryPageBanner.Values);
        found.AddRange(page.LLiveryPageEnsign.Values);

        List<string> paths = [];
        HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (string path in found)
        {
            if (seen.Add(path) && LPortraitAsset.LPortraitAssetLoad(path) is not null)
            {
                paths.Add(path);
            }
        }

        return paths;
    }

    public static string LSheetEnsignFormat(IReadOnlyList<string> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        StringBuilder rules = new StringBuilder();
        for (int index = 0; index < paths.Count; index++)
        {
            if (LPortraitAsset.LPortraitAssetLoad(paths[index]) is not LPortraitAsset asset)
            {
                continue;
            }

            rules.Append(".ensign-")
                .Append(index.ToString(CultureInfo.InvariantCulture))
                .Append("{background-image:url(\"")
                .Append(asset.LPortraitAssetAddress)
                .Append("\")}\n");
        }

        return rules.ToString();
    }

    public static string LSheetEnsignRead(IReadOnlyList<string> paths, string? path)
    {
        ArgumentNullException.ThrowIfNull(paths);

        for (int index = 0; index < paths.Count; index++)
        {
            if (string.Equals(paths[index], path, StringComparison.Ordinal))
            {
                return "ensign-" + index.ToString(CultureInfo.InvariantCulture);
            }
        }

        return string.Empty;
    }
}
