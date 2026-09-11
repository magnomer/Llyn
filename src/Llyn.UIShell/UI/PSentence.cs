using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PSentence : INotifyPropertyChanged
{
    private readonly LEngine _pSentenceEngine;
    private long _pSentenceId;
    private string _pSentenceText;
    private bool _pSentenceUnreadable;
    private long _pSentenceCitation;
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
    private long _pSentenceRow;
    private LStateValue _pSentenceTranslation = LStateValue.LStateValueUnspecified;
    private string _pSentenceLanguage = string.Empty;

    internal PSentence(
        LEngine engine,
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences)
        : this(engine, catalog, particles, dependences, LSentenceDraft.LSentenceDraftCreate(string.Empty))
    {
    }

    internal PSentence(
        LEngine engine,
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        LSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(draft);

        _pSentenceEngine = engine;

        LExampleDraft example = draft.LSentenceDraftExample
            ?? LExampleDraft.LExampleDraftCreate(string.Empty);

        PSentenceCitationCatalog = catalog;
        PSentenceParticleCatalog = particles;
        PSentenceDependenceCatalog = dependences;
        _pSentenceRow = draft.LSentenceDraftId;
        _pSentenceId = example.LExampleDraftId;
        _pSentenceText = example.LExampleDraftText.LStateValueShow();
        _pSentenceUnreadable = example.LExampleDraftText.LStateValueState == LState.LStateUnknown;
        _pSentenceCitation = example.LExampleDraftReference.LStateAnchorShow();
        _pSentenceCitationUnreadable = example.LExampleDraftReference.LStateAnchorState == LState.LStateUnknown;
        _pSentenceCitationName = PSentenceCitationFind(_pSentenceCitation);
        _pSentenceTranslation = example.LExampleDraftTranslation;
        _pSentenceLanguage = example.LExampleDraftLanguage;
        _pSentenceParticle = draft.LSentenceDraftParticle.LStateValueShow();
        _pSentenceParticleUnreadable = draft.LSentenceDraftParticle.LStateValueState == LState.LStateUnknown;
        _pSentenceDependence = draft.LSentenceDraftDependence.LStateValueShow();
        _pSentenceDependenceUnreadable = draft.LSentenceDraftDependence.LStateValueState == LState.LStateUnknown;
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

    public long PSentenceCitation
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
            PSentenceRaise(nameof(PSentenceFrameWritten));
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
            PSentenceDependenceUnreadable = false;
            PSentenceRaise(nameof(PSentenceDependence));
            PSentenceRaise(nameof(PSentenceFrameVisible));
            PSentenceRaise(nameof(PSentenceFrameWritten));
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
        || _pSentenceParticleUnreadable
        || _pSentenceDependenceUnreadable;

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

    internal LSentenceDraft PSentenceDraftRead()
    {
        LStateValue text = PSentenceTextRead();
        LExampleDraft? example = text.LStateValueEmpty && _pSentenceId == 0
            ? null
            : new LExampleDraft(
                text,
                _pSentenceId,
                PSentenceCitationRead(),
                _pSentenceTranslation,
                _pSentenceLanguage);

        return new LSentenceDraft(
            example, PSentenceParticleRead(), PSentenceDependenceRead(), _pSentenceRow);
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

    internal LStateAnchor PSentenceCitationRead()
    {
        return _pSentenceCitationUnreadable
            ? LStateAnchor.LStateAnchorUnknown
            : LStateAnchor.LStateAnchorRead(_pSentenceCitation);
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
            PSentenceId = 0;
            return;
        }

        if (_pSentenceId != 0)
        {
            return;
        }

        PSentenceId = _pSentenceEngine.LEngineIdentityCreate();
    }

    internal void PSentenceClear()
    {
        _pSentenceRow = 0;
        PSentenceText = string.Empty;
        PSentenceUnreadable = false;
        PSentenceCitation = 0;
        PSentenceCitationUnreadable = false;
        PSentenceId = 0;
        PSentenceParticle = string.Empty;
        PSentenceDependence = string.Empty;
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
