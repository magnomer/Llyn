using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMarkupLoss(
    string LMarkupLossEntry,
    string LMarkupLossKind,
    string LMarkupLossTarget)
{
    public static IReadOnlyList<LMarkupLoss> LMarkupLossRead(
        LMarkup.LMarkupDocument document, IReadOnlyDictionary<string, string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        List<LMarkupLoss> lost = new List<LMarkupLoss>();
        foreach (LMarkup.LMarkupEntry entry in document.LMarkupDocumentEntry)
        {
            string headword = entry.LMarkupEntryDraft.LEntryDraftHeadword;
            LMarkupLossRead(lost, headword, entry.LMarkupEntryDraft.LEntryDraftMeanings, keys);
            LMarkupLossRead(lost, headword, entry.LMarkupEntryDraft.LEntryDraftCollocations, keys);
        }

        return lost;
    }

    private static void LMarkupLossRead(
        List<LMarkupLoss> lost,
        string headword,
        IReadOnlyList<LCardDraft> cards,
        IReadOnlyDictionary<string, string> keys)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (string translation in card.LCardDraftTranslation)
            {
                LMarkupLossRead(lost, headword, "translation", translation, keys);
            }

            foreach (LRelationDraft relation in card.LCardDraftRelation)
            {
                LMarkupLossRead(lost, headword, "relation", LMarkupLossRead(
                    relation.LRelationDraftEntry, relation.LRelationDraftMeaning), keys);
            }

            foreach (LSynonymDraft interlink in card.LCardDraftInterlink)
            {
                LMarkupLossRead(lost, headword, "synonym", LMarkupLossRead(
                    interlink.LSynonymDraftEntry, interlink.LSynonymDraftMeaning), keys);
            }

            LMarkupLossRead(lost, headword, card.LCardDraftChild, keys);
        }
    }

    private static void LMarkupLossRead(
        List<LMarkupLoss> lost,
        string headword,
        string kind,
        string row,
        IReadOnlyDictionary<string, string> keys)
    {
        if (row.Length == 0 || keys.ContainsKey(row))
        {
            return;
        }

        lost.Add(new LMarkupLoss(headword, kind, row));
    }

    private static string LMarkupLossRead(string entry, string meaning)
    {
        return entry.Length > 0 ? entry : meaning;
    }
}
