using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed partial class PSentence : INotifyPropertyChanged
{
    private long _pSentenceId;
    private LStateValue _pSentenceText;
    private LStateAnchor _pSentenceCitation;
    private LStateValue _pSentenceParticle;
    private LStateValue _pSentenceDependence;
    private int _pSentenceParticleColumn;
    private int _pSentenceDependenceColumn = 2;
    private bool _pSentenceFrameVisible;
    private long _pSentenceRow;

    internal PSentence(
        ObservableCollection<PCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages,
        LSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LExampleDraft example = draft.LSentenceDraftExample
            ?? LExampleDraft.LExampleDraftCreate(string.Empty);

        PSentenceCitationCatalog = catalog;
        PSentenceParticleCatalog = particles;
        PSentenceDependenceCatalog = dependences;
        PSentenceLanguageCatalog = languages;
        _pSentenceRow = draft.LSentenceDraftId;
        _pSentenceId = example.LExampleDraftId;
        _pSentenceText = example.LExampleDraftText;
        _pSentenceCitation = example.LExampleDraftReference;
        _pSentenceParticle = draft.LSentenceDraftParticle;
        _pSentenceDependence = draft.LSentenceDraftDependence;
        _pSentenceMention = example.LExampleDraftMention;
        PSentenceGlossShow(example.LExampleDraftGloss);
    }

    public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }

    public ObservableCollection<PLanguageItem> PSentenceLanguageCatalog { get; }

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

    public LStateValue PSentenceText
    {
        get => _pSentenceText;
        private set
        {
            if (_pSentenceText == value)
            {
                return;
            }

            _pSentenceText = value;
            PSentenceRaise(nameof(PSentenceText));
        }
    }

    public LStateAnchor PSentenceCitation
    {
        get => _pSentenceCitation;
        private set
        {
            if (_pSentenceCitation == value)
            {
                return;
            }

            _pSentenceCitation = value;
            PSentenceRaise(nameof(PSentenceCitation));
        }
    }

    public LStateValue PSentenceParticle
    {
        get => _pSentenceParticle;
        private set
        {
            if (_pSentenceParticle == value)
            {
                return;
            }

            _pSentenceParticle = value;
            PSentenceRaise(nameof(PSentenceParticle));
            PSentenceFrameRaise();
        }
    }

    public LStateValue PSentenceDependence
    {
        get => _pSentenceDependence;
        private set
        {
            if (_pSentenceDependence == value)
            {
                return;
            }

            _pSentenceDependence = value;
            PSentenceRaise(nameof(PSentenceDependence));
            PSentenceFrameRaise();
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
        PSentenceFrameCheck(_pSentenceParticle) || PSentenceFrameCheck(_pSentenceDependence);

    public string PSentenceFrameGap =>
        PSentenceFrameCheck(_pSentenceParticle) && PSentenceFrameCheck(_pSentenceDependence) ? " " : string.Empty;

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

    internal void PSentenceShow(LSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LExampleDraft example = draft.LSentenceDraftExample
            ?? LExampleDraft.LExampleDraftCreate(string.Empty);

        _pSentenceRow = draft.LSentenceDraftId;
        PSentenceId = example.LExampleDraftId;
        _pSentenceMention = example.LExampleDraftMention;
        PSentenceGlossShow(example.LExampleDraftGloss);
        PSentenceText = example.LExampleDraftText;
        PSentenceCitation = example.LExampleDraftReference;
        PSentenceParticle = draft.LSentenceDraftParticle;
        PSentenceDependence = draft.LSentenceDraftDependence;
    }

    internal void PSentenceCitationShow()
    {
        PSentenceRaise(nameof(PSentenceCitation));
    }

    internal static string PSentenceCitationFind(ObservableCollection<PCitationItem> catalog, LStateAnchor anchor)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(anchor);

        if (!anchor.LStateAnchorLinked)
        {
            return string.Empty;
        }

        foreach (PCitationItem row in catalog)
        {
            if (anchor.LStateAnchorMatch(row.PCitationItemId))
            {
                return row.PCitationItemName;
            }
        }

        return string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private static bool PSentenceFrameCheck(LStateValue value)
    {
        return value.LStateValueUncertain || value.LStateValueShown is not null;
    }

    private void PSentenceFrameRaise()
    {
        PSentenceRaise(nameof(PSentenceFrameVisible));
        PSentenceRaise(nameof(PSentenceFrameWritten));
        PSentenceRaise(nameof(PSentenceFrameGap));
    }

    private void PSentenceRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
