using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<LTranscriptionItem> _pGlyphItem = [];
    private LGlyph? _pGlyph;

    internal async void PGlyphNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LTranscriptionItem row && _pGlyph is not null)
        {
            await PNotationOpen(e.OriginalSource as UIElement ?? PGlyph, row.LTranscriptionItemId, _pGlyph.LGlyphName);
        }
    }

    private bool PGlyphSchemeCheck(string scheme)
    {
        return PGlyphSchemeCheck(_pGlyph, scheme);
    }

    private static bool PGlyphSchemeCheck(LGlyph? glyph, string scheme)
    {
        return glyph is not null && string.Equals(scheme, glyph.LGlyphName, StringComparison.Ordinal);
    }

    private void PGlyphShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        _pGlyph = _pEditorHost.PWindowDeportment.LWindowGlyphRead(language);
        PGlyph.Visibility = _pGlyph is null ? Visibility.Collapsed : Visibility.Visible;
        PGlyph.Tag = _pGlyph?.LGlyphSourced ?? false;
        LFontFace.LFontGlyphApply(PGlyph.Resources, _pEditorHost.PWindowDeportment, language);

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
            static row => row.LTranscriptionItemId,
            static spelled => spelled.LTranscriptionDraftId,
            PTranscriptionCreate,
            PTranscriptionUpdate);
    }
}
