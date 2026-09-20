using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed record PMentionItem(
    long PMentionItemEntry,
    long PMentionItemSense,
    string PMentionItemName,
    string PMentionItemLanguage,
    ImageSource? PMentionItemFlag,
    int PMentionItemDepth)
{
    private const double PMentionItemStep = 16;

    public Thickness PMentionItemIndent => new(PMentionItemDepth * PMentionItemStep, 0, 0, 0);

    internal static IReadOnlyList<PMentionItem> PMentionItemCreate(IReadOnlyList<LTranslationTarget> targets)
    {
        List<PMentionItem> rows = [];
        foreach (LTranslationTarget target in targets)
        {
            rows.Add(new PMentionItem(
                target.LTranslationTargetId,
                0,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage,
                PEnsign.PEnsignFind(target.LTranslationTargetLanguage),
                0));
        }

        return rows;
    }

    internal static IReadOnlyList<PMentionItem> PMentionItemCreate(
        long entryId, IReadOnlyList<LMeaning> meanings, string whole, string unknown)
    {
        List<PMentionItem> rows = [new PMentionItem(entryId, 0, whole, string.Empty, null, 0)];
        PMentionItemAppend(rows, entryId, meanings, 0, 0, unknown);
        return rows;
    }

    private static void PMentionItemAppend(
        List<PMentionItem> rows,
        long entryId,
        IReadOnlyList<LMeaning> meanings,
        long parent,
        int depth,
        string unknown)
    {
        List<LMeaning> children = [];
        foreach (LMeaning meaning in meanings)
        {
            if ((meaning.LMeaningParentId ?? 0) == parent && meaning.LMeaningId != parent)
            {
                children.Add(meaning);
            }
        }

        children.Sort((left, right) => left.LMeaningPosition.CompareTo(right.LMeaningPosition));
        foreach (LMeaning meaning in children)
        {
            string name = meaning.LMeaningName.Length > 0 ? meaning.LMeaningName : unknown;

            rows.Add(new PMentionItem(entryId, meaning.LMeaningId, name, string.Empty, null, depth));
            PMentionItemAppend(rows, entryId, meanings, meaning.LMeaningId, depth + 1, unknown);
        }
    }
}
