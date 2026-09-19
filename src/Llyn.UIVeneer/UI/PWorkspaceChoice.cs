using System;
using System.Windows;
using System.Windows.Input;
using Llyn.Application;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PWorkspaceDialogHandle(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = PLocalizationCatalog.PLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = _lEngine.LEngineWorkspaceRead()
        };

        if (dialog.ShowDialog(_pSettingsHost) == true)
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
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
        }
    }

    private void PWorkspaceFocusHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
    }

    private void PWorkspaceApply(string chosen)
    {
        string path = chosen.Trim();
        if (path.Length == 0 || string.Equals(path, _lEngine.LEngineWorkspaceRead(), StringComparison.Ordinal))
        {
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            return;
        }

        if (!_pSettingsHost.PWindowDiscardConfirm())
        {
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            return;
        }

        try
        {
            _pSettingsHost.PWindowWorkspaceChange(path);
        }
        catch (Exception exception)
        {
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            _pSettingsHost.PWindowFailureShow("Workspace.OpenFailed", exception);
            return;
        }

        PSettingsSync();
        PLocalizationApply(LLocalization.LLocalizationNormalize(
            _lEngine.LEngineSettingsRead().LSettingsLocalization));
        _pSettingsHost.PWindowLayout.PLayoutReset();
        _pSettingsHost.PWindowLayout.PLayoutRestore();
        _pSettingsHost.PWindowViewRestore(_lEngine.LEngineStateRead());
    }
}
