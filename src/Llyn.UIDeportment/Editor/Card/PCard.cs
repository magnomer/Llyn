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
    private readonly CAtelier _pCardAtelier;
    private readonly string _pCardPrefix;
    private readonly ObservableCollection<QCitationItem> _pCardCitation;
    private readonly ObservableCollection<string> _pCardParticle;
    private readonly ObservableCollection<string> _pCardDependence;
    private readonly ObservableCollection<PLanguageItem> _pCardLanguage;
    private int _pCardPosition;
    private string _pCardPositionText;
    private bool _pCardPositionActive;
    private CStateValue _pTitle = CStateValue.CStateValueEmpty;
    private CStateValue _pCardDefinition = CStateValue.CStateValueEmpty;
    private CStateValue _pCardExpression = CStateValue.CStateValueEmpty;

    internal PCard(
        CAtelier atelier,
        string prefix,
        int position,
        ObservableCollection<QCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _pCardAtelier = atelier;
        _pCardPrefix = prefix;
        _pCardPosition = position;
        _pCardPositionText = position.ToString(CultureInfo.InvariantCulture);
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
            PCardPositionText = value.ToString(CultureInfo.InvariantCulture);
            PCardRaise(nameof(PCardPosition));
            PCardRaise(nameof(PCardTitle));
        }
    }

    public string PCardPositionText
    {
        get => _pCardPositionText;
        set
        {
            if (string.Equals(_pCardPositionText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardPositionText = value;
            PCardRaise(nameof(PCardPositionText));
        }
    }

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
        PCardPositionText = _pCardPosition.ToString(CultureInfo.InvariantCulture);
    }

    public string PCardTitle => $"{_pCardPrefix} {_pCardPosition}";

    public CStateValue PTitle => _pTitle;

    public CStateValue PCardDefinition => _pCardDefinition;

    public CStateValue PCardExpression => _pCardExpression;

    internal void PCardTitleShow(CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pTitle == value)
        {
            return;
        }

        _pTitle = value;
        PCardRaise(nameof(PTitle));
    }

    internal void PCardDefinitionShow(CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pCardDefinition == value)
        {
            return;
        }

        _pCardDefinition = value;
        PCardRaise(nameof(PCardDefinition));
    }

    internal void PCardExpressionShow(CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pCardExpression == value)
        {
            return;
        }

        _pCardExpression = value;
        PCardRaise(nameof(PCardExpression));
    }

    internal static void PCardRowApply(FrameworkElement container, PCard card, string hint, string? changed)
    {
        ArgumentNullException.ThrowIfNull(card);

        QStateConverter state = new();
        CultureInfo culture = CultureInfo.CurrentCulture;
        string unknown = QLocalizationCatalog.QLocalizationTextRead("Display.Unknown");
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
            if (changed is null or nameof(PCardPositionText))
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
                title.Text = (string)state.Convert(card.PTitle, typeof(string), string.Empty, culture);
            }

            title.SetValue(
                QField.QFieldHintProperty,
                state.Convert([card.PTitle, card.PCardTitle], typeof(string), string.Empty, culture));
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardExpression") is TextBox expression)
        {
            if (changed is null or nameof(PCardExpression))
            {
                expression.Text = (string)state.Convert(card.PCardExpression, typeof(string), string.Empty, culture);
            }

            expression.SetValue(QField.QFieldHintProperty, state.Convert(
                [card.PCardExpression, unknown, QLocalizationCatalog.QLocalizationTextRead("Card.ExpressionHint")],
                typeof(string),
                string.Empty,
                culture));
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardDefinition") is TextBox definition)
        {
            if (changed is null or nameof(PCardDefinition))
            {
                definition.Text = (string)state.Convert(card.PCardDefinition, typeof(string), string.Empty, culture);
            }

            definition.SetValue(QField.QFieldHintProperty, state.Convert(
                [card.PCardDefinition, unknown, QLocalizationCatalog.QLocalizationTextRead(hint)],
                typeof(string),
                string.Empty,
                culture));
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
