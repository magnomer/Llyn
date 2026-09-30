using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternSound
{
    private readonly CDisplaySound _qLecternSoundArea;

    private readonly ObservableCollection<QTranscriptionItem> _qLecternSoundTranscription = [];

    private readonly ObservableCollection<QGlyphItem> _qLecternSoundGlyph = [];

    private readonly ObservableCollection<QReflexItem> _qLecternSoundReflex = [];

    private UIElement _qLecternSoundSection = null!;

    private ColumnDefinition _qLecternSoundGutter = null!;

    private TextBlock _qLecternSoundHeading = null!;

    private ItemsControl _qLecternSoundStrip = null!;

    private UIElement _qLecternSoundLoading = null!;

    private ToggleButton _qLecternSoundFold = null!;

    private DependencyObject _qLecternSoundFanqie = null!;

    private TextBlock _qLecternSoundReading = null!;

    private Action<IReadOnlyList<CFanqieGroup>, bool> _qLecternSoundRime = null!;

    private DependencyObject _qLecternSoundScript = null!;

    private Action<IReadOnlyList<CScriptGroup>, bool> _qLecternSoundWriting = null!;

    private DependencyObject _qLecternSoundParadigm = null!;

    private Action<IReadOnlyList<CParadigmSlot>> _qLecternSoundInflection = null!;

    public QLecternSound(CDisplaySound area)
    {
        ArgumentNullException.ThrowIfNull(area);

        _qLecternSoundArea = area;
    }

    public void QLecternGlyphIntroduce(
        ItemsControl transcriptions,
        UIElement section,
        ColumnDefinition lead,
        TextBlock label,
        ItemsControl glyph)
    {
        ArgumentNullException.ThrowIfNull(transcriptions);
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(lead);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(glyph);

        _qLecternSoundSection = section;
        _qLecternSoundGutter = lead;
        _qLecternSoundHeading = label;
        _qLecternSoundStrip = glyph;
        transcriptions.ItemsSource = _qLecternSoundTranscription;
        QLookItem.QLookItemAttach(transcriptions, QTranscriptionItem.QTranscriptionItemRefine);
        glyph.ItemsSource = _qLecternSoundGlyph;
        QLookItem.QLookItemAttach(glyph, QGlyphItem.QGlyphItemRefine);
    }

    public void QLecternReflexIntroduce(ItemsControl reflex, UIElement loading, ToggleButton fold)
    {
        ArgumentNullException.ThrowIfNull(reflex);
        ArgumentNullException.ThrowIfNull(loading);
        ArgumentNullException.ThrowIfNull(fold);

        _qLecternSoundLoading = loading;
        _qLecternSoundFold = fold;
        reflex.ItemsSource = _qLecternSoundReflex;
        QLookItem.QLookItemAttach(reflex, QReflexItem.QReflexItemRefine);
        _qLecternSoundArea.CDisplayFoldChanged += QLecternFoldRefine;
    }

    public void QLecternFanqieIntroduce(
        DependencyObject fanqie,
        TextBlock reading,
        Action<IReadOnlyList<CFanqieGroup>, bool> fanqieSeam)
    {
        ArgumentNullException.ThrowIfNull(fanqie);
        ArgumentNullException.ThrowIfNull(reading);
        ArgumentNullException.ThrowIfNull(fanqieSeam);

        _qLecternSoundFanqie = fanqie;
        _qLecternSoundReading = reading;
        _qLecternSoundRime = fanqieSeam;
    }

    public void QLecternScriptIntroduce(DependencyObject script, Action<IReadOnlyList<CScriptGroup>, bool> scriptSeam)
    {
        ArgumentNullException.ThrowIfNull(script);
        ArgumentNullException.ThrowIfNull(scriptSeam);

        _qLecternSoundScript = script;
        _qLecternSoundWriting = scriptSeam;
    }

    public void QLecternParadigmIntroduce(
        DependencyObject paradigm, Action<IReadOnlyList<CParadigmSlot>> paradigmSeam)
    {
        ArgumentNullException.ThrowIfNull(paradigm);
        ArgumentNullException.ThrowIfNull(paradigmSeam);

        _qLecternSoundParadigm = paradigm;
        _qLecternSoundInflection = paradigmSeam;
    }

    public void QLecternGlyphRefine()
    {
        CLecternGlyph glyph = _qLecternSoundArea.CDisplayGlyphRead();
        _qLecternSoundGlyph.Clear();
        foreach (CGlyphCell cell in glyph.CLecternGlyphCells)
        {
            _qLecternSoundGlyph.Add(
                new QGlyphItem(cell.CGlyphCellText, cell.CGlyphCellLanguage, cell.CGlyphCellLinked));
        }

        _qLecternSoundHeading.Text = glyph.CLecternGlyphShown
            ? QTranscriptionItem.QTranscriptionLabelRefine(glyph.CLecternGlyphKey, glyph.CLecternGlyphName)
            : string.Empty;
        QFontFace.QFontGlyphRefine(_qLecternSoundStrip.Resources, glyph.CLecternGlyphFont);
        _qLecternSoundSection.Visibility = QLook.QLookVisibleRead(glyph.CLecternGlyphShown);
        _qLecternSoundGutter.SharedSizeGroup = glyph.CLecternGlyphShown ? "PReadingLabel" : null;
    }

    public void QLecternTranscriptionRefine()
    {
        _qLecternSoundTranscription.Clear();
        foreach (CTranscriptionDraft spelled in _qLecternSoundArea.CDisplayTranscriptionRead())
        {
            _qLecternSoundTranscription.Add(QTranscriptionItem.QTranscriptionRowRefine(spelled));
        }
    }

    public void QLecternReflexRefine()
    {
        QLecternReflexRefine(_qLecternSoundArea.CDisplayReflexRead());
    }

    public void QLecternRenewalRefine()
    {
        QLecternReflexRefine(_qLecternSoundArea.CDisplayReflexResonate());
    }

    public void QLecternFoldRefine()
    {
        QReflexItem.QReflexFoldRefine(
            _qLecternSoundReflex, _qLecternSoundFold, _qLecternSoundArea.CDisplayFoldOpened);
    }

    public void QLecternFanqieRefine()
    {
        CLecternFanqie fanqie = _qLecternSoundArea.CDisplayFanqieRead();
        QReflexItem.QReflexAnchorRefine(
            _qLecternSoundReflex,
            fanqie.CLecternFanqieAnchor.CLecternAnchorOffered,
            fanqie.CLecternFanqieAnchor.CLecternAnchorTexts);
        QFontFace.QFontRefine(fanqie.CLecternFanqieFont, _qLecternSoundFanqie);
        _qLecternSoundRime(fanqie.CLecternFanqieGroups, fanqie.CLecternFanqiePending);
        _qLecternSoundReading.Text = fanqie.CLecternFanqieReading;
    }

    public void QLecternScriptRefine()
    {
        CLecternScript script = _qLecternSoundArea.CDisplayScriptRead();
        QFontFace.QFontRefine(script.CLecternScriptFont, _qLecternSoundScript);
        _qLecternSoundWriting(script.CLecternScriptGroups, script.CLecternScriptPending);
    }

    public void QLecternParadigmRefine()
    {
        CLecternParadigm paradigm = _qLecternSoundArea.CDisplayParadigmRead();
        QFontFace.QFontRefine(paradigm.CLecternParadigmFont, _qLecternSoundParadigm);
        _qLecternSoundInflection(paradigm.CLecternParadigmSlots);
    }

    public void QLecternSilenceRefine()
    {
        _qLecternSoundTranscription.Clear();
        _qLecternSoundGlyph.Clear();
        _qLecternSoundHeading.Text = string.Empty;
        _qLecternSoundSection.Visibility = Visibility.Collapsed;
        _qLecternSoundGutter.SharedSizeGroup = null;
        _qLecternSoundReflex.Clear();
        _qLecternSoundFold.Visibility = Visibility.Collapsed;
        _qLecternSoundLoading.Visibility = Visibility.Collapsed;
        _qLecternSoundInflection([]);
        _qLecternSoundWriting([], false);
        _qLecternSoundRime([], false);
        _qLecternSoundReading.Text = string.Empty;
    }

    public void QLecternFoldObserve()
    {
        _qLecternSoundArea.CDisplayReflexToggle(QLook.QLookCheckedRead(_qLecternSoundFold.IsChecked));
    }

    public void QLecternDiweiObserve(bool initial, string key)
    {
        _qLecternSoundArea.CDisplayDiweiOpen(initial, key);
    }

    public void QLecternStemObserve(string? key)
    {
        _qLecternSoundArea.CDisplayStemOpen(key);
    }

    public void QLecternGlyphObserve(object parameter)
    {
        if (parameter is QGlyphItem { QGlyphItemLinked: true } item)
        {
            _qLecternSoundArea.CDisplayGlyphOpen(item.QGlyphItemText, item.QGlyphItemLanguage);
        }
    }

    private void QLecternReflexRefine(CLecternReflex reflex)
    {
        _qLecternSoundReflex.Clear();
        foreach (CReflex row in reflex.CLecternReflexRows)
        {
            _qLecternSoundReflex.Add(new QReflexItem(row));
        }

        QReflexItem.QReflexAnchorRefine(
            _qLecternSoundReflex,
            reflex.CLecternReflexAnchor.CLecternAnchorOffered,
            reflex.CLecternReflexAnchor.CLecternAnchorTexts);
        _qLecternSoundLoading.Visibility = QLook.QLookVisibleRead(reflex.CLecternReflexPending);
    }
}
