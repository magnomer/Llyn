using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLeafItem
{
    private readonly CLeaf _qLeafItemCard;

    internal QLeafItem(CLeaf card)
    {
        ArgumentNullException.ThrowIfNull(card);

        _qLeafItemCard = card;
        QLeafItemTitle = card.CLeafTitle.CStateWordingKey is string title
            ? QLocalizationCatalog.QLocalizationTextRead(title)
            : card.CLeafTitle.CStateWordingText;
        QLeafItemExpression = card.CLeafExpression.CStateWordingKey is string expression
            ? QLocalizationCatalog.QLocalizationTextRead(expression)
            : card.CLeafExpression.CStateWordingText;
        QLeafItemMeaning = card.CLeafMeaning.CStateWordingKey is string meaning
            ? QLocalizationCatalog.QLocalizationTextRead(meaning)
            : card.CLeafMeaning.CStateWordingText;
        QLeafItemSituation = card.CLeafSituation.Select(static chip => new QLeafChip(chip)).ToList();
        QLeafItemRegister = card.CLeafRegister.Select(static chip => new QLeafChip(chip)).ToList();
        QLeafItemTag = card.CLeafTag.Select(static chip => new QLeafChip(chip)).ToList();
        QLeafItemTranslation = card.CLeafTranslation
            .Select(static target => new LLinkChip(
                target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage))
            .ToList();
        QLeafItemSentence = card.CLeafSentence;
    }

    internal string QLeafItemRank => _qLeafItemCard.CLeafPosition.ToString(CultureInfo.CurrentCulture);

    internal string QLeafItemTitle { get; }

    internal bool QLeafItemTitled => !_qLeafItemCard.CLeafTitle.CStateWordingMuted;

    internal string QLeafItemExpression { get; }

    internal bool QLeafItemExpressed => !_qLeafItemCard.CLeafExpression.CStateWordingMuted;

    internal string QLeafItemMeaning { get; }

    internal bool QLeafItemDefined => !_qLeafItemCard.CLeafMeaning.CStateWordingMuted;

    internal IReadOnlyList<QLeafChip> QLeafItemSituation { get; }

    internal IReadOnlyList<QLeafChip> QLeafItemRegister { get; }

    internal IReadOnlyList<QLeafChip> QLeafItemTag { get; }

    internal IReadOnlyList<LLinkChip> QLeafItemTranslation { get; }

    internal IReadOnlyList<CLeafLine> QLeafItemSentence { get; }

    internal IReadOnlyList<CImageDraft> QLeafItemImage => _qLeafItemCard.CLeafImage;

    internal IReadOnlyList<CVideoDraft> QLeafItemVideo => _qLeafItemCard.CLeafVideo;
}
