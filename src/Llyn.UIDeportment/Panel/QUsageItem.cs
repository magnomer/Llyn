using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QUsageItem
{
    internal QUsageItem(CUsage usage)
    {
        ArgumentNullException.ThrowIfNull(usage);

        QUsageItemUsage = usage;
        QUsageItemName = usage.CUsageName;
        QUsageItemEpithet = usage.CUsageEpithet;
        QUsageItemLanguage = usage.CUsageLanguage;
        QUsageItemOwner = QLocalizationCatalog.QLocalizationTextRead(usage.CUsageOwnerKey);
        QUsageItemTitle = usage.CUsageTitleKey is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : usage.CUsageTitle.CStateValueText;
        QUsageItemFlag = QEnsignImage.QEnsignRead(QUsageItemLanguage);
    }

    internal CUsage QUsageItemUsage { get; }

    internal string QUsageItemName { get; }

    internal string QUsageItemEpithet { get; }

    internal string QUsageItemLanguage { get; }

    internal string QUsageItemOwner { get; }

    internal string QUsageItemTitle { get; }

    internal ImageSource? QUsageItemFlag { get; }

    internal static IReadOnlyList<QUsageItem> QUsageItemBuild(IReadOnlyList<CUsage> usages)
    {
        ArgumentNullException.ThrowIfNull(usages);

        return usages.Select(static usage => new QUsageItem(usage)).ToList();
    }

    internal static void QUsageItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QUsageItem usage)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PUsageFlag") is Image flag)
        {
            flag.Source = usage.QUsageItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PUsageName") is Run name)
        {
            name.Text = usage.QUsageItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PUsageEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(usage.QUsageItemEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageTitle") is TextBlock title)
        {
            title.Text = usage.QUsageItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageOwner") is TextBlock owner)
        {
            owner.Text = usage.QUsageItemOwner;
        }
    }
}
