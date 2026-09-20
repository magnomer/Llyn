using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PInput : UserControl
{
    private PWindow _pInputHost = null!;

    private LEditor _lEditor = null!;

    private PObserver? _pInputObserver;

    public PInput()
    {
        InitializeComponent();
    }

    internal void PInputAttach(PWindow host, LEngine engine)
    {
        _pInputHost = host;

        _lEditor = new LEditor(engine, engine, engine, engine, host.PWindowUnreadableConfirm);
        PEditor.PEditorAttach(host, _lEditor);

        _pInputObserver = new PObserver(this, PInputBulletinHandle);
        host.PWindowDeportment.LWindowObserverAttach(_pInputObserver);
    }

    private void PInputBulletinHandle(LBulletin bulletin)
    {
        if (!bulletin.LBulletinMatch(LSubject.LSubjectWorkspace))
        {
            return;
        }

        PInputReset();
    }

    internal void PInputVistaRestore()
    {
        _lEditor.LEditorVistaRestore(_pInputHost.PWindowPosture);
        PEditor.PEditorVistaRestore();
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
            _pInputHost.PWindowDeportment.LWindowObserverDetach(_pInputObserver);
            _pInputObserver = null;
        }

        PEditor.PEditorClose();
    }
}
