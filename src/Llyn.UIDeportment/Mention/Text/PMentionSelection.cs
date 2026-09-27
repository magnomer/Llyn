using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal static class PMentionSelection
{
    internal static (int PMentionSelectionOffset, int PMentionSelectionLength) PMentionSelectionRead(
        TextBox box, CAtelier atelier)
    {
        return atelier.CAtelierMention.CMentionSpanRead(
            box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal static Rect PMentionSelectionPlace(TextBox box)
    {
        Rect place = box.GetRectFromCharacterIndex(box.SelectionStart);
        return place.IsEmpty ? new Rect(0, box.ActualHeight, 0, 0) : place;
    }
}
