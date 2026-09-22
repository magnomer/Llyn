using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEpoch(string LEpochLabel, string LEpochCode)
{
    public static (string LEpochFound, string LEpochCaption) LEpochResolve(
        IReadOnlyList<LEpoch> epochs, string caption)
    {
        ArgumentNullException.ThrowIfNull(epochs);
        ArgumentNullException.ThrowIfNull(caption);

        string label = string.Empty;
        string code = string.Empty;
        foreach (LEpoch epoch in epochs)
        {
            if (epoch.LEpochLabel.Length > label.Length && LEpochPrefixCheck(caption, epoch.LEpochLabel))
            {
                label = epoch.LEpochLabel;
                code = epoch.LEpochCode;
            }
        }

        return label.Length == 0 ? (string.Empty, caption) : (code, caption[label.Length..].TrimStart());
    }

    private static bool LEpochPrefixCheck(string caption, string label)
    {
        return label.Length > 0
            && caption.StartsWith(label, StringComparison.Ordinal)
            && (caption.Length == label.Length || char.IsWhiteSpace(caption[label.Length]));
    }
}
