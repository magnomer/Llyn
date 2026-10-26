using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QPlacard : INotifyPropertyChanged
{
    private readonly string _qPlacardPrefix;
    private readonly Func<QPlacard, CStateWording> _qPlacardPeek;
    private int _qPlacardPosition;
    private bool _qPlacardPositionActive;
    private CStateWording _qPlacardTitle;
    private CStateWording _qPlacardDefinition;
    private CStateWording _qPlacardExpression;
    private CStep? _qPlacardStep;

    internal QPlacard(
        string prefix,
        bool collocated,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<QLanguageItem> languages,
        Func<QPlacard, CStateWording> peek,
        CCardDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _qPlacardPrefix = prefix;
        _qPlacardPeek = peek;
        QPlacardId = draft.CCardDraftId;
        QPlacardCollocated = collocated;
        _qPlacardTitle = draft.CCardDraftTitle;
        _qPlacardDefinition = draft.CCardDraftMeaning;
        _qPlacardExpression = draft.CCardDraftExpression;
        QCardSentence = new QCardSentence(particles, dependences, languages);
    }

    public QCardSentence QCardSentence { get; }

    public ObservableCollection<QImageItem> QPlacardImage { get; } = [];

    public ObservableCollection<QVideoItem> QPlacardVideo { get; } = [];

    public QInlet<QLinkChip> QPlacardLink { get; } = new(
        "Card.TranslationHint",
        static chip => chip.QLinkChipId,
        static (row, fresh) =>
            string.Equals(row.QLinkChipHeadword, fresh.QLinkChipHeadword, StringComparison.Ordinal)
            && string.Equals(row.QLinkChipLanguage, fresh.QLinkChipLanguage, StringComparison.Ordinal)
                ? row
                : fresh);

    public ObservableCollection<QBondItem> QPlacardBond { get; } = [];

    public QInlet<QLabelChip> QPlacardLabel { get; } = new(
        "Card.LabelHint",
        static chip => chip.QLabelChipId,
        static (row, fresh) =>
            string.Equals(row.QLabelChipName, fresh.QLabelChipName, StringComparison.Ordinal) ? row : fresh);

    public QInlet<QContextChip> QPlacardContext { get; } = new(
        "Card.SituationHint",
        static chip => chip.QContextChipId,
        static (row, fresh) => fresh.QContextChipText == row.QContextChipText ? row : fresh);

    public QInlet<QRegisterChip> QPlacardRegister { get; } = new(
        "Card.RegisterHint",
        static chip => chip.QRegisterChipId,
        static (row, fresh) => fresh.QRegisterChipText == row.QRegisterChipText ? row : fresh);

    public long QPlacardId { get; }

    public bool QPlacardCollocated { get; }

    public int QPlacardPosition
    {
        get => _qPlacardPosition;
        set
        {
            if (_qPlacardPosition == value)
            {
                return;
            }

            _qPlacardPosition = value;
            QPlacardRaise(nameof(QPlacardPosition));
            QPlacardRaise(nameof(QPlacardTitle));
        }
    }

    public string QPlacardPositionText => _qPlacardPosition.ToString(CultureInfo.InvariantCulture);

    public bool QPlacardPositionActive
    {
        get => _qPlacardPositionActive;
        set
        {
            if (_qPlacardPositionActive == value)
            {
                return;
            }

            _qPlacardPositionActive = value;
            QPlacardRaise(nameof(QPlacardPositionActive));
        }
    }

    internal void QPlacardPositionHide()
    {
        QPlacardPositionActive = false;
    }

    public string QPlacardTitle => $"{_qPlacardPrefix} {_qPlacardPosition}";

    public CStateWording PTitle => _qPlacardTitle;

    public CStateWording QPlacardDefinition => _qPlacardDefinition;

    public CStateWording QPlacardExpression => _qPlacardExpression;

    public bool QPlacardFolded => _qPlacardStep?.CStepCard?.CCardDraftFolded ?? false;

    public bool QPlacardStored => _qPlacardStep?.CStepFold is not null;

    internal CStep? QPlacardStep
    {
        get => _qPlacardStep;
        set
        {
            _qPlacardStep = value;
            QPlacardRaise(nameof(QPlacardFolded));
        }
    }

    public string QPlacardPeek => _qPlacardPeek(this).CStateWordingText;

    internal void QPlacardTitleShow(CStateWording value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_qPlacardTitle == value)
        {
            return;
        }

        _qPlacardTitle = value;
        QPlacardRaise(nameof(PTitle));
    }

    internal void QPlacardDefinitionShow(CStateWording value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_qPlacardDefinition == value)
        {
            return;
        }

        _qPlacardDefinition = value;
        QPlacardRaise(nameof(QPlacardDefinition));
    }

    internal void QPlacardExpressionShow(CStateWording value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_qPlacardExpression == value)
        {
            return;
        }

        _qPlacardExpression = value;
        QPlacardRaise(nameof(QPlacardExpression));
    }

    internal void QPlacardImageShow(IReadOnlyList<CImageDraft> rows)
    {
        QLookItem.QLookItemShow(
            QPlacardImage,
            rows,
            static row => row.QImageItemId,
            static draft => draft.CImageDraftId,
            draft => new QImageItem(draft, QPlacardCollocated, QPlacardId),
            (row, draft) =>
            {
                row.QImageItemShow(draft);
                return row;
            });
    }

    internal void QPlacardVideoShow(IReadOnlyList<CVideoDraft> rows)
    {
        QLookItem.QLookItemShow(
            QPlacardVideo,
            rows,
            static row => row.QVideoItemId,
            static draft => draft.CVideoDraftId,
            draft => new QVideoItem(draft, QPlacardCollocated, QPlacardId),
            (row, draft) =>
            {
                row.QVideoItemShow(draft);
                return row;
            });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void QPlacardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
