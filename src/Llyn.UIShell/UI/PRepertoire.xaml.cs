using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PRepertoire : UserControl
{
    private PWindow _pRepertoireHost = null!;

    private LEngine _lEngine = null!;

    public PRepertoire()
    {
        InitializeComponent();
    }

    internal void PRepertoireAttach(PWindow host, LEngine engine)
    {
        _pRepertoireHost = host;
        _lEngine = engine;
    }

    internal void PRepertoireReset()
    {
        PRepertoireClear();
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    internal bool PRepertoireChangeCheck()
    {
        return PScenarioChangeCheck();
    }

    internal void PRepertoireClose()
    {
        PTierDropdown.IsOpen = false;
    }
}
