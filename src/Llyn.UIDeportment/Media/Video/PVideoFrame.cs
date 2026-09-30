using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PVideoFrame : Decorator
{
    public PVideoFrame()
    {
        DataContextChanged += PVideoContextHandle;
    }

    private void PVideoContextHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        PVideoFrameResolve();
    }

    private void PVideoFrameResolve()
    {
        if (DataContext is not CVideoDraft draft)
        {
            return;
        }

        Visibility = QLook.QLookVisibleRead(!draft.CVideoDraftEmpty);
        if (PMedia.PMediaRead(this) is PMedia media)
        {
            DataContext = media.PMediaVideoCreate(draft);
        }
    }
}
