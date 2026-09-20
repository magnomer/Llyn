using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIVeneer;

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
        return PLocalizationCatalog.PLocalizationTextRead("Settings." + child);
    }

    private string PLedgerMetaRead(string child)
    {
        LSettings settings = PSettingsWindow.LWindowSettingsRead();

        switch (child)
        {
            case "Workspace":
                return PSettingsWindow.LWindowWorkspaceFormat();

            case "Language":
                return PLocalization.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item => string.Equals(
                        item.Tag as string, settings.LSettingsLocalization, StringComparison.Ordinal))
                    ?.Content as string ?? settings.LSettingsLocalization;

            case "Transcription":
                return PLocalizationCatalog.PLocalizationTextRead(
                    settings.LSettingsRespelled ? "Settings.On" : "Settings.Off");

            case "Listing":
                return PLocalizationCatalog.PLocalizationTextRead(
                    settings.LSettingsEpithet ? "Settings.On" : "Settings.Off");

            case "Web":
                return string.Format(
                    CultureInfo.CurrentCulture,
                    PLocalizationCatalog.PLocalizationTextRead("Settings.Tally"),
                    settings.LSettingsOnline,
                    2);

            case "Layout":
                return PLocalizationCatalog.PLocalizationTextRead(
                    _pSettingsHost.PWindowPosture.LPostureRead().LPostureStateLinked
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
        string wanted = LCase.LCaseLowerChange(text.Trim(), CultureInfo.CurrentCulture);
        Dictionary<string, string[]> keys = PDialTableRead()
            .ToDictionary(row => row.PDialChild, row => row.PDialKeys, StringComparer.Ordinal);

        List<PLedgerItem> shown = _pLedgerList
            .Where(item => wanted.Length == 0 || keys[item.PLedgerItemChild]
                .Prepend("Settings." + item.PLedgerItemChild + "Helper")
                .Prepend("Settings." + item.PLedgerItemChild)
                .Select(key => LCase.LCaseLowerChange(
                    PLocalizationCatalog.PLocalizationTextRead(key), CultureInfo.CurrentCulture))
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

    private void PWinnowHandle(object sender, TextChangedEventArgs e)
    {
        PLedgerFind(PWinnow.Text ?? string.Empty);
    }
}
