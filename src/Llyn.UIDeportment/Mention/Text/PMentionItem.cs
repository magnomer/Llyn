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

    internal static IReadOnlyList<PMentionItem> PMentionItemCreate(IReadOnlyList<CTranslationTarget> targets)
    {
        List<PMentionItem> rows = [];
        foreach (CTranslationTarget target in targets)
        {
            rows.Add(new PMentionItem(
                target.CTranslationTargetId,
                0,
                target.CTranslationTargetHeadword,
                target.CTranslationTargetLanguage,
                LEnsignImage.LEnsignFind(target.CTranslationTargetLanguage),
                0));
        }

        return rows;
    }

    internal static IReadOnlyList<PMentionItem> PMentionItemCreate(IReadOnlyList<CMeaning> senses, string whole)
    {
        List<PMentionItem> rows = [new PMentionItem(0, 0, whole, string.Empty, null, 0)];
        foreach (CMeaning sense in senses)
        {
            rows.Add(new PMentionItem(
                0, sense.CMeaningId, sense.CMeaningName, string.Empty, null, sense.CMeaningDepth));
        }

        return rows;
    }
}
