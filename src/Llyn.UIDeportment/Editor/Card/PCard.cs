using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed partial class PCard : INotifyPropertyChanged
{
    private readonly LWindow _pCardWindow;
    private readonly string _pCardPrefix;
    private readonly ObservableCollection<PCitationItem> _pCardCitation;
    private readonly ObservableCollection<string> _pCardParticle;
    private readonly ObservableCollection<string> _pCardDependence;
    private readonly ObservableCollection<PLanguageItem> _pCardLanguage;
    private int _pCardPosition;
    private string _pCardPositionText;
    private bool _pCardPositionActive;
    private LStateValue _pTitle = LStateValue.LStateValueUnspecified;
    private LStateValue _pCardDefinition = LStateValue.LStateValueUnspecified;
    private LStateValue _pCardExpression = LStateValue.LStateValueUnspecified;

    internal PCard(
        LWindow window,
        string prefix,
        int position,
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages)
    {
        ArgumentNullException.ThrowIfNull(window);

        _pCardWindow = window;
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

    public LStateValue PTitle => _pTitle;

    public LStateValue PCardDefinition => _pCardDefinition;

    public LStateValue PCardExpression => _pCardExpression;

    internal void PCardTitleShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pTitle.LStateValueMatch(value))
        {
            return;
        }

        _pTitle = value;
        PCardRaise(nameof(PTitle));
    }

    internal void PCardDefinitionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pCardDefinition.LStateValueMatch(value))
        {
            return;
        }

        _pCardDefinition = value;
        PCardRaise(nameof(PCardDefinition));
    }

    internal void PCardExpressionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (_pCardExpression.LStateValueMatch(value))
        {
            return;
        }

        _pCardExpression = value;
        PCardRaise(nameof(PCardExpression));
    }

    internal static void PCardRowApply(FrameworkElement container, PCard card, string hint, string? changed)
    {
        ArgumentNullException.ThrowIfNull(card);

        PStateConverter state = new();
        CultureInfo culture = CultureInfo.CurrentCulture;
        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        if (PLook.PLookPartFind<Border>(container, "PCardPosition") is Border position)
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

        if (PLook.PLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
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

        if (PLook.PLookPartFind<TextBox>(container, "PTitle") is TextBox title)
        {
            if (changed is null or nameof(PTitle))
            {
                title.Text = (string)state.Convert(card.PTitle, typeof(string), string.Empty, culture);
            }

            title.Tag = state.Convert([card.PTitle, card.PCardTitle], typeof(string), string.Empty, culture);
        }

        if (PLook.PLookPartFind<TextBox>(container, "PCardExpression") is TextBox expression)
        {
            if (changed is null or nameof(PCardExpression))
            {
                expression.Text = (string)state.Convert(card.PCardExpression, typeof(string), string.Empty, culture);
            }

            expression.Tag = state.Convert(
                [card.PCardExpression, unknown, PLocalizationCatalog.PLocalizationTextRead("Card.ExpressionHint")],
                typeof(string),
                string.Empty,
                culture);
        }

        if (PLook.PLookPartFind<TextBox>(container, "PCardDefinition") is TextBox definition)
        {
            if (changed is null or nameof(PCardDefinition))
            {
                definition.Text = (string)state.Convert(card.PCardDefinition, typeof(string), string.Empty, culture);
            }

            definition.Tag = state.Convert(
                [card.PCardDefinition, unknown, PLocalizationCatalog.PLocalizationTextRead(hint)],
                typeof(string),
                string.Empty,
                culture);
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PCardIcon") is PIconImage icon)
        {
            icon.PIconSource = PIcon.PIconResolve("close", 12);
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PCardImageIcon") is PIconImage image)
        {
            image.PIconSource = PIcon.PIconResolve("image", 24);
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PCardVideoIcon") is PIconImage video)
        {
            video.PIconSource = PIcon.PIconResolve("video", 24);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
