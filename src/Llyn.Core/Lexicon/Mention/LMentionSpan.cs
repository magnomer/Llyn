using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.Core;

public static class LMentionSpan
{
    public static (int LMentionSpanOffset, int LMentionSpanLength) LMentionSpanResolve(
        string text, int offset, bool separated)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        List<Rune> runes = new();
        foreach (Rune rune in text.EnumerateRunes())
        {
            runes.Add(rune);
        }

        if (offset >= runes.Count || !LMentionWordCheck(runes[offset], separated))
        {
            return (offset, 0);
        }

        int start = offset;
        while (start > 0 && LMentionJoinCheck(runes[start - 1], runes[start], separated))
        {
            start--;
        }

        int end = offset;
        while (end + 1 < runes.Count && LMentionJoinCheck(runes[end], runes[end + 1], separated))
        {
            end++;
        }

        return (start, end - start + 1);
    }

    public static LMentionDraft LMentionSpanRead(string text, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(text);

        int end = start + length;
        while (start < end && char.IsWhiteSpace(text[start]))
        {
            start++;
        }

        while (end > start && char.IsWhiteSpace(text[end - 1]))
        {
            end--;
        }

        int offset = LMentionOffsetRead(text, start);
        return new LMentionDraft(0, offset, LMentionOffsetRead(text, end) - offset, 0);
    }

    public static IReadOnlyList<LMentionPiece> LMentionSpanDivide(string text, IReadOnlyList<LMention> mentions)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mentions);

        if (LMention.LMentionOverlapCheck(mentions))
        {
            throw new ArgumentException("Mentions overlap.", nameof(mentions));
        }

        int count = 0;
        foreach (Rune rune in text.EnumerateRunes())
        {
            count++;
        }

        List<LMentionPiece> pieces = [];
        int cursor = 0;
        foreach (LMention mention in LMention.LMentionSort(mentions))
        {
            int start = Math.Max(mention.LMentionOffset, cursor);
            int end = Math.Min(mention.LMentionOffset + mention.LMentionLength, count);
            if (end <= start)
            {
                continue;
            }

            if (start > cursor)
            {
                pieces.Add(new LMentionPiece(cursor, start - cursor, null, LMentionTextRead(text, cursor, start)));
            }

            pieces.Add(new LMentionPiece(start, end - start, mention, LMentionTextRead(text, start, end)));
            cursor = end;
        }

        if (cursor < count)
        {
            pieces.Add(new LMentionPiece(cursor, count - cursor, null, LMentionTextRead(text, cursor, count)));
        }

        return pieces;
    }

    public static LMentionDraft? LMentionSpanFind(IReadOnlyList<LMentionDraft> mentions, LMentionDraft span)
    {
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(span);

        int end = span.LMentionDraftOffset + span.LMentionDraftLength;
        foreach (LMentionDraft mention in mentions)
        {
            if (span.LMentionDraftOffset >= mention.LMentionDraftOffset
                && end <= mention.LMentionDraftOffset + mention.LMentionDraftLength)
            {
                return mention;
            }
        }

        return null;
    }

    public static int LMentionOffsetRead(string text, int unit)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(unit);

        int offset = 0;
        int index = 0;
        foreach (Rune rune in text.EnumerateRunes())
        {
            if (unit < index + rune.Utf16SequenceLength)
            {
                return offset;
            }

            index += rune.Utf16SequenceLength;
            offset++;
        }

        return offset;
    }

    public static int LMentionUnitRead(string text, int offset)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        int unit = 0;
        int index = 0;
        foreach (Rune rune in text.EnumerateRunes())
        {
            if (index == offset)
            {
                return unit;
            }

            unit += rune.Utf16SequenceLength;
            index++;
        }

        return unit;
    }

    private static string LMentionTextRead(string text, int start, int end)
    {
        return text[LMentionUnitRead(text, start)..LMentionUnitRead(text, end)];
    }

    private static bool LMentionJoinCheck(Rune left, Rune right, bool separated)
    {
        if (!LMentionWordCheck(left, separated) || !LMentionWordCheck(right, separated))
        {
            return false;
        }

        return separated || LMentionScriptRead(left) == LMentionScriptRead(right);
    }

    private static bool LMentionWordCheck(Rune rune, bool separated)
    {
        if (Rune.IsLetter(rune))
        {
            return true;
        }

        if (!separated)
        {
            return false;
        }

        return Rune.IsDigit(rune) || rune.Value is 0x27 or 0x2019 or 0x2D;
    }

    private static int LMentionScriptRead(Rune rune)
    {
        int value = rune.Value;
        if (value is >= 0x3040 and <= 0x309F)
        {
            return 1;
        }

        if (value is (>= 0x30A0 and <= 0x30FF) or (>= 0x31F0 and <= 0x31FF) or (>= 0xFF66 and <= 0xFF9F))
        {
            return 2;
        }

        if (value is (>= 0x3400 and <= 0x4DBF) or (>= 0x4E00 and <= 0x9FFF) or (>= 0xF900 and <= 0xFAFF)
            or (>= 0x20000 and <= 0x3134F))
        {
            return 3;
        }

        if (value is (>= 0x1100 and <= 0x11FF) or (>= 0x3130 and <= 0x318F) or (>= 0xAC00 and <= 0xD7AF))
        {
            return 4;
        }

        return CharUnicodeInfo.GetUnicodeCategory(value) == UnicodeCategory.OtherLetter ? 5 : 0;
    }
}
