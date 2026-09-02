using System.Windows;
using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PExample : UserControl
{
    private PWindow _pExampleHost = null!;

    private LEngine _lEngine = null!;

    public PExample()
    {
        InitializeComponent();
    }

    internal void PExampleAttach(PWindow host, LEngine engine)
    {
        _pExampleHost = host;
        _lEngine = engine;
    }

    internal void PExampleReset()
    {
        PExampleClear();
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    internal bool PExampleChangeCheck()
    {
        return PEditor.Visibility == Visibility.Visible && PEditorChangeCheck();
    }

    internal void PExampleClose()
    {
        PCitationMenu.IsOpen = false;
        PLangcodeMenu.IsOpen = false;
        PRankMenu.IsOpen = false;
    }
}
