using System.Windows;
using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PCorpus : UserControl
{
    private PWindow _pCorpusHost = null!;

    private LEngine _lEngine = null!;

    public PCorpus()
    {
        InitializeComponent();
    }

    internal void PCorpusAttach(PWindow host, LEngine engine)
    {
        _pCorpusHost = host;
        _lEngine = engine;
    }

    internal void PCorpusReset()
    {
        PCorpusClear();
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    internal bool PCorpusChangeCheck()
    {
        return PTranscript.Visibility == Visibility.Visible && PTranscriptChangeCheck();
    }

    internal void PCorpusClose()
    {
        PCitationMenu.IsOpen = false;
        PLanguage.IsOpen = false;
        PRankMenu.IsOpen = false;
    }
}
