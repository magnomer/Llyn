using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

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
        if (DataContext is not LVideoDraft draft)
        {
            return;
        }

        Visibility = PLook.PLookVisibleRead(!draft.LVideoDraftEmpty);
        if (PMedia.PMediaRead(this) is PMedia media)
        {
            DataContext = media.PMediaVideoCreate(draft);
        }
    }
}
