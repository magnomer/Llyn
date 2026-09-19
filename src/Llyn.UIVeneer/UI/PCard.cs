using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

internal sealed partial class PCard : INotifyPropertyChanged
{
    private readonly LEngine _pCardEngine;
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
        LEngine engine,
        string prefix,
        int position,
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _pCardEngine = engine;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
