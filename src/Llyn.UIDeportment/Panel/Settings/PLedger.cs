using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private readonly ObservableCollection<PLedgerItem> _pLedgerList = [];

    private void PLedgerBuild()
    {
        _pLedgerList.Clear();
        foreach ((string child, _, _) in PDialTableRead())
        {
            _pLedgerList.Add(new PLedgerItem(child, PLedgerTitleRead(child)));
        }

        PLedger.ItemsSource = _pLedgerList;
        PLedgerEmpty.Visibility = _pLedgerList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PLedgerMetaApply();
    }

    private string PLedgerTitleRead(string child)
    {
        return QLocalizationCatalog.QLocalizationTextRead("Settings." + child);
    }

    private string PLedgerMetaRead(string child)
    {
        CSettings settings = PSettingsWindow.LWindowWorkspace.QWorkspaceSettingsRead();

        switch (child)
        {
            case "Workspace":
                return PSettingsWindow.LWindowWorkspace.QWorkspacePathFormat();

            case "Language":
                return PLedgerLanguageRead(settings.CSettingsLocalization);

            case "Transcription":
                return QLocalizationCatalog.QLocalizationTextRead(
                    settings.CSettingsRespelled ? "Settings.On" : "Settings.Off");

            case "Listing":
                return QLocalizationCatalog.QLocalizationTextRead(
                    settings.CSettingsEpithet ? "Settings.On" : "Settings.Off");

            case "Web":
                return string.Format(
                    CultureInfo.CurrentCulture,
                    QLocalizationCatalog.QLocalizationTextRead("Settings.Tally"),
                    settings.CSettingsOnline,
                    2);

            case "Layout":
                return QLocalizationCatalog.QLocalizationTextRead(
                    PSettingsWindow.LWindowPostureRead().CPostureStateLinked
                        ? "Layout.LinkedMeta"
                        : "Layout.FreeMeta");

            default:
                return string.Empty;
        }
    }

    private void PLedgerTitleApply()
    {
        foreach (PLedgerItem item in _pLedgerList)
        {
            item.PLedgerItemTitle = PLedgerTitleRead(item.PLedgerItemChild);
        }
    }

    private void PLedgerMetaApply()
    {
        foreach (PLedgerItem item in _pLedgerList)
        {
            item.PLedgerItemMeta = PLedgerMetaRead(item.PLedgerItemChild);
        }
    }

    private void PLedgerFind(string text)
    {
        TextInfo casing = CultureInfo.CurrentCulture.TextInfo;
        string wanted = casing.ToLower(text.Trim());
        Dictionary<string, string[]> keys = PDialTableRead()
            .ToDictionary(row => row.PDialChild, row => row.PDialKeys, StringComparer.Ordinal);

        List<PLedgerItem> shown = _pLedgerList
            .Where(item => wanted.Length == 0 || keys[item.PLedgerItemChild]
                .Prepend("Settings." + item.PLedgerItemChild + "Helper")
                .Prepend("Settings." + item.PLedgerItemChild)
                .Select(key => casing.ToLower(QLocalizationCatalog.QLocalizationTextRead(key)))
                .Any(label => label.Contains(wanted, StringComparison.Ordinal)))
            .ToList();

        PLedger.ItemsSource = shown;
        PLedgerEmpty.Visibility = shown.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PLedgerHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PLedgerItem item)
        {
            return;
        }

        PDialShow(item.PLedgerItemChild);
    }

    private void PLedgerApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PLedgerItem ledger)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PLedgerRow") is Button row)
        {
            if (ledger.PLedgerItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= PLedgerHandle;
            row.Click += PLedgerHandle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLedgerTitle") is TextBlock title)
        {
            title.Text = ledger.PLedgerItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLedgerMeta") is TextBlock meta)
        {
            meta.Text = ledger.PLedgerItemMeta;
        }
    }

    private void PWinnowHandle(object sender, TextChangedEventArgs e)
    {
        PLedgerFind(PWinnow.Text ?? string.Empty);
    }

    private string PLedgerLanguageRead(string localization)
    {
        return PLocalization.Items
            .OfType<ComboBoxItem>()
            .FirstOrDefault(item => string.Equals(item.Tag as string, localization, StringComparison.Ordinal))
            ?.Content as string ?? localization;
    }
}
