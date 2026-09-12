using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PSentence : INotifyPropertyChanged
{
    private long _pSentenceId;
    private string _pSentenceText;
    private bool _pSentenceUnknown;
    private long _pSentenceCitation;
    private bool _pSentenceCitationUnknown;
    private string _pSentenceCitationName;
    private bool _pSentenceCitationVisible;
    private string _pSentenceParticle;
    private bool _pSentenceParticleUnknown;
    private string _pSentenceDependence;
    private bool _pSentenceDependenceUnknown;
    private int _pSentenceParticleColumn;
    private int _pSentenceDependenceColumn = 2;
    private bool _pSentenceFrameVisible;
    private long _pSentenceRow;

    internal PSentence(
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        LSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LExampleDraft example = draft.LSentenceDraftExample
            ?? LExampleDraft.LExampleDraftCreate(string.Empty);

        PSentenceCitationCatalog = catalog;
        PSentenceParticleCatalog = particles;
        PSentenceDependenceCatalog = dependences;
        _pSentenceRow = draft.LSentenceDraftId;
        _pSentenceId = example.LExampleDraftId;
        _pSentenceText = example.LExampleDraftText.LStateValueShow();
        _pSentenceUnknown = example.LExampleDraftText.LStateValueState == LState.LStateUnknown;
        _pSentenceCitation = example.LExampleDraftReference.LStateAnchorShow();
        _pSentenceCitationUnknown = example.LExampleDraftReference.LStateAnchorState == LState.LStateUnknown;
        _pSentenceCitationName = PSentenceCitationFind(_pSentenceCitation);
        _pSentenceParticle = draft.LSentenceDraftParticle.LStateValueShow();
        _pSentenceParticleUnknown = draft.LSentenceDraftParticle.LStateValueState == LState.LStateUnknown;
        _pSentenceDependence = draft.LSentenceDraftDependence.LStateValueShow();
        _pSentenceDependenceUnknown = draft.LSentenceDraftDependence.LStateValueState == LState.LStateUnknown;
    }

    public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }

    public ObservableCollection<string> PSentenceParticleCatalog { get; }

    public ObservableCollection<string> PSentenceDependenceCatalog { get; }

    public long PSentenceId
    {
        get => _pSentenceId;
        private set
        {
            if (_pSentenceId == value)
            {
                return;
            }

            _pSentenceId = value;
            PSentenceRaise(nameof(PSentenceId));
        }
    }

    internal long PSentenceRow => _pSentenceRow;

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
            PSentenceUnknown = false;
            PSentenceRaise(nameof(PSentenceText));
        }
    }

    public bool PSentenceUnknown
    {
        get => _pSentenceUnknown;
        private set
        {
            if (_pSentenceUnknown == value)
            {
                return;
            }

            _pSentenceUnknown = value;
            PSentenceRaise(nameof(PSentenceUnknown));
        }
    }

    public long PSentenceCitationId
    {
        get => _pSentenceCitation;
        set
        {
            long chosen = value;
            if (_pSentenceCitation == chosen)
            {
                return;
            }

            _pSentenceCitation = chosen;
            PSentenceCitationUnknown = false;
            PSentenceRaise(nameof(PSentenceCitationId));

            PSentenceCitationName = PSentenceCitationFind(chosen);
            PSentenceCitationVisible = false;
        }
    }

    public bool PSentenceCitationUnknown
    {
        get => _pSentenceCitationUnknown;
        private set
        {
            if (_pSentenceCitationUnknown == value)
            {
                return;
            }

            _pSentenceCitationUnknown = value;
            PSentenceRaise(nameof(PSentenceCitationUnknown));
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

    public string PSentenceParticle
    {
        get => _pSentenceParticle;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pSentenceParticle, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceParticle = chosen;
            PSentenceParticleUnknown = false;
            PSentenceRaise(nameof(PSentenceParticle));
            PSentenceRaise(nameof(PSentenceFrameVisible));
            PSentenceRaise(nameof(PSentenceFrameWritten));
            PSentenceRaise(nameof(PSentenceFrameGap));
        }
    }

    public bool PSentenceParticleUnknown
    {
        get => _pSentenceParticleUnknown;
        private set
        {
            if (_pSentenceParticleUnknown == value)
            {
                return;
            }

            _pSentenceParticleUnknown = value;
            PSentenceRaise(nameof(PSentenceParticleUnknown));
            PSentenceRaise(nameof(PSentenceFrameVisible));
            PSentenceRaise(nameof(PSentenceFrameWritten));
            PSentenceRaise(nameof(PSentenceFrameGap));
        }
    }

    public string PSentenceDependence
    {
        get => _pSentenceDependence;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pSentenceDependence, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pSentenceDependence = chosen;
            PSentenceDependenceUnknown = false;
            PSentenceRaise(nameof(PSentenceDependence));
            PSentenceRaise(nameof(PSentenceFrameVisible));
            PSentenceRaise(nameof(PSentenceFrameWritten));
            PSentenceRaise(nameof(PSentenceFrameGap));
        }
    }

    public bool PSentenceDependenceUnknown
    {
        get => _pSentenceDependenceUnknown;
        private set
        {
            if (_pSentenceDependenceUnknown == value)
            {
                return;
            }

            _pSentenceDependenceUnknown = value;
            PSentenceRaise(nameof(PSentenceDependenceUnknown));
            PSentenceRaise(nameof(PSentenceFrameVisible));
            PSentenceRaise(nameof(PSentenceFrameWritten));
            PSentenceRaise(nameof(PSentenceFrameGap));
        }
    }

    public bool PSentenceFrameVisible
    {
        get => _pSentenceFrameVisible || PSentenceFrameWritten;
        set
        {
            _pSentenceFrameVisible = value;
            PSentenceRaise(nameof(PSentenceFrameVisible));
        }
    }

    public bool PSentenceFrameWritten =>
        _pSentenceParticle.Length > 0
        || _pSentenceDependence.Length > 0
        || _pSentenceParticleUnknown
        || _pSentenceDependenceUnknown;

    public string PSentenceFrameGap
    {
        get
        {
            bool particle = _pSentenceParticle.Length > 0 || _pSentenceParticleUnknown;
            bool dependence = _pSentenceDependence.Length > 0 || _pSentenceDependenceUnknown;
            return particle && dependence ? " " : string.Empty;
        }
    }

    public int PSentenceParticleColumn
    {
        get => _pSentenceParticleColumn;
        private set
        {
            if (_pSentenceParticleColumn == value)
            {
                return;
            }

            _pSentenceParticleColumn = value;
            PSentenceRaise(nameof(PSentenceParticleColumn));
        }
    }

    public int PSentenceDependenceColumn
    {
        get => _pSentenceDependenceColumn;
        private set
        {
            if (_pSentenceDependenceColumn == value)
            {
                return;
            }

            _pSentenceDependenceColumn = value;
            PSentenceRaise(nameof(PSentenceDependenceColumn));
        }
    }

    internal void PSentenceOrderApply(LSentenceOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        PSentenceParticleColumn = order.LSentenceOrderParticle * 2;
        PSentenceDependenceColumn = order.LSentenceOrderDependence * 2;
    }

    internal LStateValue PSentenceParticleRead()
    {
        return LStateValue.LStateValueResolve(_pSentenceParticle, _pSentenceParticleUnknown);
    }

    internal LStateValue PSentenceDependenceRead()
    {
        return LStateValue.LStateValueResolve(_pSentenceDependence, _pSentenceDependenceUnknown);
    }

    internal LStateValue PSentenceTextRead()
    {
        return LStateValue.LStateValueResolve(_pSentenceText, _pSentenceUnknown);
    }

    internal LStateAnchor PSentenceCitationRead()
    {
        return _pSentenceCitationUnknown
            ? LStateAnchor.LStateAnchorUnknown
            : LStateAnchor.LStateAnchorRead(_pSentenceCitation);
    }

    internal void PSentenceShow(LSentenceDraft draft, Func<string, bool> pending)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(pending);

        LExampleDraft example = draft.LSentenceDraftExample
            ?? LExampleDraft.LExampleDraftCreate(string.Empty);

        _pSentenceRow = draft.LSentenceDraftId;
        PSentenceId = example.LExampleDraftId;

        if (!pending(nameof(PSentenceText)) && PSentenceTextRead() != example.LExampleDraftText)
        {
            _pSentenceText = example.LExampleDraftText.LStateValueShow();
            PSentenceRaise(nameof(PSentenceText));
            PSentenceUnknown = example.LExampleDraftText.LStateValueState == LState.LStateUnknown;
        }

        if (PSentenceCitationRead() != example.LExampleDraftReference)
        {
            _pSentenceCitation = example.LExampleDraftReference.LStateAnchorShow();
            PSentenceRaise(nameof(PSentenceCitationId));
            PSentenceCitationUnknown = example.LExampleDraftReference.LStateAnchorState == LState.LStateUnknown;
            PSentenceCitationName = PSentenceCitationFind(_pSentenceCitation);
        }

        if (!pending(nameof(PSentenceParticle)) && PSentenceParticleRead() != draft.LSentenceDraftParticle)
        {
            _pSentenceParticle = draft.LSentenceDraftParticle.LStateValueShow();
            PSentenceRaise(nameof(PSentenceParticle));
            PSentenceParticleUnknown = draft.LSentenceDraftParticle.LStateValueState == LState.LStateUnknown;
        }

        if (!pending(nameof(PSentenceDependence)) && PSentenceDependenceRead() != draft.LSentenceDraftDependence)
        {
            _pSentenceDependence = draft.LSentenceDraftDependence.LStateValueShow();
            PSentenceRaise(nameof(PSentenceDependence));
            PSentenceDependenceUnknown = draft.LSentenceDraftDependence.LStateValueState == LState.LStateUnknown;
        }
    }

    internal void PSentenceCitationShow()
    {
        PSentenceCitationName = PSentenceCitationFind(_pSentenceCitation);
    }

    private string PSentenceCitationFind(long reference)
    {
        if (reference == 0)
        {
            return string.Empty;
        }

        foreach (PCitationItem row in PSentenceCitationCatalog)
        {
            if (row.PCitationItemId == reference)
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
