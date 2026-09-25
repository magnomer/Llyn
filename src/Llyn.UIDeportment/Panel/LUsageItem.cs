using System;
using System.Collections.Generic;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LUsageItem
{
    public LUsageItem(LUsage usage, string owner, string unknown, string unnamed, string epithet = "")
    {
        LUsageItemId = usage.LUsageId;
        LUsageItemEntry = usage.LUsageEntry;
        LUsageItemName = usage.LUsageName;
        LUsageItemEpithet = epithet ?? string.Empty;
        LUsageItemLanguage = usage.LUsageLanguage;
        LUsageItemOwner = owner;
        LUsageItemQuoted = usage.LUsageQuoted;
        LUsageItemTitle = usage.LUsageTitle.LStateValueUncertain
            ? unknown
            : usage.LUsageTitle.LStateValueShown ?? unnamed;
        LUsageItemFlag = LEnsignImage.LEnsignFind(usage.LUsageLanguage);
    }

    public long LUsageItemId { get; }

    public long LUsageItemEntry { get; }

    public string LUsageItemEpithet { get; }

    public string LUsageItemName { get; }

    public string LUsageItemLanguage { get; }

    public string LUsageItemOwner { get; }

    public bool LUsageItemQuoted { get; }

    public string LUsageItemTitle { get; }

    public ImageSource? LUsageItemFlag { get; }

    public static IReadOnlyList<LUsageItem> LUsageItemBuild(IReadOnlyList<LUsage> usages)
    {
        ArgumentNullException.ThrowIfNull(usages);

        string unknown = LLocalizationCatalog.LLocalizationTextRead("Display.Unknown");
        string meaning = LLocalizationCatalog.LLocalizationTextRead("Display.MeaningSingle");
        string collocation = LLocalizationCatalog.LLocalizationTextRead("Display.CollocationSingle");
        string example = LLocalizationCatalog.LLocalizationTextRead("Vita.Example");

        List<LUsageItem> built = new(usages.Count);
        foreach (LUsage usage in usages)
        {
            string owner = usage.LUsageCollocated ? collocation : usage.LUsageQuoted ? example : meaning;
            built.Add(new LUsageItem(usage, owner, unknown, string.Empty, usage.LUsageEpithet));
        }

        return built;
    }

    public void LUsageItemShow(Func<long, bool> exampleSeam, Func<long, bool> entrySeam)
    {
        ArgumentNullException.ThrowIfNull(exampleSeam);
        ArgumentNullException.ThrowIfNull(entrySeam);

        if (LUsageItemQuoted)
        {
            exampleSeam(LUsageItemId);
            return;
        }

        entrySeam(LUsageItemEntry);
    }
}
