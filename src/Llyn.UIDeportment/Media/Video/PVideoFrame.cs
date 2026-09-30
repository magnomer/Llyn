using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PVideoFrame : Decorator
{
    public PVideoFrame()
    {
        DataContextChanged += PVideoContextRefine;
    }

    private void PVideoContextRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not CVideoDraft draft)
        {
            return;
        }

        PVideoEmptyRefine(draft);
        PVideoRowRefine(draft);
    }

    private void PVideoEmptyRefine(CVideoDraft draft)
    {
        Visibility = QLook.QLookVisibleRead(!draft.CVideoDraftEmpty);
    }

    private void PVideoRowRefine(CVideoDraft draft)
    {
        if (PMedia.PMediaRead(this) is PMedia media)
        {
            DataContext = media.PMediaVideoCreate(draft);
        }
    }
}
