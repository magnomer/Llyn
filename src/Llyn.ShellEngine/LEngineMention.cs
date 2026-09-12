using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private const int LEngineMentionReach = 8;

    public LMentionResult LEngineMentionFind(long exampleId, int offset)
    {
        lock (_lEngineGate)
        {
            LExample stored = new LExampleArchive(_lEngineDatabase).LExampleRead(exampleId)
                ?? throw new LRefusal(LRefusal.LRefusalExample);

            return LEngineMentionFind(
                stored.LExampleText.LStateValueShow(), stored.LExampleLanguage, offset, stored.LExampleMention);
        }
    }

    public LMentionResult LEngineMentionFind(
        string text, string language, int offset, IReadOnlyList<LMention> mentions)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(language);
            ArgumentNullException.ThrowIfNull(mentions);
            ArgumentOutOfRangeException.ThrowIfNegative(offset);

            foreach (LMention mention in mentions)
            {
                if (offset >= mention.LMentionOffset && offset < mention.LMentionOffset + mention.LMentionLength)
                {
                    return new LMentionResult(mention.LMentionOffset, mention.LMentionLength, mention, []);
                }
            }

            LEntryArchive entries = new(_lEngineDatabase);
            bool separated = language.Length == 0 || LEngineLanguageLoad(language).LLanguageSeparated;
            List<Rune> runes = LEngineRuneRead(text);

            (int start, int length) = separated
                ? (offset, 0)
                : LEngineMentionScan(entries, runes, language, offset);

            if (length == 0)
            {
                (start, length) = LMentionSpan.LMentionSpanResolve(text, offset, separated);
            }

            if (length == 0)
            {
                return new LMentionResult(start, 0, null, []);
            }

            string word = LEngineRuneFormat(runes, start, length);
            List<LTranslationTarget> found = [];
            foreach (LEntry entry in entries.LEntryHeadwordFind(language, word))
            {
                found.Add(new LTranslationTarget(entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
            }

            return new LMentionResult(start, length, null, found);
        }
    }

    private static (int LEngineMentionOffset, int LEngineMentionLength) LEngineMentionScan(
        LEntryArchive entries, List<Rune> runes, string language, int offset)
    {
        if (offset >= runes.Count)
        {
            return (offset, 0);
        }

        int first = Math.Max(0, offset - LEngineMentionReach);
        int last = Math.Min(runes.Count, offset + 1 + LEngineMentionReach * 2);
        string window = LEngineRuneFormat(runes, first, last - first).ToLowerInvariant();

        int bestStart = offset;
        int bestLength = 0;
        foreach (LEntry entry in entries.LEntryHeadwordScan(language, window))
        {
            List<Rune> headword = LEngineRuneRead(entry.LEntryHeadword.ToLowerInvariant());
            if (headword.Count <= bestLength)
            {
                continue;
            }

            for (int start = Math.Max(first, offset - headword.Count + 1); start <= offset; start++)
            {
                if (start + headword.Count <= runes.Count && LEngineRuneMatch(runes, start, headword))
                {
                    bestStart = start;
                    bestLength = headword.Count;
                    break;
                }
            }
        }

        return (bestStart, bestLength);
    }

    private static bool LEngineRuneMatch(List<Rune> runes, int start, List<Rune> headword)
    {
        for (int index = 0; index < headword.Count; index++)
        {
            if (Rune.ToLowerInvariant(runes[start + index]) != headword[index])
            {
                return false;
            }
        }

        return true;
    }

    private static string LEngineRuneFormat(List<Rune> runes, int start, int length)
    {
        StringBuilder text = new();
        for (int index = start; index < start + length && index < runes.Count; index++)
        {
            text.Append(runes[index].ToString());
        }

        return text.ToString();
    }
}
