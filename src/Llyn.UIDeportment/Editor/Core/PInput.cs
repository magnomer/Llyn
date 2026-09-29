using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public class PInput : UserControl
{
    private LEditor _lEditor = null!;

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
    }

    internal void PInputVistaRestore()
    {
        PEditor.PEditorVistaRestore();
    }

    internal void PInputClose()
    {
        PEditor.PEditorClose();
    }
}
