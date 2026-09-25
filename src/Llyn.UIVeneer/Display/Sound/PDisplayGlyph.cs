using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly ObservableCollection<LGlyphItem> _pDisplayGlyph = [];
    private LGlyph? _pDisplayGlyphSection;

    private void PDisplayGlyphShow(LEntryDraft draft)
    {
        _pDisplayGlyph.Clear();

        string language = draft.LEntryDraftLanguage;
        _pDisplayGlyphSection = _lLectern.LLecternGlyphRead(language);
        if (_pDisplayGlyphSection is null)
        {
            PDisplayGlyphClear();
            return;
        }

        string text = draft.LEntryDraftHeadword;
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (spelled.LTranscriptionDraftEmpty)
            {
                continue;
            }

            if (PDisplayGlyphCheck(spelled.LTranscriptionDraftScheme))
            {
                text = spelled.LTranscriptionDraftText;
                break;
            }
        }

        foreach (Rune rune in text.EnumerateRunes())
        {
            string character = rune.ToString();
            string target = LGlyph.LGlyphSingleCheck(character)
                ? _pDisplayGlyphSection.LGlyphLanguage
                : string.Empty;
            _pDisplayGlyph.Add(new LGlyphItem(character, target));
        }

        PDisplayGlyphLabel.Text = LTranscriptionItem.LTranscriptionLabelFormat(_pDisplayGlyphSection.LGlyphName);
        LFontFace.LFontGlyphApply(PDisplayGlyph.Resources, _pDisplayHost.PWindowDeportment, language);
        PDisplayGlyphSection.Visibility = _pDisplayGlyph.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        PDisplayGlyphLead.SharedSizeGroup = _pDisplayGlyph.Count == 0 ? null : "PReadingLabel";
    }

    private bool PDisplayGlyphCheck(string scheme)
    {
        return _pDisplayGlyphSection is not null
            && string.Equals(scheme, _pDisplayGlyphSection.LGlyphName, StringComparison.Ordinal);
    }

    private void PDisplayGlyphHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LGlyphItem item && item.LGlyphItemLanguage.Length > 0)
        {
            _pDisplayHost.PWindowGlyphShow(item.LGlyphItemText, item.LGlyphItemLanguage);
        }
    }

    private void PDisplayGlyphClear()
    {
        _pDisplayGlyph.Clear();
        _pDisplayGlyphSection = null;
        PDisplayGlyphLabel.Text = string.Empty;
        PDisplayGlyphSection.Visibility = Visibility.Collapsed;
        PDisplayGlyphLead.SharedSizeGroup = null;
    }
}
