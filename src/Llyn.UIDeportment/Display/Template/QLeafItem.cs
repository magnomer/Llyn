using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
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
        QLeafItemPhrase = card.CLeafPhrase.CStateWordingKey is string phrase
            ? QLocalizationCatalog.QLocalizationTextRead(phrase)
            : card.CLeafPhrase.CStateWordingText;
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
        QLeafItemFlag = card.CLeafFlag.Select(QEnsignImage.QEnsignRead).ToList();
        QLeafItemHome =
            card.CLeafCollocation.Select(static leaf => new QLeafItem(leaf, leaf.CLeafPhrase)).ToList();
        QLeafItemSee = card.CLeafSee.Select(static see => new QLeafSee(see.CLeafSeeId, see.CLeafSeeText)).ToList();
    }

    internal long QLeafItemId => _qLeafItemCard.CLeafId;

    internal string QLeafItemRank => _qLeafItemCard.CLeafPosition.ToString(CultureInfo.CurrentCulture);

    internal string QLeafItemTitle { get; }

    internal bool QLeafItemTitled => !_qLeafItemCard.CLeafTitle.CStateWordingMuted;

    internal string QLeafItemPhrase { get; }

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

    internal IReadOnlyList<ImageSource?> QLeafItemFlag { get; }

    internal string QLeafItemExample => _qLeafItemCard.CLeafExample;

    internal bool QLeafItemQuoted => _qLeafItemCard.CLeafQuoted;

    internal string QLeafItemAlso => _qLeafItemCard.CLeafAlso;

    internal bool QLeafItemShared => _qLeafItemCard.CLeafShared;

    internal string QLeafItemHeading => _qLeafItemCard.CLeafHeading;

    internal bool QLeafItemHomed => _qLeafItemCard.CLeafHomed;

    internal IReadOnlyList<QLeafItem> QLeafItemHome { get; }

    internal IReadOnlyList<QLeafSee> QLeafItemSee { get; }
}
