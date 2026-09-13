using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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

        PExcerptGlossSection.Visibility = glosses.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
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

        PTranscriptSeedShow();
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
        int position = sender is FrameworkElement { DataContext: PGloss row }
            ? _pTranscriptGloss.IndexOf(row) + 1
            : _pTranscriptGloss.Count;

        PTranscriptRequestSend(new LRequestGlossAddition(
            _pTranscriptDraft, 0, 0, PGlossLanguageRead(), position));
    }

    private void PGlossRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PGloss gloss })
        {
            PTranscriptRequestSend(new LRequestGlossRemoval(_pTranscriptDraft, 0, 0, gloss.PGlossId));
        }
    }

    private void PTranscriptSeedShow()
    {
        PTranscriptSeed.Visibility = _pTranscriptGloss.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PTranscriptSeedHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_pTranscriptGloss.Count > 0)
        {
            return;
        }

        PTranscriptRequestSend(new LRequestGlossAddition(_pTranscriptDraft, 0, 0, PGlossLanguageRead(), 0));

        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            if (PTranscriptGlossLine.ItemContainerGenerator.ContainerFromIndex(0) is DependencyObject container
                && PEditor.PEditorCaretFind(container) is TextBox box)
            {
                box.Focus();
            }
        });
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
