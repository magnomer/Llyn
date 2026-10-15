using System.Windows.Controls;
using System.Windows.Documents;

namespace Llyn.UIDeportment;

public readonly record struct QSwathSeam(int QSwathSeamIndex, TextPointer? QSwathSeamCaret)
{
    public TextPointer QSwathSeamStart(int index, TextBlock block)
    {
        return index == QSwathSeamIndex ? QSwathSeamCaret ?? block.ContentStart : block.ContentStart;
    }

    public TextPointer QSwathSeamFinish(int index, TextBlock block)
    {
        return index == QSwathSeamIndex ? QSwathSeamCaret ?? block.ContentEnd : block.ContentEnd;
    }
}
