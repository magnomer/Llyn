using System;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PCard : INotifyPropertyChanged
{
    private readonly string _pCardPrefix;
    private int _pCardOrder;
    private string _pTitle;
    private string _pCardDefinition;
    private string _pCardExpression;
    private string _pCardExample;
    private string _pCardSituation;
    private string _pCardTag;

    internal PCard(string prefix, int order)
    {
        _pCardPrefix = prefix;
        _pCardOrder = order;
        _pTitle = string.Empty;
        _pCardDefinition = string.Empty;
        _pCardExpression = string.Empty;
        _pCardExample = string.Empty;
        _pCardSituation = string.Empty;
        _pCardTag = string.Empty;
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
            PCardRaise(nameof(PTitle));
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
            PCardRaise(nameof(PCardDefinition));
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
            PCardRaise(nameof(PCardExpression));
        }
    }

    public string PCardExample
    {
        get => _pCardExample;
        set
        {
            if (string.Equals(_pCardExample, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardExample = value;
            PCardRaise(nameof(PCardExample));
        }
    }

    public string PCardSituation
    {
        get => _pCardSituation;
        set
        {
            if (string.Equals(_pCardSituation, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardSituation = value;
            PCardRaise(nameof(PCardSituation));
        }
    }

    public string PCardTag
    {
        get => _pCardTag;
        set
        {
            if (string.Equals(_pCardTag, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardTag = value;
            PCardRaise(nameof(PCardTag));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
