using System;
using System.Windows;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PWorkspaceDialogHandle(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = PWorkspacePath.Text
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
            PWorkspacePath.Text = PSettingsAtelier.CAtelierPathRead();
        }
    }

    private void PWorkspaceFocusHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        PWorkspacePath.Text = PSettingsAtelier.CAtelierPathRead();
    }

    private void PWorkspaceApply(string chosen)
    {
        CWorkspaceState? state;
        try
        {
            state = PSettingsAtelier.CAtelierWorkspaceChange(chosen, _pSettingsHost.PWindowEnvoy);
        }
        catch (Exception exception)
        {
            PWorkspacePath.Text = _pSettingsState.CLedgerStatePath;
            _pSettingsHost.PWindowFailureRefine("Workspace.OpenFailed", exception);
            return;
        }

        if (state is null)
        {
            PWorkspacePath.Text = _pSettingsState.CLedgerStatePath;
            return;
        }

        _pSettingsHost.PWindowLayout.PLayoutResetRefine();
        _pSettingsHost.PWindowLayout.PLayoutRefine();
        _pSettingsHost.PWindowForge.QForgeVistaRestore();
        PSettingsAtelier.CAtelierOpen();
    }
}
