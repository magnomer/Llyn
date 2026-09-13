using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private const string PGlossLanguage = "English";

    private readonly ObservableCollection<PGloss> _pTranscriptGloss = [];

    private readonly ObservableCollection<PGloss> _pExcerptGloss = [];

    private readonly HashSet<long> _pTranscriptGlossDirty = [];

    private void PExcerptGlossShow(IReadOnlyList<LGloss> glosses)
    {
        _pExcerptGloss.Clear();
        foreach (LGloss gloss in glosses)
        {
            _pExcerptGloss.Add(new PGloss(_pLanguageItem, LGlossDraft.LGlossDraftCreate(gloss)));
        }

        PExcerptValueShow(PExcerptTranslation, LStateValue.LStateValueUnspecified);
        PExcerptTranslation.Visibility = glosses.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PTranscriptGlossShow(LExample? example)
    {
        List<LGlossDraft> drafts = [];
        foreach (LGloss gloss in example?.LExampleGloss ?? [])
        {
            drafts.Add(LGlossDraft.LGlossDraftCreate(gloss));
        }

        PCard.PCardRowShow(
            _pTranscriptGloss,
            drafts,
            static row => row.PGlossId,
            static draft => draft.LGlossDraftId,
            PTranscriptGlossCreate,
            (row, draft) =>
            {
                row.PGlossShow(draft, _ => _pTranscriptGlossDirty.Contains(row.PGlossId));
                return row;
            });
    }

    private PGloss PTranscriptGlossCreate(LGlossDraft draft)
    {
        PGloss row = new(_pLanguageItem, draft);
        row.PropertyChanged += PTranscriptGlossChange;
        return row;
    }

    private void PTranscriptGlossChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (_pTranscriptLoading || sender is not PGloss gloss)
        {
            return;
        }

        switch (arguments.PropertyName)
        {
            case nameof(PGloss.PGlossText):
                _pTranscriptGlossDirty.Add(gloss.PGlossId);
                PTranscriptChangeDefer();
                break;
            case nameof(PGloss.PGlossLanguage):
                PTranscriptRequestSend(
                    new LRequestGlossLanguage(_pTranscriptDraft, 0, 0, gloss.PGlossId, gloss.PGlossLanguage));
                break;
        }
    }

    private IReadOnlyList<LRequest> PTranscriptGlossRead(long draft)
    {
        List<LRequest> requests = [];
        foreach (PGloss gloss in _pTranscriptGloss)
        {
            if (_pTranscriptGlossDirty.Contains(gloss.PGlossId))
            {
                requests.Add(new LRequestGlossText(draft, 0, 0, gloss.PGlossId, gloss.PGlossTextRead()));
            }
        }

        _pTranscriptGlossDirty.Clear();
        return requests;
    }

    private void PGlossAddHandle(object sender, RoutedEventArgs e)
    {
        PTranscriptRequestSend(new LRequestGlossAddition(
            _pTranscriptDraft, 0, 0, PGlossLanguageRead(), _pTranscriptGloss.Count));
    }

    private void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PGloss gloss)
        {
            PTranscriptRequestSend(new LRequestGlossRemoval(_pTranscriptDraft, 0, 0, gloss.PGlossId));
        }
    }

    private string PGlossLanguageRead()
    {
        foreach (PLanguageItem item in _pLanguageItem)
        {
            if (string.Equals(item.PLanguageItemName, PGlossLanguage, StringComparison.Ordinal))
            {
                return item.PLanguageItemName;
            }
        }

        return _pLanguageItem.Count > 0 ? _pLanguageItem[0].PLanguageItemName : string.Empty;
    }
}
