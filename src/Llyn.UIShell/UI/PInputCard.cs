using System;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PInputCard : INotifyPropertyChanged
{
    private readonly string _pInputCardPrefix;
    private int _pInputCardOrder;
    private string _pTitle;
    private string _pInputCardDefinition;
    private string _pInputCardExpression;
    private string _pInputCardExample;
    private string _pInputCardSituation;
    private string _pInputCardSynonym;
    private string _pInputCardTag;

    internal PInputCard(string prefix, int order)
    {
        _pInputCardPrefix = prefix;
        _pInputCardOrder = order;
        _pTitle = string.Empty;
        _pInputCardDefinition = string.Empty;
        _pInputCardExpression = string.Empty;
        _pInputCardExample = string.Empty;
        _pInputCardSituation = string.Empty;
        _pInputCardSynonym = string.Empty;
        _pInputCardTag = string.Empty;
    }

    public int PInputCardOrder
    {
        get => _pInputCardOrder;
        set
        {
            if (_pInputCardOrder == value)
            {
                return;
            }

            _pInputCardOrder = value;
            PInputCardRaise(nameof(PInputCardOrder));
            PInputCardRaise(nameof(PInputCardTitle));
        }
    }

    public string PInputCardTitle => $"{_pInputCardPrefix} {_pInputCardOrder}";

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
            PInputCardRaise(nameof(PTitle));
        }
    }

    public string PInputCardDefinition
    {
        get => _pInputCardDefinition;
        set
        {
            if (string.Equals(_pInputCardDefinition, value, StringComparison.Ordinal))
            {
                return;
            }

            _pInputCardDefinition = value;
            PInputCardRaise(nameof(PInputCardDefinition));
        }
    }

    public string PInputCardExpression
    {
        get => _pInputCardExpression;
        set
        {
            if (string.Equals(_pInputCardExpression, value, StringComparison.Ordinal))
            {
                return;
            }

            _pInputCardExpression = value;
            PInputCardRaise(nameof(PInputCardExpression));
        }
    }

    public string PInputCardExample
    {
        get => _pInputCardExample;
        set
        {
            if (string.Equals(_pInputCardExample, value, StringComparison.Ordinal))
            {
                return;
            }

            _pInputCardExample = value;
            PInputCardRaise(nameof(PInputCardExample));
        }
    }

    public string PInputCardSituation
    {
        get => _pInputCardSituation;
        set
        {
            if (string.Equals(_pInputCardSituation, value, StringComparison.Ordinal))
            {
                return;
            }

            _pInputCardSituation = value;
            PInputCardRaise(nameof(PInputCardSituation));
        }
    }

    public string PInputCardSynonym
    {
        get => _pInputCardSynonym;
        set
        {
            if (string.Equals(_pInputCardSynonym, value, StringComparison.Ordinal))
            {
                return;
            }

            _pInputCardSynonym = value;
            PInputCardRaise(nameof(PInputCardSynonym));
        }
    }

    public string PInputCardTag
    {
        get => _pInputCardTag;
        set
        {
            if (string.Equals(_pInputCardTag, value, StringComparison.Ordinal))
            {
                return;
            }

            _pInputCardTag = value;
            PInputCardRaise(nameof(PInputCardTag));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PInputCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
