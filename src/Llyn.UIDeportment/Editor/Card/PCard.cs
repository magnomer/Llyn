using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard : INotifyPropertyChanged
{
    private readonly string _pCardPrefix;
    private readonly ObservableCollection<QCitationItem> _pCardCitation;
    private readonly ObservableCollection<string> _pCardParticle;
    private readonly ObservableCollection<string> _pCardDependence;
    private readonly ObservableCollection<PLanguageItem> _pCardLanguage;
    private int _pCardPosition;
    private bool _pCardPositionActive;
    private CStateWording _pTitle;
    private CStateWording _pCardDefinition;
    private CStateWording _pCardExpression;

    internal PCard(
        string prefix,
        ObservableCollection<QCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages,
        CCardDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _pCardPrefix = prefix;
        _pTitle = draft.CCardDraftTitle;
        _pCardDefinition = draft.CCardDraftMeaning;
        _pCardExpression = draft.CCardDraftExpression;
        _pCardCitation = catalog;
        _pCardParticle = particles;
        _pCardDependence = dependences;
        _pCardLanguage = languages;
        PCardContextStart();
        PCardRegisterStart();
        PCardLinkStart();
        PCardLabelStart();
    }

    public long PCardId { get; set; }

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

    internal static void PCardRowApply(FrameworkElement container, PCard card, string? changed)
    {
        ArgumentNullException.ThrowIfNull(card);

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            if (card.PCardPositionActive)
            {
                position.SetResourceReference(Border.BorderBrushProperty, "Theme.Accent");
            }
            else
            {
                position.ClearValue(Border.BorderBrushProperty);
            }
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            if (changed is null or nameof(PCardPosition) or nameof(PCardPositionActive))
            {
                ordinal.Text = card.PCardPositionText;
            }

            if (card.PCardPositionActive)
            {
                ordinal.IsReadOnly = false;
                ordinal.IsHitTestVisible = true;
            }
            else
            {
                ordinal.ClearValue(TextBoxBase.IsReadOnlyProperty);
                ordinal.ClearValue(UIElement.IsHitTestVisibleProperty);
            }
        }

        if (QLook.QLookPartFind<TextBox>(container, "PTitle") is TextBox title)
        {
            if (changed is null or nameof(PTitle))
            {
                title.Text = card.PTitle.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(title, QField.QFieldHintProperty, card.PTitle);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardExpression") is TextBox expression)
        {
            if (changed is null or nameof(PCardExpression))
            {
                expression.Text = card.PCardExpression.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(expression, QField.QFieldHintProperty, card.PCardExpression);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardDefinition") is TextBox definition)
        {
            if (changed is null or nameof(PCardDefinition))
            {
                definition.Text = card.PCardDefinition.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(definition, QField.QFieldHintProperty, card.PCardDefinition);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCardIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("close", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCardImageIcon") is QIconImage image)
        {
            image.QIconSource = QIcon.QIconResolve("image", 24);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCardVideoIcon") is QIconImage video)
        {
            video.QIconSource = QIcon.QIconResolve("video", 24);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
