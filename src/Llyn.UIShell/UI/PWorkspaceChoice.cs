using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PWorkspaceDialogHandle(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = _pSettingsHost.PLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = PWorkspacePath.Text
        };

        if (dialog.ShowDialog(_pSettingsHost) == true)
        {
            PWorkspacePath.Text = dialog.FolderName;
            PWorkspaceApply();
        }
    }

    private void PWorkspacePathHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        PWorkspaceApply();
    }

    private void PWorkspaceApply()
    {
        string path = PWorkspacePath.Text?.Trim() ?? string.Empty;
        if (path.Length == 0 || string.Equals(path, _lEngine.LEngineWorkspaceRead(), StringComparison.Ordinal))
        {
            return;
        }

        if (!_pSettingsHost.PWindowDiscardConfirm())
        {
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            return;
        }

        try
        {
            _lEngine.LEngineWorkspaceChange(path);
        }
        catch (Exception exception)
        {
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            _pSettingsHost.PWindowFailureShow("Workspace.OpenFailed", exception);
            return;
        }

        _pSettingsHost.PInput.PInputReset();
        _pSettingsHost.PLibrary.PLibraryReset();
        _pSettingsHost.PPhonology.PPhonologyReset();
        _pSettingsHost.PTaxonomy.PTaxonomyReset();
        _pSettingsHost.PRepertoire.PRepertoireReset();
        _pSettingsHost.PCorpus.PCorpusReset();
        _pSettingsHost.PReference.PReferenceReset();
        _pSettingsHost.PFavorite.PFavoriteReset();
        _pSettingsHost.PDuplex.PDuplexReset();
    }
}
