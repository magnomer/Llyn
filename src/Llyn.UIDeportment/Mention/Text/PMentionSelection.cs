using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal static class PMentionSelection
{
    internal static Rect PMentionSelectionPlace(TextBox box)
    {
        Rect place = box.GetRectFromCharacterIndex(box.SelectionStart);
        return place.IsEmpty ? new Rect(0, box.ActualHeight, 0, 0) : place;
    }
}
