using System.Windows;
using System.Windows.Controls;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal static class PMentionSelection
{
    internal static (int PMentionSelectionOffset, int PMentionSelectionLength) PMentionSelectionRead(
        TextBox box, LWindow window)
    {
        return window.LWindowSpanRead(box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal static Rect PMentionSelectionPlace(TextBox box)
    {
        Rect place = box.GetRectFromCharacterIndex(box.SelectionStart);
        return place.IsEmpty ? new Rect(0, box.ActualHeight, 0, 0) : place;
    }
}
