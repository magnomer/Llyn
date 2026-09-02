using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PSituation : INotifyPropertyChanged
{
    private string _pSituationId;
    private string _pSituationText;
    private bool _pSituationUnreadable;
    private string _pSituationReference;
    private bool _pSituationReferenceUnreadable;
    private string _pSituationReferenceName;
    private string _pSituationReferenceDraft;
    private string _pSituationOrderText;
    private bool _pSituationReferenceVisible;

    internal PSituation(ObservableCollection<PReference> catalog)
        : this(catalog, LStateValue.LStateValueUnspecified, string.Empty, LStateValue.LStateValueUnspecified)
    {
    }

    internal PSituation(
        ObservableCollection<PReference> catalog,
        LStateValue text,
        string id,
        LStateValue reference)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(reference);

        PSituationReferenceCatalog = catalog;
        _pSituationId = id;
        _pSituationText = text.LStateValueShow();
        _pSituationUnreadable = text.LStateValueState == LState.LStateUnknown;
        _pSituationReference = reference.LStateValueShow();
        _pSituationReferenceUnreadable = reference.LStateValueState == LState.LStateUnknown;
        _pSituationReferenceName = PSituationReferenceFind(_pSituationReference);
        _pSituationReferenceDraft = string.Empty;
        _pSituationOrderText = string.Empty;
    }

    public ObservableCollection<PReference> PSituationReferenceCatalog { get; }

    public string PSituationId
    {
        get => _pSituationId;
        private set
        {
            if (string.Equals(_pSituationId, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSituationId = value;
            PSituationRaise(nameof(PSituationId));
        }
    }

    public string PSituationText
    {
        get => _pSituationText;
        set
        {
            if (string.Equals(_pSituationText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSituationText = value;
            PSituationUnreadable = false;
            PSituationRaise(nameof(PSituationText));
        }
    }

    public bool PSituationUnreadable
    {
        get => _pSituationUnreadable;
        private set
        {
            if (_pSituationUnreadable == value)
            {
                return;
            }

            _pSituationUnreadable = value;
            PSituationRaise(nameof(PSituationUnreadable));
        }
    }

    public string PSituationReference
    {
        get => _pSituationReference;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pSituationReference, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pSituationReference = chosen;
            PSituationReferenceUnreadable = false;
            PSituationRaise(nameof(PSituationReference));

            PSituationReferenceName = PSituationReferenceFind(chosen);
            PSituationReferenceVisible = false;
        }
    }

    public bool PSituationReferenceUnreadable
    {
        get => _pSituationReferenceUnreadable;
        private set
        {
            if (_pSituationReferenceUnreadable == value)
            {
                return;
            }

            _pSituationReferenceUnreadable = value;
            PSituationRaise(nameof(PSituationReferenceUnreadable));
        }
    }

    public string PSituationReferenceName
    {
        get => _pSituationReferenceName;
        private set
        {
            if (string.Equals(_pSituationReferenceName, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSituationReferenceName = value;
            PSituationRaise(nameof(PSituationReferenceName));
        }
    }

    public string PSituationReferenceDraft
    {
        get => _pSituationReferenceDraft;
        set
        {
            if (string.Equals(_pSituationReferenceDraft, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSituationReferenceDraft = value;
            PSituationRaise(nameof(PSituationReferenceDraft));
        }
    }

    public bool PSituationReferenceVisible
    {
        get => _pSituationReferenceVisible;
        set
        {
            if (_pSituationReferenceVisible == value)
            {
                return;
            }

            _pSituationReferenceVisible = value;
            PSituationRaise(nameof(PSituationReferenceVisible));
        }
    }

    public string PSituationOrderText
    {
        get => _pSituationOrderText;
        set
        {
            if (string.Equals(_pSituationOrderText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pSituationOrderText = value;
            PSituationRaise(nameof(PSituationOrderText));
        }
    }

    internal LStateValue PSituationTextRead()
    {
        return _pSituationUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pSituationText);
    }

    internal LStateValue PSituationReferenceRead()
    {
        return _pSituationReferenceUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pSituationReference);
    }

    internal void PSituationIdentityApply()
    {
        if (_pSituationId.Length != 0 || PSituationTextRead().LStateValueEmpty)
        {
            return;
        }

        PSituationId = LEngine.LEngineIdentityCreate();
    }

    internal void PSituationClear()
    {
        PSituationText = string.Empty;
        PSituationUnreadable = false;
        PSituationReference = string.Empty;
        PSituationReferenceUnreadable = false;
        PSituationReferenceDraft = string.Empty;
        PSituationId = string.Empty;
    }

    internal void PSituationReferenceShow()
    {
        PSituationReferenceName = PSituationReferenceFind(_pSituationReference);
    }

    private string PSituationReferenceFind(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return string.Empty;
        }

        foreach (PReference row in PSituationReferenceCatalog)
        {
            if (string.Equals(row.PReferenceId, reference, StringComparison.Ordinal))
            {
                return row.PReferenceName;
            }
        }

        return string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PSituationRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
