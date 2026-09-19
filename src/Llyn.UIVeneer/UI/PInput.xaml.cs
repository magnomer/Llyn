using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PInput : UserControl
{
    private LEngine _lEngine = null!;

    private LEditor _lEditor = null!;

    private PObserver? _pInputObserver;

    public PInput()
    {
        InitializeComponent();
    }

    internal void PInputAttach(PWindow host, LEngine engine)
    {
        _lEngine = engine;

        _lEditor = new LEditor(engine, host.PWindowUnreadableConfirm);
        PEditor.PEditorAttach(host, engine, _lEditor);

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

    internal void PInputVistaRestore(LVista vista)
    {
        PEditor.PEditorVistaRestore(vista);
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
