using System;
using System.Collections.Generic;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PUsageItem
{
    internal PUsageItem(LUsage usage, string owner, string unknown, string unnamed, string epithet = "")
    {
        PUsageItemId = usage.LUsageId;
        PUsageItemEntry = usage.LUsageEntry;
        PUsageItemName = usage.LUsageName;
        PUsageItemEpithet = epithet ?? string.Empty;
        PUsageItemLanguage = usage.LUsageLanguage;
        PUsageItemOwner = owner;
        PUsageItemQuoted = usage.LUsageQuoted;
        PUsageItemTitle = usage.LUsageTitle.LStateValueUncertain
            ? unknown
            : usage.LUsageTitle.LStateValueShown ?? unnamed;
        PUsageItemFlag = PEnsign.PEnsignFind(usage.LUsageLanguage);
    }

    public long PUsageItemId { get; }

    public long PUsageItemEntry { get; }

    public string PUsageItemEpithet { get; }

    public string PUsageItemName { get; }

    public string PUsageItemLanguage { get; }

    public string PUsageItemOwner { get; }

    public bool PUsageItemQuoted { get; }

    public string PUsageItemTitle { get; }

    public ImageSource? PUsageItemFlag { get; }

    internal static IReadOnlyList<PUsageItem> PUsageItemBuild(IReadOnlyList<LUsage> usages)
    {
        ArgumentNullException.ThrowIfNull(usages);

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string meaning = PLocalizationCatalog.PLocalizationTextRead("Display.MeaningSingle");
        string collocation = PLocalizationCatalog.PLocalizationTextRead("Display.CollocationSingle");
        string example = PLocalizationCatalog.PLocalizationTextRead("Vita.Example");

        List<PUsageItem> built = new(usages.Count);
        foreach (LUsage usage in usages)
        {
            string owner = usage.LUsageCollocated ? collocation : usage.LUsageQuoted ? example : meaning;
            built.Add(new PUsageItem(usage, owner, unknown, string.Empty, usage.LUsageEpithet));
        }

        return built;
    }

    internal void PUsageItemShow(PWindow host)
    {
        ArgumentNullException.ThrowIfNull(host);

        if (PUsageItemQuoted)
        {
            host.PWindowExampleShow(PUsageItemId);
            return;
        }

        host.PWindowEntryShow(PUsageItemEntry);
    }
}
