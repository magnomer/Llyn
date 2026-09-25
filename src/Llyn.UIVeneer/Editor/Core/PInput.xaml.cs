using System;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PInput : UserControl
{
    private PWindow _pInputHost = null!;

    private LEditor _lEditor = null!;

    private Action<LBulletin>? _pInputObserver;

    public PInput()
    {
        InitializeComponent();
    }

    internal void PInputAttach(PWindow host)
    {
        _pInputHost = host;

        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        PEditor.PEditorAttach(host, _lEditor);

        _pInputObserver = LObserver.LObserverCreate(this, PInputBulletinHandle);
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
        _lEditor.LEditorVistaRestore(_pInputHost.PWindowDeportment);
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
