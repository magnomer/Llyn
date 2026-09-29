using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PWorkspaceDialogObserve(object sender, RoutedEventArgs e)
    {
        PWorkspacePathRefine(PSettingsAtelier.CAtelierWorkspace.CWorkspaceChange(_pSettingsHost.PWindowEnvoy));
    }

    private void PWorkspacePathObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        string path = PSettingsAtelier.CAtelierWorkspace.CWorkspaceChange(
            PWorkspacePath.Text, _pSettingsHost.PWindowEnvoy);
        e.Handled = true;
        PWorkspacePathRefine(path);
    }

    private void PWorkspaceEscapeRefine(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
        {
            return;
        }

        e.Handled = true;
        PWorkspacePathRefine(PSettingsAtelier.CAtelierPathRead());
    }

    private void PWorkspaceFocusRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        PWorkspacePathRefine(PSettingsAtelier.CAtelierPathRead());
    }

    private void PWorkspacePathRefine(string path)
    {
        PWorkspacePath.Text = path;
    }
}
