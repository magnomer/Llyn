using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public sealed class QVideoFrame : Decorator
{
    public QVideoFrame()
    {
        DataContextChanged += PVideoContextRefine;
    }

    private void PVideoContextRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not QLeafVideo video)
        {
            return;
        }

        PVideoEmptyRefine(video);
        PVideoRowRefine(video);
    }

    private void PVideoEmptyRefine(QLeafVideo video)
    {
        Visibility = QLook.QLookVisibleRead(!video.QLeafVideoEmpty);
    }

    private void PVideoRowRefine(QLeafVideo video)
    {
        DataContext = video.QLeafVideoRow;
    }
}
