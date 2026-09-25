using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LLecternSound
{
    private readonly LDisplaySound _lLecternSoundDisplay;

    private readonly ObservableCollection<LTranscriptionItem> _lLecternSoundTranscription = [];

    private readonly ObservableCollection<LGlyphItem> _lLecternSoundGlyph = [];

    private LWindow _lLecternSoundWindow = null!;

    private UIElement _lLecternSoundSection = null!;

    private ColumnDefinition _lLecternSoundGutter = null!;

    private TextBlock _lLecternSoundHeading = null!;

    private ItemsControl _lLecternSoundStrip = null!;

    private Action<string, string> _lLecternSoundOpening = null!;

    private readonly ObservableCollection<LReflexItem> _lLecternSoundReflex = [];

    private UIElement _lLecternSoundLoading = null!;

    private ToggleButton _lLecternSoundFold = null!;

    private DependencyObject _lLecternSoundFanqie = null!;

    private TextBlock _lLecternSoundReading = null!;

    private Action<IReadOnlyList<LFanqieGroup>, bool> _lLecternSoundRime = null!;

    private Action<string, string, string> _lLecternSoundDiwei = null!;

    private Action<string, string?> _lLecternSoundStem = null!;

    private DependencyObject _lLecternSoundScript = null!;

    private Action<IReadOnlyList<LScriptGroup>, bool> _lLecternSoundWriting = null!;

    private DependencyObject _lLecternSoundParadigm = null!;

    private Action<IReadOnlyList<LParadigmSlot>, bool, bool> _lLecternSoundInflection = null!;

    public LLecternSound(LDisplaySound display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternSoundDisplay = display;
    }

    public void LLecternGlyphAttach(
        LWindow window,
        ItemsControl transcriptions,
        UIElement section,
        ColumnDefinition lead,
        TextBlock label,
        ItemsControl glyph,
        Action<string, string> glyphSeam)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(transcriptions);
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(lead);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(glyph);
        ArgumentNullException.ThrowIfNull(glyphSeam);

        _lLecternSoundWindow = window;
        _lLecternSoundSection = section;
        _lLecternSoundGutter = lead;
        _lLecternSoundHeading = label;
        _lLecternSoundStrip = glyph;
        _lLecternSoundOpening = glyphSeam;
        transcriptions.ItemsSource = _lLecternSoundTranscription;
        glyph.ItemsSource = _lLecternSoundGlyph;
    }

    public void LLecternReflexAttach(ItemsControl reflex, UIElement loading, ToggleButton fold)
    {
        ArgumentNullException.ThrowIfNull(reflex);
        ArgumentNullException.ThrowIfNull(loading);
        ArgumentNullException.ThrowIfNull(fold);

        _lLecternSoundLoading = loading;
        _lLecternSoundFold = fold;
        reflex.ItemsSource = _lLecternSoundReflex;
    }

    public void LLecternFanqieAttach(
        DependencyObject fanqie,
        TextBlock reading,
        Action<IReadOnlyList<LFanqieGroup>, bool> fanqieSeam,
        Action<string, string, string> diweiSeam,
        Action<string, string?> stemSeam)
    {
        ArgumentNullException.ThrowIfNull(fanqie);
        ArgumentNullException.ThrowIfNull(reading);
        ArgumentNullException.ThrowIfNull(fanqieSeam);
        ArgumentNullException.ThrowIfNull(diweiSeam);
        ArgumentNullException.ThrowIfNull(stemSeam);

        _lLecternSoundFanqie = fanqie;
        _lLecternSoundReading = reading;
        _lLecternSoundRime = fanqieSeam;
        _lLecternSoundDiwei = diweiSeam;
        _lLecternSoundStem = stemSeam;
    }

    public void LLecternScriptAttach(DependencyObject script, Action<IReadOnlyList<LScriptGroup>, bool> scriptSeam)
    {
        ArgumentNullException.ThrowIfNull(script);
        ArgumentNullException.ThrowIfNull(scriptSeam);

        _lLecternSoundScript = script;
        _lLecternSoundWriting = scriptSeam;
    }

    public void LLecternParadigmAttach(
        DependencyObject paradigm, Action<IReadOnlyList<LParadigmSlot>, bool, bool> paradigmSeam)
    {
        ArgumentNullException.ThrowIfNull(paradigm);
        ArgumentNullException.ThrowIfNull(paradigmSeam);

        _lLecternSoundParadigm = paradigm;
        _lLecternSoundInflection = paradigmSeam;
    }

    public void LLecternReflexUpdate()
    {
        if (_lLecternSoundDisplay.LDisplayEntry is null)
        {
            return;
        }

        _lLecternSoundDisplay.LDisplayReflexLoad();
        LLecternReflexShow();
        LLecternAnchorShow(_lLecternSoundDisplay.LDisplayFanqieDivide());
        LLecternPendingShow();
    }

    public void LLecternFanqieUpdate()
    {
        if (_lLecternSoundDisplay.LDisplayEntry is not null)
        {
            LLecternFanqieShow(_lLecternSoundDisplay.LDisplayFanqieDivide());
        }
    }

    public void LLecternScriptUpdate()
    {
        if (_lLecternSoundDisplay.LDisplayEntry is not null)
        {
            LLecternScriptShow();
        }
    }

    public void LLecternParadigmUpdate()
    {
        if (_lLecternSoundDisplay.LDisplayEntry is not null)
        {
            LLecternParadigmShow();
        }
    }

    public void LLecternFoldHandle(bool opened)
    {
        _lLecternSoundDisplay.LDisplayFoldSet(opened);
        LReflexItem.LReflexFoldApply(
            _lLecternSoundReflex, _lLecternSoundFold, _lLecternSoundDisplay.LDisplayFoldOpened);
    }

    public void LLecternDiweiShow(string kind, string key)
    {
        if (_lLecternSoundDisplay.LDisplayShown is LEntryDraft draft)
        {
            _lLecternSoundDiwei(draft.LEntryDraftLanguage, kind, key);
        }
    }

    public void LLecternStemShow(string? key)
    {
        if (_lLecternSoundDisplay.LDisplayShown is LEntryDraft draft)
        {
            _lLecternSoundStem(draft.LEntryDraftLanguage, key);
        }
    }

    public void LLecternFanqieSet(long fanqieId, int rank)
    {
        _lLecternSoundDisplay.LDisplayFanqieSet(fanqieId, rank);
    }

    public void LLecternSoundShow()
    {
        LLecternGlyphShow();
        LLecternTranscriptionShow();
        _lLecternSoundDisplay.LDisplayReflexStart(_lLecternSoundDisplay.LDisplayEntry);
        LLecternReflexShow();
        LLecternPendingShow();
        LLecternParadigmShow();
        _lLecternSoundDisplay.LDisplayScriptStart();
        LLecternScriptShow();
        _lLecternSoundDisplay.LDisplayFanqieStart();
        LLecternFanqieShow(_lLecternSoundDisplay.LDisplayFanqieDivide());
    }

    public void LLecternSoundClear()
    {
        _lLecternSoundTranscription.Clear();
        _lLecternSoundGlyph.Clear();
        _lLecternSoundHeading.Text = string.Empty;
        _lLecternSoundSection.Visibility = Visibility.Collapsed;
        _lLecternSoundGutter.SharedSizeGroup = null;
        _lLecternSoundReflex.Clear();
        _lLecternSoundFold.Visibility = Visibility.Collapsed;
        _lLecternSoundLoading.Visibility = Visibility.Collapsed;
        _lLecternSoundInflection([], false, false);
        _lLecternSoundWriting([], _lLecternSoundDisplay.LDisplayScriptCheck(null));
        _lLecternSoundRime([], _lLecternSoundDisplay.LDisplayFanqieCheck(null));
        _lLecternSoundReading.Text = string.Empty;
    }

    public void LLecternGlyphHandle(object parameter)
    {
        if (parameter is LGlyphItem item && item.LGlyphItemLanguage.Length > 0)
        {
            _lLecternSoundOpening(item.LGlyphItemText, item.LGlyphItemLanguage);
        }
    }

    private void LLecternGlyphShow()
    {
        _lLecternSoundGlyph.Clear();

        LGlyph? section = _lLecternSoundDisplay.LDisplayGlyphRead();
        if (section is null)
        {
            _lLecternSoundHeading.Text = string.Empty;
            _lLecternSoundSection.Visibility = Visibility.Collapsed;
            _lLecternSoundGutter.SharedSizeGroup = null;
            return;
        }

        foreach (LGlyphCell cell in _lLecternSoundDisplay.LDisplayGlyphDivide())
        {
            _lLecternSoundGlyph.Add(new LGlyphItem(cell.LGlyphCellText, cell.LGlyphCellLanguage));
        }

        _lLecternSoundHeading.Text = LTranscriptionItem.LTranscriptionLabelFormat(section.LGlyphName);
        LFontFace.LFontGlyphApply(
            _lLecternSoundStrip.Resources,
            _lLecternSoundWindow,
            _lLecternSoundDisplay.LDisplayShown!.LEntryDraftLanguage);
        _lLecternSoundSection.Visibility = _lLecternSoundGlyph.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        _lLecternSoundGutter.SharedSizeGroup = _lLecternSoundGlyph.Count == 0 ? null : "PReadingLabel";
    }

    private void LLecternTranscriptionShow()
    {
        _lLecternSoundTranscription.Clear();
        foreach (LTranscriptionDraft spelled in _lLecternSoundDisplay.LDisplayTranscriptionRead())
        {
            _lLecternSoundTranscription.Add(LTranscriptionItem.LTranscriptionItemCreate(spelled));
        }
    }

    private void LLecternReflexShow()
    {
        _lLecternSoundReflex.Clear();
        HashSet<string> folded = LReflexItem.LReflexFoldRead(
            _lLecternSoundWindow, _lLecternSoundDisplay.LDisplayShown!.LEntryDraftLanguage);
        foreach (LReflexItem row in LReflexItem.LReflexItemScan(
            _lLecternSoundWindow, _lLecternSoundDisplay.LDisplayReflexRead(), folded))
        {
            _lLecternSoundReflex.Add(row);
        }

        LReflexItem.LReflexLeadApply(_lLecternSoundReflex);
        LReflexItem.LReflexFoldApply(
            _lLecternSoundReflex, _lLecternSoundFold, _lLecternSoundDisplay.LDisplayFoldOpened);
    }

    private void LLecternAnchorShow(IReadOnlyList<LFanqieGroup> groups)
    {
        LReflexItem.LReflexAnchorApply(
            _lLecternSoundWindow,
            _lLecternSoundReflex,
            LDisplaySound.LDisplayAnchorRead(groups),
            _lLecternSoundDisplay.LDisplayShown!.LEntryDraftHeadword);
    }

    private void LLecternPendingShow()
    {
        _lLecternSoundLoading.Visibility =
            _lLecternSoundDisplay.LDisplayReflexCheck(_lLecternSoundDisplay.LDisplayEntry)
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    private void LLecternFanqieShow(IReadOnlyList<LFanqieGroup> groups)
    {
        LLecternAnchorShow(groups);
        LFontFace.LFontApply(
            _lLecternSoundWindow,
            _lLecternSoundDisplay.LDisplayShown!.LEntryDraftLanguage,
            LFontRole.LFontRoleGlyph,
            _lLecternSoundFanqie);
        _lLecternSoundRime(groups, _lLecternSoundDisplay.LDisplayFanqieCheck(_lLecternSoundDisplay.LDisplayEntry));
        _lLecternSoundReading.Text = _lLecternSoundDisplay.LDisplayReadingRead();
    }

    private void LLecternScriptShow()
    {
        LFontFace.LFontApply(
            _lLecternSoundWindow,
            _lLecternSoundDisplay.LDisplayShown!.LEntryDraftLanguage,
            LFontRole.LFontRoleGlyph,
            _lLecternSoundScript);
        _lLecternSoundWriting(
            _lLecternSoundDisplay.LDisplayScriptDivide(),
            _lLecternSoundDisplay.LDisplayScriptCheck(_lLecternSoundDisplay.LDisplayEntry));
    }

    private void LLecternParadigmShow()
    {
        IReadOnlyList<LParadigmSlot> slots = _lLecternSoundDisplay.LDisplayParadigmRead();
        _lLecternSoundDisplay.LDisplayInflectionStart();
        LFontFace.LFontApply(
            _lLecternSoundWindow, _lLecternSoundDisplay.LDisplayLanguageRead(), _lLecternSoundParadigm);
        _lLecternSoundInflection(
            slots,
            _lLecternSoundDisplay.LDisplayParadigmCheck(_lLecternSoundDisplay.LDisplayEntry),
            _lLecternSoundDisplay.LDisplayMorphologyRead());
    }
}
