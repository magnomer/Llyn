using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PSentence : INotifyPropertyChanged
{
    private string _pSentenceId;
    private string _pSentenceText;
    private bool _pSentenceUnreadable;
    private string _pSentenceCitation;
    private bool _pSentenceCitationUnreadable;
    private string _pSentenceCitationName;
    private string _pSentenceOrderText;
    private bool _pSentenceCitationVisible;

    internal PSentence(ObservableCollection<PCitationItem> catalog)
        : this(catalog, LStateValue.LStateValueUnspecified, string.Empty, LStateValue.LStateValueUnspecified)
    {
    }

    internal PSentence(
        ObservableCollection<PCitationItem> catalog,
        LStateValue text,
        string id,
        LStateValue reference)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(reference);

        PSentenceCitationCatalog = catalog;
        _pSentenceId = id;
        _pSentenceText = text.LStateValueShow();
        _pSentenceUnreadable = text.LStateValueState == LState.LStateUnknown;
        _pSentenceCitation = reference.LStateValueShow();
        _pSentenceCitationUnreadable = reference.LStateValueState == LState.LStateUnknown;
        _pSentenceCitationName = PSentenceCitationFind(_pSentenceCitation);
        _pSentenceOrderText = string.Empty;
    }

    public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }

    public string PSentenceId
    {
        get => _pSentenceId;
        private set
        {
            if (string.Equals(_pSentenceId, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceId = value;
            PSentenceRaise(nameof(PSentenceId));
        }
    }

    public string PSentenceText
    {
        get => _pSentenceText;
        set
        {
            if (string.Equals(_pSentenceText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceText = value;
            PSentenceUnreadable = false;
            PSentenceRaise(nameof(PSentenceText));
        }
    }

    public bool PSentenceUnreadable
    {
        get => _pSentenceUnreadable;
        private set
        {
            if (_pSentenceUnreadable == value)
            {
                return;
            }

            _pSentenceUnreadable = value;
            PSentenceRaise(nameof(PSentenceUnreadable));
        }
    }

    public string PSentenceCitation
    {
        get => _pSentenceCitation;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pSentenceCitation, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceCitation = chosen;
            PSentenceCitationUnreadable = false;
            PSentenceRaise(nameof(PSentenceCitation));

            PSentenceCitationName = PSentenceCitationFind(chosen);
            PSentenceCitationVisible = false;
        }
    }

    public bool PSentenceCitationUnreadable
    {
        get => _pSentenceCitationUnreadable;
        private set
        {
            if (_pSentenceCitationUnreadable == value)
            {
                return;
            }

            _pSentenceCitationUnreadable = value;
            PSentenceRaise(nameof(PSentenceCitationUnreadable));
        }
    }

    public string PSentenceCitationName
    {
        get => _pSentenceCitationName;
        private set
        {
            if (string.Equals(_pSentenceCitationName, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceCitationName = value;
            PSentenceRaise(nameof(PSentenceCitationName));
        }
    }

    public bool PSentenceCitationVisible
    {
        get => _pSentenceCitationVisible;
        set
        {
            if (_pSentenceCitationVisible == value)
            {
                return;
            }

            _pSentenceCitationVisible = value;
            PSentenceRaise(nameof(PSentenceCitationVisible));
        }
    }

    public string PSentenceOrderText
    {
        get => _pSentenceOrderText;
        set
        {
            if (string.Equals(_pSentenceOrderText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceOrderText = value;
            PSentenceRaise(nameof(PSentenceOrderText));
        }
    }

    internal LStateValue PSentenceTextRead()
    {
        return _pSentenceUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pSentenceText);
    }

    internal LStateValue PSentenceCitationRead()
    {
        return _pSentenceCitationUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pSentenceCitation);
    }

    internal void PSentenceIdentityApply()
    {
        if (_pSentenceId.Length != 0 || PSentenceTextRead().LStateValueEmpty)
        {
            return;
        }

        PSentenceId = LEngine.LEngineIdentityCreate();
    }

    internal void PSentenceClear()
    {
        PSentenceText = string.Empty;
        PSentenceUnreadable = false;
        PSentenceCitation = string.Empty;
        PSentenceCitationUnreadable = false;
        PSentenceId = string.Empty;
    }

    internal void PSentenceCitationShow()
    {
        PSentenceCitationName = PSentenceCitationFind(_pSentenceCitation);
    }

    private string PSentenceCitationFind(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return string.Empty;
        }

        foreach (PCitationItem row in PSentenceCitationCatalog)
        {
            if (string.Equals(row.PCitationItemId, reference, StringComparison.Ordinal))
            {
                return row.PCitationItemName;
            }
        }

        return string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PSentenceRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
