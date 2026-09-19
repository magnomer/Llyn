using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PInput : UserControl
{
    private LEngine _lEngine = null!;

    private PObserver? _pInputObserver;

    public PInput()
    {
        InitializeComponent();
    }

    internal void PInputAttach(PWindow host, LEngine engine)
    {
        _lEngine = engine;

        PEditor.PEditorAttach(host, engine, "Input", null);

        _pInputObserver = new PObserver(this, PInputBulletinHandle);
        engine.LEngineObserverAttach(_pInputObserver);
    }

    private void PInputBulletinHandle(LBulletin bulletin)
    {
        if (!bulletin.LBulletinMatch(LSubject.LSubjectWorkspace))
        {
            return;
        }

        PInputReset();
    }

    internal void PInputReset()
    {
        PEditor.PEditorReset();
    }

    internal bool PInputDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PInputChangeCheck()
    {
        return PEditor.PEditorChangeCheck();
    }

    internal void PInputClose()
    {
        if (_pInputObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pInputObserver);
            _pInputObserver = null;
        }

        PEditor.PEditorClose();
    }
}
