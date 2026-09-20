using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private const int LEngineMentionReach = 8;

    public LMentionResult LEngineMentionFind(long exampleId, int offset)
    {
        lock (_lEngineGate)
        {
            LExample stored = _lEngineExamples.LExampleRead(exampleId)
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

            bool separated = language.Length == 0 || LEngineLanguageLoad(language).LLanguageSeparated;
            List<Rune> runes = LEngineRuneRead(text);

            (int start, int length) = separated
                ? (offset, 0)
                : LEngineMentionScan(_lEngineEntries, runes, language, offset);

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
            foreach (LEntry entry in _lEngineEntries.LEntryHeadwordFind(language, word))
            {
                found.Add(new LTranslationTarget(entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
            }

            return new LMentionResult(start, length, null, found);
        }
    }

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mentions);

        lock (_lEngineGate)
        {
            List<long> entries = [];
            foreach (LMentionDraft mention in mentions)
            {
                if (mention.LMentionDraftEntry != 0 && !entries.Contains(mention.LMentionDraftEntry))
                {
                    entries.Add(mention.LMentionDraftEntry);
                }
            }

            Dictionary<long, string> headwords = [];
            if (entries.Count > 0)
            {
                LTranslationVault targets = _lEngineTranslations;
                foreach (LTranslationTarget target in targets.LTranslationTargetRead(entries))
                {
                    headwords[target.LTranslationTargetId] = target.LTranslationTargetHeadword;
                }
            }

            LMeaningVault meanings = _lEngineMeanings;
            Dictionary<long, string> senses = [];
            List<LMentionLabel> labels = new(mentions.Count);
            foreach (LMentionDraft mention in mentions)
            {
                int start = LMentionSpan.LMentionUnitRead(text, mention.LMentionDraftOffset);
                int end = LMentionSpan.LMentionUnitRead(
                    text, mention.LMentionDraftOffset + mention.LMentionDraftLength);
                string word = end > start ? text[start..end] : string.Empty;
                string name = headwords.GetValueOrDefault(mention.LMentionDraftEntry, string.Empty);

                string sense = string.Empty;
                if (mention.LMentionDraftSense != 0)
                {
                    if (!senses.TryGetValue(mention.LMentionDraftSense, out string? held))
                    {
                        held = LEngineSenseRead(meanings, mention.LMentionDraftSense);
                        senses[mention.LMentionDraftSense] = held;
                    }

                    sense = held;
                }

                labels.Add(new LMentionLabel(mention.LMentionDraftId, word, mention.LMentionDraftEntry, name, sense));
            }

            return labels;
        }
    }

    private static string LEngineSenseRead(LMeaningVault meanings, long sense)
    {
        LMeaning? meaning = meanings.LMeaningSingleRead(sense);
        if (meaning is null)
        {
            return string.Empty;
        }

        string title = meaning.LMeaningTitle.LStateValueShow();
        return title.Length > 0 ? title : meaning.LMeaningDefinition.LStateValueShow();
    }

    private static (int LEngineMentionOffset, int LEngineMentionLength) LEngineMentionScan(
        LEntryVault entries, List<Rune> runes, string language, int offset)
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

    private static List<Rune> LEngineRuneRead(string text)
    {
        List<Rune> runes = [];
        foreach (Rune rune in text.EnumerateRunes())
        {
            runes.Add(rune);
        }

        return runes;
    }
}
