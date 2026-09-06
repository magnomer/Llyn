using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PDuplex : UserControl
{
    private PWindow _pDuplexHost = null!;

    private LEngine _lEngine = null!;

    public PDuplex()
    {
        InitializeComponent();
    }

    internal void PDuplexAttach(PWindow host, LEngine engine)
    {
        _pDuplexHost = host;
        _lEngine = engine;

        PLeftIndex.ItemsSource = _pLeftIndex;
        PRightIndex.ItemsSource = _pRightIndex;

        PLeftDisplay.PDisplayAttach(host, engine);
        PRightDisplay.PDisplayAttach(host, engine);
    }

    internal async void PDuplexRestore(LWorkspaceState state)
    {
        await PEnsign.PEnsignLoad(_lEngine);

        PLeftQuery.Text = string.Empty;
        PRightQuery.Text = string.Empty;

        PLeftDisplay.PDisplayClear();
        PRightDisplay.PDisplayClear();

        if (state.LWorkspaceStateLeft is string left)
        {
            PDuplexEntryShow(left, PLeftDisplay);
        }

        if (state.LWorkspaceStateRight is string right)
        {
            PDuplexEntryShow(right, PRightDisplay);
        }
    }

    internal void PDuplexClose()
    {
        PLeftDisplay.PDisplayClose();
        PRightDisplay.PDisplayClose();
    }
}
