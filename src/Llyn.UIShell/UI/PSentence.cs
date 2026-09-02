using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PExample : INotifyPropertyChanged
{
    private string _pExampleId;
    private string _pExampleText;
    private bool _pExampleUnreadable;
    private string _pExampleReference;
    private bool _pExampleReferenceUnreadable;
    private string _pExampleReferenceName;
    private string _pExampleReferenceDraft;
    private string _pExampleOrderText;
    private bool _pExampleReferenceVisible;

    internal PExample(ObservableCollection<PReference> catalog)
        : this(catalog, LStateValue.LStateValueUnspecified, string.Empty, LStateValue.LStateValueUnspecified)
    {
    }

    internal PExample(
        ObservableCollection<PReference> catalog,
        LStateValue text,
        string id,
        LStateValue reference)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(reference);

        PExampleReferenceCatalog = catalog;
        _pExampleId = id;
        _pExampleText = text.LStateValueShow();
        _pExampleUnreadable = text.LStateValueState == LState.LStateUnknown;
        _pExampleReference = reference.LStateValueShow();
        _pExampleReferenceUnreadable = reference.LStateValueState == LState.LStateUnknown;
        _pExampleReferenceName = PExampleReferenceFind(_pExampleReference);
        _pExampleReferenceDraft = string.Empty;
        _pExampleOrderText = string.Empty;
    }

    public ObservableCollection<PReference> PExampleReferenceCatalog { get; }

    public string PExampleId
    {
        get => _pExampleId;
        private set
        {
            if (string.Equals(_pExampleId, value, StringComparison.Ordinal))
            {
                return;
            }

            _pExampleId = value;
            PExampleRaise(nameof(PExampleId));
        }
    }

    public string PExampleText
    {
        get => _pExampleText;
        set
        {
            if (string.Equals(_pExampleText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pExampleText = value;
            PExampleUnreadable = false;
            PExampleRaise(nameof(PExampleText));
        }
    }

    public bool PExampleUnreadable
    {
        get => _pExampleUnreadable;
        private set
        {
            if (_pExampleUnreadable == value)
            {
                return;
            }

            _pExampleUnreadable = value;
            PExampleRaise(nameof(PExampleUnreadable));
        }
    }

    public string PExampleReference
    {
        get => _pExampleReference;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pExampleReference, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pExampleReference = chosen;
            PExampleReferenceUnreadable = false;
            PExampleRaise(nameof(PExampleReference));

            PExampleReferenceName = PExampleReferenceFind(chosen);
            PExampleReferenceVisible = false;
        }
    }

    public bool PExampleReferenceUnreadable
    {
        get => _pExampleReferenceUnreadable;
        private set
        {
            if (_pExampleReferenceUnreadable == value)
            {
                return;
            }

            _pExampleReferenceUnreadable = value;
            PExampleRaise(nameof(PExampleReferenceUnreadable));
        }
    }

    public string PExampleReferenceName
    {
        get => _pExampleReferenceName;
        private set
        {
            if (string.Equals(_pExampleReferenceName, value, StringComparison.Ordinal))
            {
                return;
            }

            _pExampleReferenceName = value;
            PExampleRaise(nameof(PExampleReferenceName));
        }
    }

    public string PExampleReferenceDraft
    {
        get => _pExampleReferenceDraft;
        set
        {
            if (string.Equals(_pExampleReferenceDraft, value, StringComparison.Ordinal))
            {
                return;
            }

            _pExampleReferenceDraft = value;
            PExampleRaise(nameof(PExampleReferenceDraft));
        }
    }

    public bool PExampleReferenceVisible
    {
        get => _pExampleReferenceVisible;
        set
        {
            if (_pExampleReferenceVisible == value)
            {
                return;
            }

            _pExampleReferenceVisible = value;
            PExampleRaise(nameof(PExampleReferenceVisible));
        }
    }

    public string PExampleOrderText
    {
        get => _pExampleOrderText;
        set
        {
            if (string.Equals(_pExampleOrderText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pExampleOrderText = value;
            PExampleRaise(nameof(PExampleOrderText));
        }
    }

    internal LStateValue PExampleTextRead()
    {
        return _pExampleUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pExampleText);
    }

    internal LStateValue PExampleReferenceRead()
    {
        return _pExampleReferenceUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pExampleReference);
    }

    internal void PExampleIdentityApply()
    {
        if (_pExampleId.Length != 0 || PExampleTextRead().LStateValueEmpty)
        {
            return;
        }

        PExampleId = LEngine.LEngineIdentityCreate();
    }

    internal void PExampleClear()
    {
        PExampleText = string.Empty;
        PExampleUnreadable = false;
        PExampleReference = string.Empty;
        PExampleReferenceUnreadable = false;
        PExampleReferenceDraft = string.Empty;
        PExampleId = string.Empty;
    }

    internal void PExampleReferenceShow()
    {
        PExampleReferenceName = PExampleReferenceFind(_pExampleReference);
    }

    private string PExampleReferenceFind(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return string.Empty;
        }

        foreach (PReference row in PExampleReferenceCatalog)
        {
            if (string.Equals(row.PReferenceId, reference, StringComparison.Ordinal))
            {
                return row.PReferenceName;
            }
        }

        return string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PExampleRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
