using System.Windows.Controls;
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

        PLeftDisplay.PDisplayAttach(engine);
        PRightDisplay.PDisplayAttach(engine);
    }

    internal void PDuplexReset()
    {
        PLeftQuery.Text = string.Empty;
        PRightQuery.Text = string.Empty;

        PLeftDisplay.PDisplayClear();
        PRightDisplay.PDisplayClear();
    }

    internal void PDuplexClose()
    {
        PLeftDisplay.PDisplayClose();
        PRightDisplay.PDisplayClose();
    }
}
