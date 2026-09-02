using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard : INotifyPropertyChanged
{
    internal const string PCardUnreadableMark = "(?)";

    private readonly string _pCardPrefix;
    private readonly ObservableCollection<PReference> _pCardReference;
    private int _pCardOrder;
    private string _pTitle;
    private bool _pTitleUnreadable;
    private string _pCardDefinition;
    private bool _pCardDefinitionUnreadable;
    private string _pCardExpression;
    private bool _pCardExpressionUnreadable;

    internal PCard(string prefix, int order, ObservableCollection<PReference> catalog)
    {
        _pCardPrefix = prefix;
        _pCardOrder = order;
        _pCardReference = catalog;
        _pTitle = string.Empty;
        _pCardDefinition = string.Empty;
        _pCardExpression = string.Empty;
        PCardExample = [];
        PCardExampleAdd(new PExample(catalog));
        PCardExampleUpdate();
        PCardSituation = [];
        PCardSituationAdd(new PContext(catalog));
        PCardSituationUpdate();
        PCardTagStart();
        PCardImage = [];
        PCardVideo = [];
    }

    public string PCardId { get; set; } = string.Empty;

    public int PCardOrder
    {
        get => _pCardOrder;
        set
        {
            if (_pCardOrder == value)
            {
                return;
            }

            _pCardOrder = value;
            PCardRaise(nameof(PCardOrder));
            PCardRaise(nameof(PCardTitle));
        }
    }

    public string PCardTitle => $"{_pCardPrefix} {_pCardOrder}";

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
        return PCardValueRead(_pTitleUnreadable, _pTitle);
    }

    internal LStateValue PCardDefinitionRead()
    {
        return PCardValueRead(_pCardDefinitionUnreadable, _pCardDefinition);
    }

    internal LStateValue PCardExpressionRead()
    {
        return PCardValueRead(_pCardExpressionUnreadable, _pCardExpression);
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

    private static LStateValue PCardValueRead(bool unreadable, string text)
    {
        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueRead(text);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
