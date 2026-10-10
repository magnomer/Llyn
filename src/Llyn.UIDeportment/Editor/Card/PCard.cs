using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PCard : INotifyPropertyChanged
{
    private readonly string _pCardPrefix;
    private readonly Func<PCard, CStateWording> _pCardPeek;
    private int _pCardPosition;
    private bool _pCardPositionActive;
    private CStateWording _pTitle;
    private CStateWording _pCardDefinition;
    private CStateWording _pCardExpression;
    private bool _pCardFolded;

    internal PCard(
        string prefix,
        ObservableCollection<QCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages,
        Func<PCard, CStateWording> peek,
        CCardDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _pCardPrefix = prefix;
        _pCardPeek = peek;
        PCardId = draft.CCardDraftId;
        _pTitle = draft.CCardDraftTitle;
        _pCardDefinition = draft.CCardDraftMeaning;
        _pCardExpression = draft.CCardDraftExpression;
        PCardStored = draft.CCardDraftStored;
        PCardSentence = new PCardSentence(catalog, particles, dependences, languages);
    }

    public PCardSentence PCardSentence { get; }

    public ObservableCollection<QImageItem> PCardImage { get; } = [];

    public ObservableCollection<QVideoItem> PCardVideo { get; } = [];

    public PCaret<QLinkChip> PCardLink { get; } = new(
        "Card.TranslationHint",
        static chip => chip.QLinkChipId,
        static (row, fresh) =>
            string.Equals(row.QLinkChipHeadword, fresh.QLinkChipHeadword, StringComparison.Ordinal)
            && string.Equals(row.QLinkChipLanguage, fresh.QLinkChipLanguage, StringComparison.Ordinal)
                ? row
                : fresh);

    public PCaret<PLabelChip> PCardLabel { get; } = new(
        "Card.LabelHint",
        static chip => chip.PLabelChipId,
        static (row, fresh) =>
            string.Equals(row.PLabelChipName, fresh.PLabelChipName, StringComparison.Ordinal) ? row : fresh);

    public PCaret<PContext> PCardContext { get; } = new(
        "Card.SituationHint",
        static chip => chip.PContextId,
        static (row, fresh) => fresh.PContextText == row.PContextText ? row : fresh);

    public PCaret<PRegister> PCardRegister { get; } = new(
        "Card.RegisterHint",
        static chip => chip.PRegisterId,
        static (row, fresh) => fresh.PRegisterText == row.PRegisterText ? row : fresh);

    public long PCardId { get; }

    public int PCardPosition
    {
        get => _pCardPosition;
        set
        {
            if (_pCardPosition == value)
            {
                return;
            }

            _pCardPosition = value;
            PCardRaise(nameof(PCardPosition));
            PCardRaise(nameof(PCardTitle));
        }
    }

    public string PCardPositionText => _pCardPosition.ToString(CultureInfo.InvariantCulture);

    public bool PCardPositionActive
    {
        get => _pCardPositionActive;
        set
        {
            if (_pCardPositionActive == value)
            {
                return;
            }

            _pCardPositionActive = value;
            PCardRaise(nameof(PCardPositionActive));
        }
    }

    internal void PCardPositionHide()
    {
        PCardPositionActive = false;
    }

    public string PCardTitle => $"{_pCardPrefix} {_pCardPosition}";

    public CStateWording PTitle => _pTitle;

    public CStateWording PCardDefinition => _pCardDefinition;

    public CStateWording PCardExpression => _pCardExpression;

    public bool PCardFolded => _pCardFolded;

    public bool PCardStored { get; }

    public string PCardPeek => _pCardPeek(this).CStateWordingText;

    internal void PCardTitleShow(CStateWording value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pTitle == value)
        {
            return;
        }

        _pTitle = value;
        PCardRaise(nameof(PTitle));
    }

    internal void PCardDefinitionShow(CStateWording value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pCardDefinition == value)
        {
            return;
        }

        _pCardDefinition = value;
        PCardRaise(nameof(PCardDefinition));
    }

    internal void PCardExpressionShow(CStateWording value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pCardExpression == value)
        {
            return;
        }

        _pCardExpression = value;
        PCardRaise(nameof(PCardExpression));
    }

    internal void PCardFoldShow(bool folded)
    {
        if (_pCardFolded == folded)
        {
            return;
        }

        _pCardFolded = folded;
        PCardRaise(nameof(PCardFolded));
    }

    internal void PCardImageShow(IReadOnlyList<CImageDraft> rows)
    {
        QLookItem.QLookItemShow(
            PCardImage,
            rows,
            static row => row.QImageItemId,
            static draft => draft.CImageDraftId,
            static draft => new QImageItem(draft),
            (row, draft) =>
            {
                row.QImageItemShow(draft);
                return row;
            });
    }

    internal void PCardVideoShow(IReadOnlyList<CVideoDraft> rows)
    {
        QLookItem.QLookItemShow(
            PCardVideo,
            rows,
            static row => row.QVideoItemId,
            static draft => draft.CVideoDraftId,
            static draft => new QVideoItem(draft),
            (row, draft) =>
            {
                row.QVideoItemShow(draft);
                return row;
            });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
