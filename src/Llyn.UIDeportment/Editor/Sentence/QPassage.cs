using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QPassage : INotifyPropertyChanged
{
    private CStateWording _qPassageText;
    private long? _qPassageCitation;

    internal QPassage(
        ObservableCollection<QCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<QLanguageItem> languages,
        CSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        CExampleDraft? example = draft.CSentenceDraftExample;

        QPassageCitationCatalog = catalog;
        QPassageParticleCatalog = particles;
        QPassageDependenceCatalog = dependences;
        QPassageLanguageCatalog = languages;
        QPassageRow = draft.CSentenceDraftId;
        QPassageLine = draft.CSentenceDraftLine;
        _qPassageText = draft.CSentenceDraftText;
        QPassageCitation = example?.CExampleDraftReference;
        QPassageCited = draft.CSentenceDraftCited;
        QSentenceFrame = new(draft.CSentenceDraftParticle, draft.CSentenceDraftDependence);
        QSentenceFrame.PropertyChanged += (_, e) => QPassageRaise(e.PropertyName);
        QPassageGlossShow(example?.CExampleDraftGloss ?? []);
    }

    public ObservableCollection<QCitationItem> QPassageCitationCatalog { get; }

    public ObservableCollection<QLanguageItem> QPassageLanguageCatalog { get; }

    public ObservableCollection<string> QPassageParticleCatalog { get; }

    public ObservableCollection<string> QPassageDependenceCatalog { get; }

    public QSentenceFrame QSentenceFrame { get; }

    public ObservableCollection<QRendition> QPassageGloss { get; } = [];

    public QMentionLine QPassageChip { get; } = new();

    internal event Action<QRendition, string>? QPassageGlossNotice;

    internal long QPassageRow { get; }

    internal long QPassageLine { get; private set; }

    internal bool QPassageCited { get; private set; }

    public CStateWording QPassageText
    {
        get => _qPassageText;
        private set
        {
            if (_qPassageText == value)
            {
                return;
            }

            _qPassageText = value;
            QPassageRaise(nameof(QPassageText));
        }
    }

    public long? QPassageCitation
    {
        get => _qPassageCitation;
        private set
        {
            if (_qPassageCitation == value)
            {
                return;
            }

            _qPassageCitation = value;
            QPassageRaise(nameof(QPassageCitation));
        }
    }

    internal void QPassageShow(CSentenceDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        CExampleDraft? example = draft.CSentenceDraftExample;

        QPassageGlossShow(example?.CExampleDraftGloss ?? []);
        QPassageLine = draft.CSentenceDraftLine;
        QPassageText = draft.CSentenceDraftText;
        QPassageCitation = example?.CExampleDraftReference;
        QPassageCited = draft.CSentenceDraftCited;
        QSentenceFrame.QSentenceFrameShow(draft.CSentenceDraftParticle, draft.CSentenceDraftDependence);
    }

    internal void QPassageCitationShow()
    {
        QPassageRaise(nameof(QPassageCitation));
    }

    internal static string QPassageCitationFind(ObservableCollection<QCitationItem> catalog, long? anchor)
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

    private void QPassageGlossShow(IReadOnlyList<CGlossDraft> drafts)
    {
        QLookItem.QLookItemShow(
            QPassageGloss,
            drafts,
            static row => row.QRenditionId,
            static draft => draft.CGlossDraftId,
            QPassageGlossCreate,
            (row, draft) =>
            {
                row.QRenditionShow(draft);
                return row;
            });
    }

    private QRendition QPassageGlossCreate(CGlossDraft draft)
    {
        QRendition row = new(QPassageLanguageCatalog, draft);
        row.QRenditionPicked += (gloss, language) => QPassageGlossNotice?.Invoke(gloss, language);
        return row;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void QPassageRaise(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
