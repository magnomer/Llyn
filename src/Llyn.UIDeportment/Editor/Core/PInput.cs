using System;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public class PInput : UserControl
{
    public PInput()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Core/PInput.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
    }

    internal PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    internal void PInputIntroduce(PWindow host)
    {
        PEditor.PEditorIntroduce(host, new QEditor(host.PWindowAtelier.CAtelierInputCreate(host.PWindowEnvoy)));
    }
}
