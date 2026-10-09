using System.Windows.Controls;
using System.Windows.Documents;

namespace Llyn.UIDeportment;

public readonly record struct PSwathSeam(int PSwathSeamIndex, TextPointer? PSwathSeamCaret)
{
    public TextPointer PSwathSeamStart(int index, TextBlock block)
    {
        return index == PSwathSeamIndex ? PSwathSeamCaret ?? block.ContentStart : block.ContentStart;
    }

    public TextPointer PSwathSeamFinish(int index, TextBlock block)
    {
        return index == PSwathSeamIndex ? PSwathSeamCaret ?? block.ContentEnd : block.ContentEnd;
    }
}
