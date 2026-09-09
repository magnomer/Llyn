using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTenor : UserControl
{
    private PWindow _pTenorHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pTenorObserver;

    public PTenor()
    {
        InitializeComponent();
    }

    internal void PTenorAttach(PWindow host, LEngine engine)
    {
        _pTenorHost = host;
        _lEngine = engine;

        PGamut.ItemsSource = _pGamutList;
        PCohort.ItemsSource = _pCohortList;

        _pTenorObserver = new PObserver(this, PTenorBulletinHandle);
        engine.LEngineObserverAttach(_pTenorObserver);

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Tenor", null);
        PEditor.PEditorChangeNotice = changed => PTenorStore.IsEnabled = changed;
    }

    internal void PTenorReset()
    {
        PTenorClear();
        PGamutReset();
        PGamutFind(PSounding.Text ?? string.Empty);
    }

    internal bool PTenorDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PTenorChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PTenorClose()
    {
        if (_pTenorObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pTenorObserver);
            _pTenorObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
