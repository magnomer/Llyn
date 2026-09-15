using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

public partial class PSettings
{
    private (string PDialChild, StackPanel PDialPage, string[] PDialKeys)[] PDialTableRead()
    {
        return
        [
            ("Workspace", PDialWorkspace, ["Workspace.Helper"]),
            ("Language", PDialLanguage, []),
            ("Transcription", PDialTranscription, ["Respelling.Switch", "Respelling.Helper"]),
            ("Web", PDialWeb, ["Frequency.Switch", "Frequency.Helper", "Morphology.Switch", "Morphology.Helper"]),
            ("Layout", PDialLayout, ["Layout.Linked", "Layout.LinkedHelper"])
        ];
    }

    private void PDialShow(string child)
    {
        foreach (PLedgerItem item in _pLedgerList)
        {
            item.PLedgerItemChosen = string.Equals(item.PLedgerItemChild, child, StringComparison.Ordinal);
        }

        foreach ((string name, StackPanel page, _) in PDialTableRead())
        {
            page.Visibility = string.Equals(name, child, StringComparison.Ordinal)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void PDialFolderHandle(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(_lEngine.LEngineWorkspaceRead()) { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            _pSettingsHost.PWindowFailureShow("Settings.FolderFailed", exception);
        }
    }

    private void PDialWidthHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineLayoutReset();
        _pSettingsHost.PWindowLayout.PLayoutReset();
    }
}
