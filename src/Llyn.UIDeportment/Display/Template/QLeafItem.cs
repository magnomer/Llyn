using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLeafItem
{
    private readonly CLeaf _qLeafItemCard;

    internal QLeafItem(CLeaf card, CStateWording peek)
    {
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(peek);

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
        QLeafItemPeek = peek.CStateWordingKey is string glance
            ? QLocalizationCatalog.QLocalizationTextRead(glance)
            : peek.CStateWordingText;
        QLeafItemSituation = card.CLeafSituation.Select(static chip => new QLeafChip(chip)).ToList();
        QLeafItemRegister = card.CLeafRegister.Select(static chip => new QLeafChip(chip)).ToList();
        QLeafItemTag = card.CLeafTag.Select(static chip => new QLeafChip(chip)).ToList();
        QLeafItemTranslation = card.CLeafTranslation.Select(static target => new QLinkChip(
            target.CTranslationTargetId, target.CTranslationTargetHeadword,
            target.CTranslationTargetLanguage)).ToList();
        QLeafItemSentence = QLeafLine.QLeafLineCreate(card.CLeafSentence);
        QLeafItemImage = QLeafImage.QLeafImageCreate(card.CLeafImage);
        QLeafItemVideo = QLeafVideo.QLeafVideoCreate(card.CLeafVideo);
    }

    internal long QLeafItemId => _qLeafItemCard.CLeafId;

    internal string QLeafItemRank => _qLeafItemCard.CLeafPosition.ToString(CultureInfo.CurrentCulture);

    internal string QLeafItemTitle { get; }

    internal bool QLeafItemTitled => !_qLeafItemCard.CLeafTitle.CStateWordingMuted;

    internal string QLeafItemExpression { get; }

    internal bool QLeafItemExpressed => !_qLeafItemCard.CLeafExpression.CStateWordingMuted;

    internal string QLeafItemMeaning { get; }

    internal bool QLeafItemDefined => !_qLeafItemCard.CLeafMeaning.CStateWordingMuted;

    internal string QLeafItemPeek { get; }

    internal bool QLeafItemFolded => _qLeafItemCard.CLeafFolded;

    internal bool QLeafItemStored => _qLeafItemCard.CLeafStored;

    internal IReadOnlyList<QLeafChip> QLeafItemSituation { get; }

    internal IReadOnlyList<QLeafChip> QLeafItemRegister { get; }

    internal IReadOnlyList<QLeafChip> QLeafItemTag { get; }

    internal IReadOnlyList<QLinkChip> QLeafItemTranslation { get; }

    internal IReadOnlyList<QLeafLine> QLeafItemSentence { get; }

    internal IReadOnlyList<QLeafImage> QLeafItemImage { get; }

    internal IReadOnlyList<QLeafVideo> QLeafItemVideo { get; }
}
