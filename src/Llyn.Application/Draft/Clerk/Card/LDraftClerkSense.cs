using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LDraftClerkSense
{
    public static LEntryDraft LCardSenseAttach(LEntryDraft content, long cardId, long senseId)
    {
        ArgumentNullException.ThrowIfNull(content);

        LCardDraft card = content.LEntryDraftCollocations.FirstOrDefault(held => held.LCardDraftId == cardId)
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        if (cardId == 0 || senseId == 0 || !LCardSenseRead(content.LEntryDraftMeanings).Contains(senseId))
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        if (card.LCardDraftSense.Contains(senseId))
        {
            return content;
        }

        return LDraftClerkCard.LCardChange(
            content,
            LCardKind.LCardKindCollocation,
            cardId,
            held => held with { LCardDraftSense = [.. held.LCardDraftSense, senseId] });
    }

    public static LEntryDraft LCardSenseDetach(LEntryDraft content, long cardId, long senseId)
    {
        ArgumentNullException.ThrowIfNull(content);

        LCardDraft card = content.LEntryDraftCollocations.FirstOrDefault(held => held.LCardDraftId == cardId)
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        if (!card.LCardDraftSense.Contains(senseId))
        {
            return content;
        }

        return LDraftClerkCard.LCardChange(
            content,
            LCardKind.LCardKindCollocation,
            cardId,
            held => held with { LCardDraftSense = held.LCardDraftSense.Where(id => id != senseId).ToList() });
    }

    public static LEntryDraft LCardSenseSweep(LEntryDraft content, LCardDraft removed)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(removed);

        HashSet<long> dropped = [.. LCardSenseRead([removed])];
        bool swept = false;
        List<LCardDraft> collocations = new(content.LEntryDraftCollocations.Count);
        foreach (LCardDraft card in content.LEntryDraftCollocations)
        {
            if (!card.LCardDraftSense.Any(dropped.Contains))
            {
                collocations.Add(card);
                continue;
            }

            swept = true;
            collocations.Add(card with
            {
                LCardDraftSense = card.LCardDraftSense.Where(id => !dropped.Contains(id)).ToList(),
            });
        }

        return swept ? content with { LEntryDraftCollocations = collocations } : content;
    }

    public static LCollocationLayout LCollocationLayoutRead(
        LEntryDraft content,
        IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> targets)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(targets);

        IReadOnlyList<LCardDraft> meanings = content.LEntryDraftMeanings;
        Dictionary<long, IReadOnlyList<LCardDraft>> homes = [];
        Dictionary<long, IReadOnlyList<(long, LStateValue, string)>> stubs = [];
        Dictionary<long, LStateValue> phrases = [];
        Dictionary<long, IReadOnlyList<string>> flags = [];
        Dictionary<long, int> examples = [];
        Dictionary<long, IReadOnlyList<string>> alsos = [];
        foreach (LCardDraft collocation in content.LEntryDraftCollocations)
        {
            long id = collocation.LCardDraftId;
            phrases[id] = LCollocation.LCollocationPhraseRead(collocation);
            flags[id] = LCollocation.LCollocationLanguageRead(
                targets.GetValueOrDefault((LCardKind.LCardKindCollocation, id), []));
            examples[id] = LCollocation.LCollocationExampleRead(collocation);
            alsos[id] = LCollocation.LCollocationAlsoRead(meanings, collocation);
            if (LCollocation.LCollocationHomeRead(meanings, collocation, false) is long home)
            {
                homes[home] = [.. homes.GetValueOrDefault(home, []), collocation];
            }

            IReadOnlyList<long> linked = LCollocation.LCollocationStubRead(meanings, collocation, false);
            if (linked.Count == 0)
            {
                continue;
            }

            long nested = LCollocation.LCollocationHomeRead(meanings, collocation, true) ?? 0;
            (long, LStateValue, string) line =
                (id, phrases[id], LMeaning.LMeaningPathRead(meanings, nested) ?? string.Empty);
            foreach (long meaning in linked)
            {
                stubs[meaning] = [.. stubs.GetValueOrDefault(meaning, []), line];
            }
        }

        return new LCollocationLayout(
            homes,
            stubs,
            LCollocation.LCollocationOtherRead(meanings, content.LEntryDraftCollocations),
            phrases,
            flags,
            examples,
            alsos);
    }

    private static IEnumerable<long> LCardSenseRead(IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            yield return card.LCardDraftId;
            foreach (long child in LCardSenseRead(card.LCardDraftChild))
            {
                yield return child;
            }
        }
    }
}
