using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PRepertoire : UserControl
{
    private PWindow _pRepertoireHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pRepertoireObserver;

    public PRepertoire()
    {
        InitializeComponent();
    }

    internal void PRepertoireAttach(PWindow host, LEngine engine)
    {
        _pRepertoireHost = host;
        _lEngine = engine;

        PAtlas.ItemsSource = _pAtlasList;
        POccurrence.ItemsSource = _pOccurrenceList;

        _pRepertoireObserver = new PObserver(this, PRepertoireBulletinHandle);
        engine.LEngineObserverAttach(_pRepertoireObserver);
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
        if (_pRepertoireObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pRepertoireObserver);
            _pRepertoireObserver = null;
        }

        PTierDropdown.IsOpen = false;
    }
}
