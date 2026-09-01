using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

/// <summary>
/// The workspace folder the user picks in the settings panel: the path field and its browse button,
/// and what a change to that path costs — a different database, so the form, the list, and the
/// display all move onto the new workspace or the change is called off.
/// </summary>
public partial class PSettings
{
    private void PWorkspaceBrowseHandle(object sender, RoutedEventArgs e)
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

        // Changing the workspace throws the form away with it, and this runs from a mere
        // LostKeyboardFocus on the path box — tabbing past it must not cost the user what they typed.
        if (!_pSettingsHost.PWindowDiscardConfirm())
        {
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            return;
        }

        try
        {
            _lEngine.LEngineWorkspaceChange(path);
        }
        catch (Exception)
        {
            // An unusable path (permission, invalid characters) leaves the previous workspace in place;
            // restore the field so it keeps showing the folder actually in use.
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            return;
        }

        // The new workspace has its own database, so everything on screen came from a database that is
        // no longer open and carries ids that mean nothing here. The input form is emptied, and the
        // list and display are re-read from the new workspace.
        _pSettingsHost.PInput.PInputReset();
        _pSettingsHost.PList.PListReset();
    }
}
