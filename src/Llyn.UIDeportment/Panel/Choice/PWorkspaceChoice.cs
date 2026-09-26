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
            Title = PLocalizationCatalog.PLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = PSettingsWindow.LWindowWorkspaceRead()
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
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspaceRead();
        }
    }

    private void PWorkspaceFocusHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        PWorkspacePath.Text = PSettingsWindow.LWindowWorkspaceRead();
    }

    private void PWorkspaceApply(string chosen)
    {
        string path = chosen.Trim();
        if (path.Length == 0 || string.Equals(path, PSettingsWindow.LWindowWorkspaceRead(), StringComparison.Ordinal))
        {
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspaceRead();
            return;
        }

        if (!_pSettingsHost.PWindowDiscardConfirm())
        {
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspaceRead();
            return;
        }

        try
        {
            _pSettingsHost.PWindowWorkspaceChange(path);
        }
        catch (Exception exception)
        {
            PWorkspacePath.Text = PSettingsWindow.LWindowWorkspaceRead();
            _pSettingsHost.PWindowFailureShow("Workspace.OpenFailed", exception);
            return;
        }

        PSettingsSync();
        PLocalizationApply(PSettingsWindow.LWindowLocalizationRead());
        _pSettingsHost.PWindowLayout.PLayoutReset();
        _pSettingsHost.PWindowLayout.PLayoutRestore();
        _pSettingsHost.PWindowViewRestore(PSettingsWindow.LWindowStateRead());
    }
}
