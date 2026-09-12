using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

internal static class PMentionSelection
{
    internal static (int PMentionSelectionOffset, int PMentionSelectionLength) PMentionSelectionRead(TextBox box)
    {
        string text = box.Text;
        int start = box.SelectionStart;
        int end = start + box.SelectionLength;

        while (start < end && char.IsWhiteSpace(text[start]))
        {
            start++;
        }

        while (end > start && char.IsWhiteSpace(text[end - 1]))
        {
            end--;
        }

        int offset = LMentionSpan.LMentionOffsetRead(text, start);
        return (offset, LMentionSpan.LMentionOffsetRead(text, end) - offset);
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
