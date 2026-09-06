using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PPhonology : UserControl
{
    private PWindow _pPhonologyHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pPhonologyObserver;

    public PPhonology()
    {
        InitializeComponent();
    }

    internal void PPhonologyAttach(PWindow host, LEngine engine)
    {
        _pPhonologyHost = host;
        _lEngine = engine;

        PInventory.ItemsSource = _pInventoryList;

        _pPhonologyObserver = new PObserver(this, PPhonologyBulletinHandle);
        engine.LEngineObserverAttach(_pPhonologyObserver);

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Phonology", null);

        PArticulation.PArticulationAttach(PEditor.PPronunciation);
    }

    internal void PPhonologyReset()
    {
        PPhonologyClear();
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    internal bool PPhonologyDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PPhonologyChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PPhonologyClose()
    {
        if (_pPhonologyObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pPhonologyObserver);
            _pPhonologyObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
