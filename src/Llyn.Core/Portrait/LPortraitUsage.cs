using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitUsage(
    string LPortraitUsageHeadword,
    string LPortraitUsageTitle,
    string LPortraitUsageOwner,
    string LPortraitUsageLanguage)
{
    public static IReadOnlyList<LPortraitUsage> LPortraitUsageCreate(
        IReadOnlyList<LUsage> incoming, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(incoming);
        ArgumentNullException.ThrowIfNull(label);

        List<LPortraitUsage> shown = new List<LPortraitUsage>();

        foreach (LUsage usage in incoming)
        {
            shown.Add(new LPortraitUsage(
                usage.LUsageHeadword,
                LPortraitText.LPortraitTextRead(usage.LUsageTitle, label.LPortraitLabelUnknown),
                usage.LUsageOwner == LOwner.LOwnerCollocation
                    ? label.LPortraitLabelCollocation
                    : label.LPortraitLabelMeaning,
                usage.LUsageLanguage));
        }

        return shown;
    }
}
