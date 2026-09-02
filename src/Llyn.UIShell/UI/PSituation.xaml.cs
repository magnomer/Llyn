using System.Windows;
using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PSituation : UserControl
{
    private PWindow _pSituationHost = null!;

    private LEngine _lEngine = null!;

    public PSituation()
    {
        InitializeComponent();
    }

    internal void PSituationAttach(PWindow host, LEngine engine)
    {
        _pSituationHost = host;
        _lEngine = engine;
    }

    internal void PSituationReset()
    {
        PSituationClear();
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    internal bool PSituationChangeCheck()
    {
        return PEditor.Visibility == Visibility.Visible && PEditorChangeCheck();
    }

    internal void PSituationClose()
    {
        PCitationMenu.IsOpen = false;
        PTierMenu.IsOpen = false;
    }
}
