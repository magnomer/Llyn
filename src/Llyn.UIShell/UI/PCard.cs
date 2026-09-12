using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed partial class PCard : INotifyPropertyChanged
{
    internal const string PCardUnknownMark = "(?)";

    private readonly LEngine _pCardEngine;
    private readonly string _pCardPrefix;
    private readonly ObservableCollection<PCitationItem> _pCardCitation;
    private readonly ObservableCollection<string> _pCardParticle;
    private readonly ObservableCollection<string> _pCardDependence;
    private int _pCardPosition;
    private string _pCardPositionText;
    private bool _pCardPositionActive;
    private string _pTitle;
    private bool _pTitleUnknown;
    private string _pCardDefinition;
    private bool _pCardDefinitionUnknown;
    private string _pCardExpression;
    private bool _pCardExpressionUnknown;

    internal PCard(
        LEngine engine,
        string prefix,
        int position,
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _pCardEngine = engine;
        _pCardPrefix = prefix;
        _pCardPosition = position;
        _pCardPositionText = position.ToString(CultureInfo.InvariantCulture);
        _pCardCitation = catalog;
        _pCardParticle = particles;
        _pCardDependence = dependences;
        _pTitle = string.Empty;
        _pCardDefinition = string.Empty;
        _pCardExpression = string.Empty;
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
            PTitleUnknown = false;
            PCardRaise(nameof(PTitle));
        }
    }

    public bool PTitleUnknown
    {
        get => _pTitleUnknown;
        private set
        {
            if (_pTitleUnknown == value)
            {
                return;
            }

            _pTitleUnknown = value;
            PCardRaise(nameof(PTitleUnknown));
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
            PCardDefinitionUnknown = false;
            PCardRaise(nameof(PCardDefinition));
        }
    }

    public bool PCardDefinitionUnknown
    {
        get => _pCardDefinitionUnknown;
        private set
        {
            if (_pCardDefinitionUnknown == value)
            {
                return;
            }

            _pCardDefinitionUnknown = value;
            PCardRaise(nameof(PCardDefinitionUnknown));
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
            PCardExpressionUnknown = false;
            PCardRaise(nameof(PCardExpression));
        }
    }

    public bool PCardExpressionUnknown
    {
        get => _pCardExpressionUnknown;
        private set
        {
            if (_pCardExpressionUnknown == value)
            {
                return;
            }

            _pCardExpressionUnknown = value;
            PCardRaise(nameof(PCardExpressionUnknown));
        }
    }

    internal LStateValue PCardTitleRead()
    {
        return LStateValue.LStateValueResolve(_pTitle, _pTitleUnknown);
    }

    internal LStateValue PCardDefinitionRead()
    {
        return LStateValue.LStateValueResolve(_pCardDefinition, _pCardDefinitionUnknown);
    }

    internal LStateValue PCardExpressionRead()
    {
        return LStateValue.LStateValueResolve(_pCardExpression, _pCardExpressionUnknown);
    }

    internal void PCardTitleShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pTitle = value.LStateValueShow();
        PCardRaise(nameof(PTitle));
        PTitleUnknown = value.LStateValueState == LState.LStateUnknown;
    }

    internal void PCardDefinitionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pCardDefinition = value.LStateValueShow();
        PCardRaise(nameof(PCardDefinition));
        PCardDefinitionUnknown = value.LStateValueState == LState.LStateUnknown;
    }

    internal void PCardExpressionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pCardExpression = value.LStateValueShow();
        PCardRaise(nameof(PCardExpression));
        PCardExpressionUnknown = value.LStateValueState == LState.LStateUnknown;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
