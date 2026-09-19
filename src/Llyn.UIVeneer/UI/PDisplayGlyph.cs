using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly ObservableCollection<PGlyphItem> _pDisplayGlyph = [];
    private LGlyph? _pDisplayGlyphSection;

    private void PDisplayGlyphShow(LEntryDraft draft)
    {
        _pDisplayGlyph.Clear();

        string language = draft.LEntryDraftLanguage;
        _pDisplayGlyphSection = _lEngine.LEngineGlyphRead(language);
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
            _pDisplayGlyph.Add(new PGlyphItem(character, target));
        }

        PDisplayGlyphLabel.Text = PTranscriptionItem.PTranscriptionLabelFormat(
            _pDisplayHost, _pDisplayGlyphSection.LGlyphName);
        PFont.PFontGlyphApply(PDisplayGlyph.Resources, _lEngine, language);
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
        if (e.Parameter is PGlyphItem item && item.PGlyphItemLanguage.Length > 0)
        {
            _pDisplayHost.PWindowGlyphShow(item.PGlyphItemText, item.PGlyphItemLanguage);
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
