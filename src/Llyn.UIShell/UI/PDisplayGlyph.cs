using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private readonly ObservableCollection<PGlyphItem> _pDisplayGlyph = [];
    private LGlyph? _pDisplayGlyphSection;

    private void PDisplayGlyphShow(LEntryDraft draft)
    {
        _pDisplayGlyph.Clear();

        string language = draft.LEntryDraftLanguage;
        _pDisplayGlyphSection = language.Length == 0 ? null : _lEngine.LEngineGlyphRead(language);
        if (_pDisplayGlyphSection is null)
        {
            PDisplayGlyphSection.Visibility = Visibility.Collapsed;
            return;
        }

        string text = draft.LEntryDraftHeadword;
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (PDisplayGlyphCheck(spelled.LTranscriptionDraftScheme) && !spelled.LTranscriptionDraftEmpty)
            {
                text = spelled.LTranscriptionDraftText;
                break;
            }
        }

        foreach (string character in LGlyph.LGlyphScan(text))
        {
            _pDisplayGlyph.Add(new PGlyphItem(character));
        }

        PDisplayGlyphLabel.Text = PTranscriptionItem.PTranscriptionLabelFormat(
            _pDisplayHost, _pDisplayGlyphSection.LGlyphName);
        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PDisplayGlyph);
        PDisplayGlyphSection.Visibility = _pDisplayGlyph.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private bool PDisplayGlyphCheck(string scheme)
    {
        return _pDisplayGlyphSection is not null
            && string.Equals(scheme, _pDisplayGlyphSection.LGlyphName, StringComparison.Ordinal);
    }

    private void PDisplayGlyphHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PGlyphItem item && _pDisplayGlyphSection is not null)
        {
            _pDisplayHost.PWindowGlyphShow(item.PGlyphItemText, _pDisplayGlyphSection.LGlyphLanguage);
        }
    }

    private void PDisplayGlyphClear()
    {
        _pDisplayGlyph.Clear();
        _pDisplayGlyphSection = null;
        PDisplayGlyphSection.Visibility = Visibility.Collapsed;
    }
}
