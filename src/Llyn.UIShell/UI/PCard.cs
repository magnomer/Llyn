using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard : INotifyPropertyChanged
{
    internal const string PCardUnreadableMark = "(?)";

    private readonly string _pCardPrefix;
    private readonly ObservableCollection<PCitationItem> _pCardCitation;
    private readonly ObservableCollection<string> _pCardParticle;
    private readonly ObservableCollection<string> _pCardDependence;
    private int _pCardPosition;
    private string _pCardPositionText;
    private bool _pCardPositionActive;
    private string _pTitle;
    private bool _pTitleUnreadable;
    private string _pCardDefinition;
    private bool _pCardDefinitionUnreadable;
    private string _pCardExpression;
    private bool _pCardExpressionUnreadable;

    internal PCard(
        string prefix,
        int position,
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences)
    {
        _pCardPrefix = prefix;
        _pCardPosition = position;
        _pCardPositionText = position.ToString(CultureInfo.InvariantCulture);
        _pCardCitation = catalog;
        _pCardParticle = particles;
        _pCardDependence = dependences;
        _pTitle = string.Empty;
        _pCardDefinition = string.Empty;
        _pCardExpression = string.Empty;
        PCardSentence = [];
        PCardSentenceAdd(new PSentence(catalog, particles, dependences));
        PCardContextStart();
        PCardRegisterStart();
        PCardLinkStart();
        PCardLabelStart();
        PCardImage = [];
        PCardVideo = [];
    }

    public long PCardId { get; set; }

    internal LCardDraft? PCardDraft { get; set; }

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

    public string PTitle
    {
        get => _pTitle;
        set
        {
            if (string.Equals(_pTitle, value, StringComparison.Ordinal))
            {
                return;
            }

            _pTitle = value;
            PTitleUnreadable = false;
            PCardRaise(nameof(PTitle));
        }
    }

    public bool PTitleUnreadable
    {
        get => _pTitleUnreadable;
        private set
        {
            if (_pTitleUnreadable == value)
            {
                return;
            }

            _pTitleUnreadable = value;
            PCardRaise(nameof(PTitleUnreadable));
        }
    }

    public string PCardDefinition
    {
        get => _pCardDefinition;
        set
        {
            if (string.Equals(_pCardDefinition, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardDefinition = value;
            PCardDefinitionUnreadable = false;
            PCardRaise(nameof(PCardDefinition));
        }
    }

    public bool PCardDefinitionUnreadable
    {
        get => _pCardDefinitionUnreadable;
        private set
        {
            if (_pCardDefinitionUnreadable == value)
            {
                return;
            }

            _pCardDefinitionUnreadable = value;
            PCardRaise(nameof(PCardDefinitionUnreadable));
        }
    }

    public string PCardExpression
    {
        get => _pCardExpression;
        set
        {
            if (string.Equals(_pCardExpression, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardExpression = value;
            PCardExpressionUnreadable = false;
            PCardRaise(nameof(PCardExpression));
        }
    }

    public bool PCardExpressionUnreadable
    {
        get => _pCardExpressionUnreadable;
        private set
        {
            if (_pCardExpressionUnreadable == value)
            {
                return;
            }

            _pCardExpressionUnreadable = value;
            PCardRaise(nameof(PCardExpressionUnreadable));
        }
    }

    internal LStateValue PCardTitleRead()
    {
        return LStateValue.LStateValueResolve(_pTitle, _pTitleUnreadable);
    }

    internal LStateValue PCardDefinitionRead()
    {
        return LStateValue.LStateValueResolve(_pCardDefinition, _pCardDefinitionUnreadable);
    }

    internal LStateValue PCardExpressionRead()
    {
        return LStateValue.LStateValueResolve(_pCardExpression, _pCardExpressionUnreadable);
    }

    internal void PCardTitleShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pTitle = value.LStateValueShow();
        PCardRaise(nameof(PTitle));
        PTitleUnreadable = value.LStateValueState == LState.LStateUnknown;
    }

    internal void PCardDefinitionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pCardDefinition = value.LStateValueShow();
        PCardRaise(nameof(PCardDefinition));
        PCardDefinitionUnreadable = value.LStateValueState == LState.LStateUnknown;
    }

    internal void PCardExpressionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pCardExpression = value.LStateValueShow();
        PCardRaise(nameof(PCardExpression));
        PCardExpressionUnreadable = value.LStateValueState == LState.LStateUnknown;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
