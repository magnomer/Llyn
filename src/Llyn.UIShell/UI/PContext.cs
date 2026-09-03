using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PContext : INotifyPropertyChanged
{
    private string _pContextId;
    private string _pContextText;
    private bool _pContextUnreadable;
    private string _pContextReference;
    private bool _pContextReferenceUnreadable;
    private string _pContextReferenceName;
    private string _pContextReferenceDraft;
    private string _pContextOrderText;
    private bool _pContextReferenceVisible;

    internal PContext(ObservableCollection<PCitationItem> catalog)
        : this(catalog, LStateValue.LStateValueUnspecified, string.Empty, LStateValue.LStateValueUnspecified)
    {
    }

    internal PContext(
        ObservableCollection<PCitationItem> catalog,
        LStateValue text,
        string id,
        LStateValue reference)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(reference);

        PContextReferenceCatalog = catalog;
        _pContextId = id;
        _pContextText = text.LStateValueShow();
        _pContextUnreadable = text.LStateValueState == LState.LStateUnknown;
        _pContextReference = reference.LStateValueShow();
        _pContextReferenceUnreadable = reference.LStateValueState == LState.LStateUnknown;
        _pContextReferenceName = PContextReferenceFind(_pContextReference);
        _pContextReferenceDraft = string.Empty;
        _pContextOrderText = string.Empty;
    }

    public ObservableCollection<PCitationItem> PContextReferenceCatalog { get; }

    public string PContextId
    {
        get => _pContextId;
        private set
        {
            if (string.Equals(_pContextId, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextId = value;
            PContextRaise(nameof(PContextId));
        }
    }

    public string PContextText
    {
        get => _pContextText;
        set
        {
            if (string.Equals(_pContextText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextText = value;
            PContextUnreadable = false;
            PContextRaise(nameof(PContextText));
        }
    }

    public bool PContextUnreadable
    {
        get => _pContextUnreadable;
        private set
        {
            if (_pContextUnreadable == value)
            {
                return;
            }

            _pContextUnreadable = value;
            PContextRaise(nameof(PContextUnreadable));
        }
    }

    public string PContextReference
    {
        get => _pContextReference;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pContextReference, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pContextReference = chosen;
            PContextReferenceUnreadable = false;
            PContextRaise(nameof(PContextReference));

            PContextReferenceName = PContextReferenceFind(chosen);
            PContextReferenceVisible = false;
        }
    }

    public bool PContextReferenceUnreadable
    {
        get => _pContextReferenceUnreadable;
        private set
        {
            if (_pContextReferenceUnreadable == value)
            {
                return;
            }

            _pContextReferenceUnreadable = value;
            PContextRaise(nameof(PContextReferenceUnreadable));
        }
    }

    public string PContextReferenceName
    {
        get => _pContextReferenceName;
        private set
        {
            if (string.Equals(_pContextReferenceName, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextReferenceName = value;
            PContextRaise(nameof(PContextReferenceName));
        }
    }

    public string PContextReferenceDraft
    {
        get => _pContextReferenceDraft;
        set
        {
            if (string.Equals(_pContextReferenceDraft, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextReferenceDraft = value;
            PContextRaise(nameof(PContextReferenceDraft));
        }
    }

    public bool PContextReferenceVisible
    {
        get => _pContextReferenceVisible;
        set
        {
            if (_pContextReferenceVisible == value)
            {
                return;
            }

            _pContextReferenceVisible = value;
            PContextRaise(nameof(PContextReferenceVisible));
        }
    }

    public string PContextOrderText
    {
        get => _pContextOrderText;
        set
        {
            if (string.Equals(_pContextOrderText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextOrderText = value;
            PContextRaise(nameof(PContextOrderText));
        }
    }

    internal LStateValue PContextTextRead()
    {
        return _pContextUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pContextText);
    }

    internal LStateValue PContextReferenceRead()
    {
        return _pContextReferenceUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pContextReference);
    }

    internal void PContextIdentityApply()
    {
        if (_pContextId.Length != 0 || PContextTextRead().LStateValueEmpty)
        {
            return;
        }

        PContextId = LEngine.LEngineIdentityCreate();
    }

    internal void PContextClear()
    {
        PContextText = string.Empty;
        PContextUnreadable = false;
        PContextReference = string.Empty;
        PContextReferenceUnreadable = false;
        PContextReferenceDraft = string.Empty;
        PContextId = string.Empty;
    }

    internal void PContextReferenceShow()
    {
        PContextReferenceName = PContextReferenceFind(_pContextReference);
    }

    private string PContextReferenceFind(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return string.Empty;
        }

        foreach (PCitationItem row in PContextReferenceCatalog)
        {
            if (string.Equals(row.PCitationItemId, reference, StringComparison.Ordinal))
            {
                return row.PCitationItemName;
            }
        }

        return string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PContextRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
