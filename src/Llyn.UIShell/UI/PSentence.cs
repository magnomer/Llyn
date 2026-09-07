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
    private bool _pSentenceCitationVisible;
    private string _pSentenceParticle;
    private bool _pSentenceParticleUnreadable;
    private string _pSentenceDependence;
    private bool _pSentenceDependenceUnreadable;
    private int _pSentenceParticleColumn;
    private int _pSentenceDependenceColumn = 2;
    private bool _pSentenceFrameVisible;

    internal PSentence(
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences)
        : this(catalog, particles, dependences, LExampleDraft.LExampleDraftCreate(string.Empty))
    {
    }

    internal PSentence(
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        LExampleDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        PSentenceCitationCatalog = catalog;
        PSentenceParticleCatalog = particles;
        PSentenceDependenceCatalog = dependences;
        _pSentenceId = draft.LExampleDraftId;
        _pSentenceText = draft.LExampleDraftText.LStateValueShow();
        _pSentenceUnreadable = draft.LExampleDraftText.LStateValueState == LState.LStateUnknown;
        _pSentenceCitation = draft.LExampleDraftReference.LStateValueShow();
        _pSentenceCitationUnreadable = draft.LExampleDraftReference.LStateValueState == LState.LStateUnknown;
        _pSentenceCitationName = PSentenceCitationFind(_pSentenceCitation);
        _pSentenceParticle = draft.LExampleDraftParticle.LStateValueShow();
        _pSentenceParticleUnreadable = draft.LExampleDraftParticle.LStateValueState == LState.LStateUnknown;
        _pSentenceDependence = draft.LExampleDraftDependence.LStateValueShow();
        _pSentenceDependenceUnreadable = draft.LExampleDraftDependence.LStateValueState == LState.LStateUnknown;
    }

    public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }

    public ObservableCollection<string> PSentenceParticleCatalog { get; }

    public ObservableCollection<string> PSentenceDependenceCatalog { get; }

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
            PSentenceParticleUnreadable = false;
            PSentenceRaise(nameof(PSentenceParticle));
            PSentenceRaise(nameof(PSentenceFrameVisible));
            PSentenceRaise(nameof(PSentenceFrameGap));
        }
    }

    public bool PSentenceParticleUnreadable
    {
        get => _pSentenceParticleUnreadable;
        private set
        {
            if (_pSentenceParticleUnreadable == value)
            {
                return;
            }

            _pSentenceParticleUnreadable = value;
            PSentenceRaise(nameof(PSentenceParticleUnreadable));
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
            PSentenceDependenceUnreadable = false;
            PSentenceRaise(nameof(PSentenceDependence));
            PSentenceRaise(nameof(PSentenceFrameVisible));
            PSentenceRaise(nameof(PSentenceFrameGap));
        }
    }

    public bool PSentenceDependenceUnreadable
    {
        get => _pSentenceDependenceUnreadable;
        private set
        {
            if (_pSentenceDependenceUnreadable == value)
            {
                return;
            }

            _pSentenceDependenceUnreadable = value;
            PSentenceRaise(nameof(PSentenceDependenceUnreadable));
            PSentenceRaise(nameof(PSentenceFrameGap));
        }
    }

    public bool PSentenceFrameVisible
    {
        get => _pSentenceFrameVisible
            || _pSentenceParticle.Length > 0
            || _pSentenceDependence.Length > 0
            || _pSentenceParticleUnreadable
            || _pSentenceDependenceUnreadable;
        set
        {
            _pSentenceFrameVisible = value;
            PSentenceRaise(nameof(PSentenceFrameVisible));
        }
    }

    public string PSentenceFrameGap
    {
        get
        {
            bool particle = _pSentenceParticle.Length > 0 || _pSentenceParticleUnreadable;
            bool dependence = _pSentenceDependence.Length > 0 || _pSentenceDependenceUnreadable;
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

    internal LExampleDraft PSentenceDraftRead()
    {
        return new LExampleDraft(
            PSentenceTextRead(),
            _pSentenceId,
            PSentenceCitationRead(),
            PSentenceParticleRead(),
            PSentenceDependenceRead());
    }

    internal LStateValue PSentenceParticleRead()
    {
        return LStateValue.LStateValueResolve(_pSentenceParticle, _pSentenceParticleUnreadable);
    }

    internal LStateValue PSentenceDependenceRead()
    {
        return LStateValue.LStateValueResolve(_pSentenceDependence, _pSentenceDependenceUnreadable);
    }

    internal LStateValue PSentenceTextRead()
    {
        return LStateValue.LStateValueResolve(_pSentenceText, _pSentenceUnreadable);
    }

    internal LStateValue PSentenceCitationRead()
    {
        return LStateValue.LStateValueResolve(_pSentenceCitation, _pSentenceCitationUnreadable);
    }

    internal bool PSentenceCheck()
    {
        return !PSentenceTextRead().LStateValueEmpty
            || !PSentenceParticleRead().LStateValueEmpty
            || !PSentenceDependenceRead().LStateValueEmpty;
    }

    internal void PSentenceIdentityApply()
    {
        if (PSentenceTextRead().LStateValueEmpty)
        {
            PSentenceId = string.Empty;
            return;
        }

        if (_pSentenceId.Length != 0)
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
        PSentenceParticle = string.Empty;
        PSentenceDependence = string.Empty;
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
