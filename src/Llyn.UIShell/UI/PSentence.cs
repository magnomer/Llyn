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
    private string _pSentenceReference;
    private bool _pSentenceReferenceUnreadable;
    private string _pSentenceReferenceName;
    private string _pSentenceReferenceDraft;
    private string _pSentenceOrderText;
    private bool _pSentenceReferenceVisible;

    internal PSentence(ObservableCollection<PReference> catalog)
        : this(catalog, LStateValue.LStateValueUnspecified, string.Empty, LStateValue.LStateValueUnspecified)
    {
    }

    internal PSentence(
        ObservableCollection<PReference> catalog,
        LStateValue text,
        string id,
        LStateValue reference)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(reference);

        PSentenceReferenceCatalog = catalog;
        _pSentenceId = id;
        _pSentenceText = text.LStateValueShow();
        _pSentenceUnreadable = text.LStateValueState == LState.LStateUnknown;
        _pSentenceReference = reference.LStateValueShow();
        _pSentenceReferenceUnreadable = reference.LStateValueState == LState.LStateUnknown;
        _pSentenceReferenceName = PSentenceReferenceFind(_pSentenceReference);
        _pSentenceReferenceDraft = string.Empty;
        _pSentenceOrderText = string.Empty;
    }

    public ObservableCollection<PReference> PSentenceReferenceCatalog { get; }

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

    public string PSentenceReference
    {
        get => _pSentenceReference;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pSentenceReference, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceReference = chosen;
            PSentenceReferenceUnreadable = false;
            PSentenceRaise(nameof(PSentenceReference));

            PSentenceReferenceName = PSentenceReferenceFind(chosen);
            PSentenceReferenceVisible = false;
        }
    }

    public bool PSentenceReferenceUnreadable
    {
        get => _pSentenceReferenceUnreadable;
        private set
        {
            if (_pSentenceReferenceUnreadable == value)
            {
                return;
            }

            _pSentenceReferenceUnreadable = value;
            PSentenceRaise(nameof(PSentenceReferenceUnreadable));
        }
    }

    public string PSentenceReferenceName
    {
        get => _pSentenceReferenceName;
        private set
        {
            if (string.Equals(_pSentenceReferenceName, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceReferenceName = value;
            PSentenceRaise(nameof(PSentenceReferenceName));
        }
    }

    public string PSentenceReferenceDraft
    {
        get => _pSentenceReferenceDraft;
        set
        {
            if (string.Equals(_pSentenceReferenceDraft, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceReferenceDraft = value;
            PSentenceRaise(nameof(PSentenceReferenceDraft));
        }
    }

    public bool PSentenceReferenceVisible
    {
        get => _pSentenceReferenceVisible;
        set
        {
            if (_pSentenceReferenceVisible == value)
            {
                return;
            }

            _pSentenceReferenceVisible = value;
            PSentenceRaise(nameof(PSentenceReferenceVisible));
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

    internal LStateValue PSentenceReferenceRead()
    {
        return _pSentenceReferenceUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pSentenceReference);
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
        PSentenceReference = string.Empty;
        PSentenceReferenceUnreadable = false;
        PSentenceReferenceDraft = string.Empty;
        PSentenceId = string.Empty;
    }

    internal void PSentenceReferenceShow()
    {
        PSentenceReferenceName = PSentenceReferenceFind(_pSentenceReference);
    }

    private string PSentenceReferenceFind(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return string.Empty;
        }

        foreach (PReference row in PSentenceReferenceCatalog)
        {
            if (string.Equals(row.PReferenceId, reference, StringComparison.Ordinal))
            {
                return row.PReferenceName;
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
