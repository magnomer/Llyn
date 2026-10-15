using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed record QMentionItem(
    long QMentionItemEntry,
    long QMentionItemSense,
    string QMentionItemName,
    string QMentionItemLanguage,
    ImageSource? QMentionItemFlag,
    int QMentionItemDepth)
{
    private const double QMentionItemStep = 16;

    public Thickness QMentionItemIndent => new(QMentionItemDepth * QMentionItemStep, 0, 0, 0);

    internal static IReadOnlyList<QMentionItem> QMentionItemCreate(IReadOnlyList<CTranslationTarget> targets)
    {
        List<QMentionItem> rows = [];
        foreach (CTranslationTarget target in targets)
        {
            rows.Add(new QMentionItem(
                target.CTranslationTargetId,
                0,
                target.CTranslationTargetHeadword,
                target.CTranslationTargetLanguage,
                QEnsignImage.QEnsignRead(target.CTranslationTargetLanguage),
                0));
        }

        return rows;
    }

    internal static IReadOnlyList<QMentionItem> QMentionItemCreate(IReadOnlyList<CMeaning> senses)
    {
        List<QMentionItem> rows = [];
        foreach (CMeaning sense in senses)
        {
            rows.Add(new QMentionItem(
                0, sense.CMeaningId, sense.CMeaningName, string.Empty, null, sense.CMeaningDepth));
        }

        return rows;
    }
}
