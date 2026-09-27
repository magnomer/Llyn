using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

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
                LEnsignImage.LEnsignFind(target.LTranslationTargetLanguage),
                0));
        }

        return rows;
    }

    internal static IReadOnlyList<PMentionItem> PMentionItemCreate(
        long entryId, IReadOnlyList<CMeaning> senses, string whole)
    {
        List<PMentionItem> rows = [new PMentionItem(entryId, 0, whole, string.Empty, null, 0)];
        foreach (CMeaning sense in senses)
        {
            rows.Add(new PMentionItem(
                entryId, sense.CMeaningId, sense.CMeaningName, string.Empty, null, sense.CMeaningDepth));
        }

        return rows;
    }
}
