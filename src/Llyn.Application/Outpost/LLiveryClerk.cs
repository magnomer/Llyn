using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LLiveryClerk
{
    public static LLiveryPage LLiveryClerkBuild(
        LEntryDraft draft,
        bool favorite,
        int grasp,
        string created,
        string updated,
        LAccentSheet accent,
        IReadOnlyList<LReflexGuise> guise,
        IReadOnlyList<string> folded,
        IReadOnlySet<long> fold,
        IReadOnlyList<LTranscriptionDraft> transcription,
        LGlyph? glyph,
        IReadOnlyList<LGlyphCell> cell,
        IReadOnlyList<LFrequency> frequency,
        IReadOnlyList<LParadigmRow> paradigm,
        LParadigmView? inflection,
        IReadOnlyList<LFanqieGroup> fanqie,
        IReadOnlyList<LScriptGroup> script,
        IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> target,
        IReadOnlyDictionary<long, string> source,
        IReadOnlyList<LUsage> incoming,
        IReadOnlyList<LTranslationTarget> etymon,
        Func<string, string?> flag,
        IReadOnlyDictionary<string, string> ensign)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(incoming);
        ArgumentNullException.ThrowIfNull(etymon);
        ArgumentNullException.ThrowIfNull(flag);

        List<string> named = [draft.LEntryDraftLanguage];
        named.AddRange(draft.LEntryDraftReflexes.Select(static reflex => reflex.LReflexDraftLanguage));
        named.AddRange(target.Values.SelectMany(static rows => rows)
            .Select(static row => row.LTranslationTargetLanguage));
        named.AddRange(incoming.Select(static usage => usage.LUsageLanguage));
        named.AddRange(etymon.Select(static row => row.LTranslationTargetLanguage));
        Stack<LCardDraft> cards =
            new Stack<LCardDraft>(draft.LEntryDraftMeanings.Concat(draft.LEntryDraftCollocations));
        while (cards.TryPop(out LCardDraft? card))
        {
            named.AddRange(card.LCardDraftSentence.SelectMany(static sentence => sentence.LSentenceDraftGloss)
                .Select(static gloss => gloss.LGlossDraftLanguage));
            foreach (LCardDraft child in card.LCardDraftChild)
            {
                cards.Push(child);
            }
        }

        Dictionary<string, string> banner = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string language in named.Distinct(StringComparer.Ordinal))
        {
            if (flag(language) is string path)
            {
                banner[language] = path;
            }
        }

        return new LLiveryPage(
            draft, favorite, grasp, created, updated, accent, guise, folded, fold, transcription, glyph, cell,
            frequency, paradigm, inflection, fanqie, script, target, source, incoming, etymon, banner, ensign);
    }

    public static LLiveryLanguage LLiveryClerkBuild(
        string language,
        LStemClerk? stem,
        LDiweiClerk? diwei,
        bool switched,
        bool tallied,
        Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(localize);

        List<LLiveryStem> series = [];
        if (stem is not null)
        {
            foreach (LStem row in stem.LStemClerkFind(language, string.Empty, LCatalogOrder.LCatalogOrderName))
            {
                series.Add(new LLiveryStem(
                    stem.LStemPageRead(row, false),
                    stem.LStemEntryScan(language, [row.LStemId], string.Empty, LCatalogOrder.LCatalogOrderHeadword)));
            }
        }

        List<LLiveryDiwei> categories = [];
        if (diwei is not null)
        {
            foreach (string kind in (string[])[LDiwei.LDiweiInitial, LDiwei.LDiweiRime, LDiwei.LDiweiTone])
            {
                foreach (LDiwei row in diwei.LDiweiClerkFind(
                    language, kind, string.Empty, LCatalogOrder.LCatalogOrderName))
                {
                    categories.Add(new LLiveryDiwei(
                        kind,
                        diwei.LDiweiPageRead(row, switched, tallied, localize),
                        diwei.LDiweiEntryScan(
                            language, [row.LDiweiId], string.Empty, LCatalogOrder.LCatalogOrderHeadword)));
                }
            }
        }

        return new LLiveryLanguage(language, series, categories);
    }
}
