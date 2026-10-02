using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QWorkspace
{
    private readonly FrameworkElement _qWorkspaceSettings;

    private CAtelier _qWorkspaceAtelier = null!;

    private CEnvoy _qWorkspaceEnvoy = null!;

    internal QWorkspace(FrameworkElement settings)
    {
        _qWorkspaceSettings = settings;
        QWorkspaceDialogIcon.QIconSource = QIcon.QIconResolve("folder", 24);
        QWorkspacePath.KeyDown += QWorkspaceEscapeRefine;
        QWorkspacePath.KeyDown += QWorkspacePathObserve;
        QWorkspacePath.LostKeyboardFocus += QWorkspaceFocusRefine;
        QWorkspaceDialog.Click += QWorkspaceDialogObserve;
    }

    private TextBox QWorkspacePath => QContract.QContractFind<TextBox>(_qWorkspaceSettings, "PWorkspacePath");

    private Button QWorkspaceDialog => QContract.QContractFind<Button>(_qWorkspaceSettings, "PWorkspaceDialog");

    private QIconImage QWorkspaceDialogIcon =>
        QContract.QContractFind<QIconImage>(_qWorkspaceSettings, "PWorkspaceDialogIcon");

    internal void QWorkspaceIntroduce(CAtelier atelier, CEnvoy envoy)
    {
        _qWorkspaceAtelier = atelier;
        _qWorkspaceEnvoy = envoy;
    }

    internal void QWorkspacePathRefine(string path)
    {
        QWorkspacePath.Text = path;
    }

    private void QWorkspaceDialogObserve(object sender, RoutedEventArgs e)
    {
        QWorkspacePathRefine(_qWorkspaceAtelier.CAtelierWorkspace.CWorkspaceChange(_qWorkspaceEnvoy));
    }

    private void QWorkspacePathObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        string path = _qWorkspaceAtelier.CAtelierWorkspace.CWorkspaceChange(QWorkspacePath.Text, _qWorkspaceEnvoy);
        e.Handled = true;
        QWorkspacePathRefine(path);
    }

    private void QWorkspaceEscapeRefine(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
        {
            return;
        }

        e.Handled = true;
        QWorkspacePathRefine(_qWorkspaceAtelier.CAtelierPathRead());
    }

    private void QWorkspaceFocusRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        QWorkspacePathRefine(_qWorkspaceAtelier.CAtelierPathRead());
    }
}
