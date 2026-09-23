using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMentionClerk
{
    private const int LMentionClerkReach = 8;

    private readonly LEntryVault _lMentionClerkEntries;
    private readonly LExampleVault _lMentionClerkExamples;
    private readonly LMeaningVault _lMentionClerkMeanings;
    private readonly LTranslationVault _lMentionClerkTranslations;
    private readonly LLanguageCache _lMentionClerkLanguages;

    public LMentionClerk(LRig rig, LLanguageCache languages)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        _lMentionClerkEntries = rig.LRigEntries;
        _lMentionClerkExamples = rig.LRigExamples;
        _lMentionClerkMeanings = rig.LRigMeanings;
        _lMentionClerkTranslations = rig.LRigTranslations;
        _lMentionClerkLanguages = languages;
    }

    public LMentionResult LMentionClerkFind(long exampleId, int offset)
    {
        LExample stored = _lMentionClerkExamples.LExampleRead(exampleId)
            ?? throw new LRefusal(LRefusal.LRefusalExample);

        return LMentionClerkFind(
            stored.LExampleText.LStateValueShow(), stored.LExampleLanguage, offset, stored.LExampleMention);
    }

    public LMentionResult LMentionClerkFind(
        string text, string language, int offset, IReadOnlyList<LMention> mentions)
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

        bool separated = language.Length == 0
            || _lMentionClerkLanguages.LLanguageCacheRead(language).LLanguageSeparated;
        List<Rune> runes = LMentionRuneRead(text);

        (int start, int length) = separated
            ? (offset, 0)
            : LMentionClerkScan(_lMentionClerkEntries, runes, language, offset);

        if (length == 0)
        {
            (start, length) = LMentionSpan.LMentionSpanResolve(text, offset, separated);
        }

        if (length == 0)
        {
            return new LMentionResult(start, 0, null, []);
        }

        string word = LMentionRuneFormat(runes, start, length);
        List<LTranslationTarget> found = [];
        foreach (LEntry entry in _lMentionClerkEntries.LEntryHeadwordFind(language, word))
        {
            found.Add(new LTranslationTarget(entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        return new LMentionResult(start, length, null, found);
    }

    public static IReadOnlyList<LMentionPiece> LMentionClerkDivide(string text, IReadOnlyList<LMention> mentions)
    {
        return LMentionSpan.LMentionSpanDivide(text, mentions);
    }

    public static int LMentionUnitRead(string text, int offset)
    {
        return LMentionSpan.LMentionUnitRead(text, offset);
    }

    public static int LMentionOffsetRead(string text, int unit)
    {
        return LMentionSpan.LMentionOffsetRead(text, unit);
    }

    public static LMentionDraft LMentionSpanRead(string text, int start, int length)
    {
        return LMentionSpan.LMentionSpanRead(text, start, length);
    }

    public IReadOnlyList<LMentionLabel> LMentionClerkResolve(string text, IReadOnlyList<LMentionDraft> mentions)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mentions);

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
            foreach (LTranslationTarget target in _lMentionClerkTranslations.LTranslationTargetRead(entries))
            {
                headwords[target.LTranslationTargetId] = target.LTranslationTargetHeadword;
            }
        }

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
                    held = LMentionSenseRead(mention.LMentionDraftSense);
                    senses[mention.LMentionDraftSense] = held;
                }

                sense = held;
            }

            labels.Add(new LMentionLabel(mention.LMentionDraftId, word, mention.LMentionDraftEntry, name, sense));
        }

        return labels;
    }

    private string LMentionSenseRead(long sense)
    {
        LMeaning? meaning = _lMentionClerkMeanings.LMeaningSingleRead(sense);
        if (meaning is null)
        {
            return string.Empty;
        }

        string title = meaning.LMeaningTitle.LStateValueShow();
        return title.Length > 0 ? title : meaning.LMeaningDefinition.LStateValueShow();
    }

    private static (int LMentionClerkOffset, int LMentionClerkLength) LMentionClerkScan(
        LEntryVault entries, List<Rune> runes, string language, int offset)
    {
        if (offset >= runes.Count)
        {
            return (offset, 0);
        }

        int first = Math.Max(0, offset - LMentionClerkReach);
        int last = Math.Min(runes.Count, offset + 1 + LMentionClerkReach * 2);
        string window = LMentionRuneFormat(runes, first, last - first).ToLowerInvariant();

        int bestStart = offset;
        int bestLength = 0;
        foreach (LEntry entry in entries.LEntryHeadwordScan(language, window))
        {
            List<Rune> headword = LMentionRuneRead(entry.LEntryHeadword.ToLowerInvariant());
            if (headword.Count <= bestLength)
            {
                continue;
            }

            for (int start = Math.Max(first, offset - headword.Count + 1); start <= offset; start++)
            {
                if (start + headword.Count <= runes.Count && LMentionRuneMatch(runes, start, headword))
                {
                    bestStart = start;
                    bestLength = headword.Count;
                    break;
                }
            }
        }

        return (bestStart, bestLength);
    }

    private static bool LMentionRuneMatch(List<Rune> runes, int start, List<Rune> headword)
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

    private static string LMentionRuneFormat(List<Rune> runes, int start, int length)
    {
        StringBuilder text = new();
        for (int index = start; index < start + length && index < runes.Count; index++)
        {
            text.Append(runes[index].ToString());
        }

        return text.ToString();
    }

    public static List<Rune> LMentionRuneRead(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<Rune> runes = [];
        foreach (Rune rune in text.EnumerateRunes())
        {
            runes.Add(rune);
        }

        return runes;
    }
}
