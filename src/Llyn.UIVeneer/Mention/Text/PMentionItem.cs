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
        long entryId, IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> senses, string whole)
    {
        List<PMentionItem> rows = [new PMentionItem(entryId, 0, whole, string.Empty, null, 0)];
        foreach ((long id, string name, int depth) in senses)
        {
            rows.Add(new PMentionItem(entryId, id, name, string.Empty, null, depth));
        }

        return rows;
    }
}
