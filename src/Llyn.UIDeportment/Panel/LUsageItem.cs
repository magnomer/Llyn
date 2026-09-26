using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

        string unknown = QLocalizationCatalog.QLocalizationTextRead("Display.Unknown");
        string meaning = QLocalizationCatalog.QLocalizationTextRead("Display.MeaningSingle");
        string collocation = QLocalizationCatalog.QLocalizationTextRead("Display.CollocationSingle");
        string example = QLocalizationCatalog.QLocalizationTextRead("Vita.Example");

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

    internal static void LUsageItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LUsageItem usage)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PUsageFlag") is Image flag)
        {
            flag.Source = usage.LUsageItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PUsageName") is Run name)
        {
            name.Text = usage.LUsageItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PUsageEpithet") is Run epithet)
        {
            epithet.Text = " " + usage.LUsageItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageTitle") is TextBlock title)
        {
            title.Text = usage.LUsageItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageOwner") is TextBlock owner)
        {
            owner.Text = usage.LUsageItemOwner;
        }
    }
}
