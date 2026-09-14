using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PSettings
{
    private readonly ObservableCollection<PLedgerItem> _pLedgerList = [];

    private void PLedgerBuild()
    {
        _pLedgerList.Clear();
        foreach (string child in new[] { "Workspace", "Language", "Transcription", "Web", "Layout" })
        {
            _pLedgerList.Add(new PLedgerItem(child, _pSettingsHost.PLocalizationTextRead("Settings." + child)));
        }

        PLedger.ItemsSource = _pLedgerList;
        PLedgerEmpty.Visibility = _pLedgerList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PLedgerMetaApply();
    }

    private string PLedgerMetaRead(string child)
    {
        LSettings settings = _lEngine.LEngineSettingsRead();

        switch (child)
        {
            case "Workspace":
                return Path.GetFileName(Path.TrimEndingDirectorySeparator(_lEngine.LEngineWorkspaceRead()));

            case "Language":
                return PLocalization.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item => string.Equals(
                        item.Tag as string, settings.LSettingsLocalization, StringComparison.Ordinal))
                    ?.Content as string ?? settings.LSettingsLocalization;

            case "Transcription":
                return _pSettingsHost.PLocalizationTextRead(
                    settings.LSettingsRespelled ? "Settings.On" : "Settings.Off");

            case "Web":
                int enabled = (settings.LSettingsFrequency ? 1 : 0) + (settings.LSettingsMorphology ? 1 : 0);
                return string.Format(
                    CultureInfo.CurrentCulture,
                    _pSettingsHost.PLocalizationTextRead("Settings.Tally"),
                    enabled,
                    2);

            case "Layout":
                return _pSettingsHost.PLocalizationTextRead(
                    settings.LSettingsLinked ? "Layout.LinkedMeta" : "Layout.FreeMeta");

            default:
                return string.Empty;
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
        Dictionary<string, string[]> keys = new(StringComparer.Ordinal)
        {
            ["Workspace"] = ["Workspace.Helper"],
            ["Language"] = [],
            ["Transcription"] = ["Respelling.Switch", "Respelling.Helper"],
            ["Web"] = ["Frequency.Switch", "Frequency.Helper", "Morphology.Switch", "Morphology.Helper"],
            ["Layout"] = ["Layout.Linked", "Layout.LinkedHelper"]
        };

        List<PLedgerItem> shown = _pLedgerList
            .Where(item => wanted.Length == 0 || keys[item.PLedgerItemChild]
                .Prepend("Settings." + item.PLedgerItemChild + "Helper")
                .Prepend("Settings." + item.PLedgerItemChild)
                .Select(key => LCase.LCaseLowerChange(
                    _pSettingsHost.PLocalizationTextRead(key), CultureInfo.CurrentCulture))
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
