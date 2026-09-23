using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal static class PMentionSelection
{
    internal static (int PMentionSelectionOffset, int PMentionSelectionLength) PMentionSelectionRead(
        TextBox box, LWindow window)
    {
        return window.LWindowSpanRead(box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal static LMentionDraft? PMentionSelectionFind(IReadOnlyList<LMentionDraft> mentions, int offset, int length)
    {
        int end = offset + length;
        foreach (LMentionDraft mention in mentions)
        {
            if (offset >= mention.LMentionDraftOffset
                && end <= mention.LMentionDraftOffset + mention.LMentionDraftLength)
            {
                return mention;
            }
        }

        return null;
    }

    internal static Rect PMentionSelectionPlace(TextBox box)
    {
        Rect place = box.GetRectFromCharacterIndex(box.SelectionStart);
        return place.IsEmpty ? new Rect(0, box.ActualHeight, 0, 0) : place;
    }
}
