using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public class PInput : UserControl
{
    private LEditor _lEditor = null!;

    private Action? _pInputRelease;

    public PInput()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Core/PInput.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
    }

    internal PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    internal void PInputAttach(PWindow host)
    {
        _lEditor = host.PWindowForge.QForgeInputCreate(host.PWindowEnvoy);
        PEditor.PEditorAttach(host, _lEditor);

        _pInputRelease = host.PWindowAtelier.CAtelierObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, PInputReset));
    }

    internal void PInputVistaRestore()
    {
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
        _pInputRelease?.Invoke();
        _pInputRelease = null;

        PEditor.PEditorClose();
    }
}
