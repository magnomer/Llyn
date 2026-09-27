using System;
using System.Windows;
using System.Windows.Input;
using Llyn.Application;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PWorkspaceDialogHandle(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = PSettingsWindow.LWindowWorkspace.QWorkspacePathRead()
        };

        if (dialog.ShowDialog(_pSettingsHost.PWindowSurface) == true)
        {
            PWorkspaceApply(dialog.FolderName);
        }
    }

    private void PWorkspacePathHandle(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            PWorkspaceApply(PWorkspacePath.Text);
            return;
        }

        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspace.QWorkspacePathRead();
        }
    }

    private void PWorkspaceFocusHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        PWorkspacePath.Text = PSettingsWindow.LWindowWorkspace.QWorkspacePathRead();
    }

    private void PWorkspaceApply(string chosen)
    {
        string path = chosen.Trim();
        if (path.Length == 0
            || string.Equals(path, PSettingsWindow.LWindowWorkspace.QWorkspacePathRead(), StringComparison.Ordinal))
        {
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspace.QWorkspacePathRead();
            return;
        }

        if (!_pSettingsHost.PWindowDiscardConfirm())
        {
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspace.QWorkspacePathRead();
            return;
        }

        try
        {
            _pSettingsHost.PWindowWorkspaceChange(path);
        }
        catch (Exception exception)
        {
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspace.QWorkspacePathRead();
            _pSettingsHost.PWindowFailureShow("Workspace.OpenFailed", exception);
            return;
        }

        PSettingsSync();
        PLocalizationApply(PSettingsWindow.LWindowWorkspace.QWorkspaceLocalizationRead());
        _pSettingsHost.PWindowLayout.PLayoutReset();
        _pSettingsHost.PWindowLayout.PLayoutRestore();
        PSettingsWindow.LWindowVistaRestore();
        _pSettingsHost.PWindowViewRestore(PSettingsWindow.LWindowWorkspace.QWorkspaceStateRead());
    }
}
