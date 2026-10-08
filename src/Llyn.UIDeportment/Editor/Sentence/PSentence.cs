using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PSentence : INotifyPropertyChanged
{
    private CStateWording _pSentenceText;
    private long? _pSentenceCitation;
    private long _pSentenceRow;

    internal PSentence(
        ObservableCollection<QCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages,
        CSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        CExampleDraft? example = draft.CSentenceDraftExample;

        PSentenceCitationCatalog = catalog;
        PSentenceParticleCatalog = particles;
        PSentenceDependenceCatalog = dependences;
        PSentenceLanguageCatalog = languages;
        _pSentenceRow = draft.CSentenceDraftId;
        _pSentenceText = draft.CSentenceDraftText;
        PSentenceCitation = example?.CExampleDraftReference;
        PSentenceCited = draft.CSentenceDraftCited;
        PSentenceFrame = new(draft.CSentenceDraftParticle, draft.CSentenceDraftDependence);
        PSentenceFrame.PropertyChanged += (_, e) => PSentenceRaise(e.PropertyName);
        PSentenceGlossShow(example?.CExampleDraftGloss ?? []);
    }

    public ObservableCollection<QCitationItem> PSentenceCitationCatalog { get; }

    public ObservableCollection<PLanguageItem> PSentenceLanguageCatalog { get; }

    public ObservableCollection<string> PSentenceParticleCatalog { get; }

    public ObservableCollection<string> PSentenceDependenceCatalog { get; }

    public PSentenceFrame PSentenceFrame { get; }

    public ObservableCollection<PGloss> PSentenceGloss { get; } = [];

    public PMentionLine PSentenceChip { get; } = new();

    internal event Action<PGloss, string>? PSentenceGlossNotice;

    internal long PSentenceRow => _pSentenceRow;

    internal bool PSentenceCited { get; private set; }

    public CStateWording PSentenceText
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

    public long? PSentenceCitation
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

    internal void PSentenceShow(CSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        CExampleDraft? example = draft.CSentenceDraftExample;

        _pSentenceRow = draft.CSentenceDraftId;
        PSentenceGlossShow(example?.CExampleDraftGloss ?? []);
        PSentenceText = draft.CSentenceDraftText;
        PSentenceCitation = example?.CExampleDraftReference;
        PSentenceCited = draft.CSentenceDraftCited;
        PSentenceFrame.PSentenceFrameShow(draft.CSentenceDraftParticle, draft.CSentenceDraftDependence);
    }

    internal void PSentenceCitationShow()
    {
        PSentenceRaise(nameof(PSentenceCitation));
    }

    internal static string PSentenceCitationFind(ObservableCollection<QCitationItem> catalog, long? anchor)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        if (anchor is not long id)
        {
            return string.Empty;
        }

        foreach (QCitationItem row in catalog)
        {
            if (row.QCitationItemId == id)
            {
                return row.QCitationItemName;
            }
        }

        return string.Empty;
    }

    private void PSentenceGlossShow(IReadOnlyList<CGlossDraft> drafts)
    {
        QLookItem.QLookItemShow(
            PSentenceGloss,
            drafts,
            static row => row.PGlossId,
            static draft => draft.CGlossDraftId,
            PSentenceGlossCreate,
            (row, draft) =>
            {
                row.PGlossShow(draft);
                return row;
            });
    }

    private PGloss PSentenceGlossCreate(CGlossDraft draft)
    {
        PGloss row = new(PSentenceLanguageCatalog, draft);
        row.PGlossPicked += (gloss, language) => PSentenceGlossNotice?.Invoke(gloss, language);
        return row;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PSentenceRaise(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
