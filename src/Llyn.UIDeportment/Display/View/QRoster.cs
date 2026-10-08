using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Llyn.UIDeportment;

internal sealed class QRoster
{
    private readonly QCompass _qRosterCompass;

    internal QRoster(FrameworkElement surface, QCompass compass)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(compass);

        _qRosterCompass = compass;

        QLookItem.QLookItemAttach(
            QContract.QContractFind<ItemsControl>(surface, "PDisplaySpeech"), QRosterSpeechRefine);
        QLookItem.QLookItemAttach(
            QContract.QContractFind<ItemsControl>(surface, "PDisplayMeaning"), PLeaf.PLeafCardRefine);
        QLookItem.QLookItemAttach(
            QContract.QContractFind<ItemsControl>(surface, "PDisplayCollocation"), PLeaf.PLeafCardRefine);
        QLookItem.QLookItemAttach(
            QContract.QContractFind<ItemsControl>(surface, "PDisplayIncoming"), QRosterUsageRefine);
        QLookItem.QLookItemAttach(
            QContract.QContractFind<ItemsControl>(surface, "PCompassList"), QRosterCompassRefine);
    }

    private static void QRosterSpeechRefine(FrameworkElement container, object item, string? _)
    {
        if (item is string speech && QLook.QLookPartFind<TextBlock>(container, "PSpeechName") is TextBlock name)
        {
            name.Text = speech;
        }
    }

    private static void QRosterUsageRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QUsageItem usage)
        {
            return;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PUsageIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("incoming", 24);
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

        if (QLook.QLookPartFind<Image>(container, "PUsageFlag") is Image flag)
        {
            flag.Source = usage.QUsageItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageLanguage") is TextBlock language)
        {
            language.Text = usage.QUsageItemLanguage;
        }
    }

    private void QRosterCompassRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QCompassItem compass)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PCompassRow") is Button row)
        {
            if (compass.QCompassItemCurrent)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QRosterCompassObserve;
            row.Click += QRosterCompassObserve;
        }

        if (QLook.QLookPartFind<Grid>(container, "PCompassIndent") is Grid indent)
        {
            indent.Margin = compass.QCompassItemIndent;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCompassNumber") is TextBlock number)
        {
            number.Text = compass.QCompassItemNumber;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCompassName") is TextBlock name)
        {
            name.Text = compass.QCompassItemName;
        }
    }

    private void QRosterCompassObserve(object sender, RoutedEventArgs e)
    {
        _qRosterCompass.QCompassRowRefine(sender);
    }
}
