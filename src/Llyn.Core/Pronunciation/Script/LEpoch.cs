using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEpoch(string LEpochLabel, string LEpochCode)
{
    private const char LEpochRange = '或';

    public static (string LEpochFound, string LEpochCaption) LEpochResolve(
        IReadOnlyList<LEpoch> epochs, string caption)
    {
        ArgumentNullException.ThrowIfNull(epochs);
        ArgumentNullException.ThrowIfNull(caption);

        string[] words = caption.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int place = -1;
        int width = 0;
        string code = string.Empty;
        for (int index = 0; index < words.Length; index++)
        {
            foreach (LEpoch epoch in epochs)
            {
                if (epoch.LEpochLabel.Length > width && LEpochWordCheck(words[index], epoch.LEpochLabel))
                {
                    place = index;
                    width = epoch.LEpochLabel.Length;
                    code = epoch.LEpochCode;
                }
            }
        }

        if (place < 0)
        {
            return (string.Empty, caption);
        }

        List<string> rest = new(words.Length - 1);
        for (int index = 0; index < words.Length; index++)
        {
            if (index != place)
            {
                rest.Add(words[index]);
            }
        }

        return (code, string.Join(' ', rest));
    }

    private static bool LEpochWordCheck(string word, string label)
    {
        return label.Length > 0
            && word.StartsWith(label, StringComparison.Ordinal)
            && (word.Length == label.Length || word[label.Length] == LEpochRange);
    }
}
