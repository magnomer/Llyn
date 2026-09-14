using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PTranscriptionItem> _pGlyphItem = [];
    private LGlyph? _pGlyph;

    internal async void PGlyphNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PTranscriptionItem row && _pGlyph is not null)
        {
            await PNotationOpen(e.OriginalSource as UIElement ?? PGlyph, row.PTranscriptionItemId, _pGlyph.LGlyphName);
        }
    }

    private bool PGlyphSchemeCheck(string scheme)
    {
        return _pGlyph is not null && string.Equals(scheme, _pGlyph.LGlyphName, StringComparison.Ordinal);
    }

    private void PGlyphShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        _pGlyph = language.Length == 0 ? null : _lEngine.LEngineGlyphRead(language);
        PGlyph.Visibility = _pGlyph is null ? Visibility.Collapsed : Visibility.Visible;
        PGlyph.Tag = _pGlyph is not null && _pGlyph.LGlyphSources.Count > 0;
        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PGlyph);

        List<LTranscriptionDraft> rows = [];
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (PGlyphSchemeCheck(spelled.LTranscriptionDraftScheme))
            {
                rows.Add(spelled);
            }
        }

        PCard.PCardRowShow(
            _pGlyphItem,
            rows,
            static row => row.PTranscriptionItemId,
            static spelled => spelled.LTranscriptionDraftId,
            PTranscriptionCreate,
            PTranscriptionUpdate);
    }

    private void PGlyphPrepare(LEntryDraft draft)
    {
        if (_pGlyph is null)
        {
            return;
        }

        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (PGlyphSchemeCheck(spelled.LTranscriptionDraftScheme))
            {
                return;
            }
        }

        PEditorRequestSend(new LRequestTranscriptionAddition(
            _pEditorDraft, _pGlyph.LGlyphName, draft.LEntryDraftTranscriptions.Count, true));
    }

    private void PGlyphClear()
    {
        foreach (PTranscriptionItem row in _pGlyphItem)
        {
            row.PropertyChanged -= PTranscriptionChangeHandle;
        }

        _pGlyphItem.Clear();
        _pGlyph = null;
        PGlyph.Visibility = Visibility.Collapsed;
    }
}
