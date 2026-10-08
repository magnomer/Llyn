using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEditorSound
{
    private readonly FrameworkElement _qEditorSoundSurface;

    private readonly MediaPlayer _qEditorSoundPlayer = new();

    private readonly QAnchor _qAnchor;

    private readonly QCadence _qCadence;

    private readonly QNotation _qNotation;

    private readonly QTranscription _qTranscription;

    private readonly QGlyph _qGlyph;

    private readonly QClip _qClip;

    private readonly QAccent _qAccent;

    private readonly QReflex _qReflex;

    private readonly QPlayback _qPlayback;

    private CEditor _cEditor = null!;

    internal QEditorSound(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qEditorSoundSurface = surface;
        _qAnchor = new QAnchor(surface);
        QEditorSoundField.TextChanged += QEditorContourRefine;
        QEditorSoundField.TextChanged += QEditorPronunciationObserve;
        QField.QFieldGhostAttach(QEditorSoundMeasure, QEditorSoundField);
        _qCadence = new QCadence(surface);
        _qNotation = new QNotation(surface);
        _qTranscription = new QTranscription(surface, _qNotation);
        _qGlyph = new QGlyph(surface, _qNotation);
        _qClip = new QClip(surface, _qEditorSoundPlayer);
        _qAccent = new QAccent(surface, _qNotation, _qClip, _qEditorSoundPlayer);
        _qReflex = new QReflex(surface, _qAnchor);
        _qPlayback = new QPlayback(surface, _qEditorSoundPlayer);
        QEditorSoundField.SetResourceReference(QField.QFieldHintProperty, "Input.Pronunciation");
    }

    private TextBox QEditorSoundHeadword => QContract.QContractFind<TextBox>(_qEditorSoundSurface, "PHeadword");

    private Border QEditorSoundRow => QContract.QContractFind<Border>(_qEditorSoundSurface, "PPronunciation");

    private TextBlock QEditorSoundOpener =>
        QContract.QContractFind<TextBlock>(_qEditorSoundSurface, "PPronunciationOpener");

    private TextBlock QEditorSoundMeasure =>
        QContract.QContractFind<TextBlock>(_qEditorSoundSurface, "PPronunciationMeasure");

    private TextBox QEditorSoundField => QContract.QContractFind<TextBox>(_qEditorSoundSurface, "PPronunciationField");

    private TextBlock QEditorSoundCloser =>
        QContract.QContractFind<TextBlock>(_qEditorSoundSurface, "PPronunciationCloser");

    private PContour QEditorSoundContour => QContract.QContractFind<PContour>(_qEditorSoundSurface, "PContour");

    internal void QEditorSoundIntroduce(CEditor editor, QVolume volume)
    {
        _cEditor = editor;
        _qCadence.QCadenceIntroduce(editor);
        _qNotation.QNotationIntroduce(editor);
        _qTranscription.QTranscriptionIntroduce(editor);
        _qGlyph.QGlyphIntroduce(editor);
        _qAccent.QAccentIntroduce(editor);
        _qReflex.QReflexIntroduce(editor);
        _qAnchor.QAnchorIntroduce(CSoundingAnchor.CSoundingAnchorCreate(editor));
        _qClip.QClipIntroduce(editor);
        _qPlayback.QPlaybackIntroduce(editor);
        volume.QVolumeSliderAttach(_qEditorSoundSurface);
        volume.QVolumePlayerAttach(_qEditorSoundPlayer);
        _qEditorSoundPlayer.MediaFailed += QEditorFailureRefine;
    }

    private void QEditorFailureRefine(object? sender, ExceptionEventArgs e)
    {
        if (Window.GetWindow(_qEditorSoundSurface)?.Tag is QWindow host)
        {
            host.QWindowFailureRefine("Sound.PlayFailed", e.ErrorException);
        }
    }

    internal void QEditorPlayerRefine()
    {
        _qEditorSoundPlayer.Close();
    }

    internal void QEditorReadingRefine()
    {
        _qCadence.QCadenceReadingRefine(QEditorSoundHeadword.Text);
    }

    internal void QEditorPronunciationRefine(CEntryDraft _)
    {
        QField.QFieldTextShow(QEditorSoundField, _cEditor.CEditorEntry.CEntryPronunciationRead());
    }

    internal void QEditorTimbreRefine(CEntryDraft _)
    {
        CTimbre timbre = _cEditor.CEditorTimbre;
        QEditorSoundOpener.Text = QLook.QLookFirstRead(timbre.CTimbrePhonemic, "/", "[");
        QEditorSoundCloser.Text = QLook.QLookFirstRead(timbre.CTimbrePhonemic, "/", "]");
        QEditorContourRefine();
        QEditorSoundRow.Visibility = QLook.QLookVisibleRead(timbre.CTimbreSpoken);
    }

    private void QEditorContourRefine(object sender, TextChangedEventArgs e)
    {
        QEditorContourRefine();
    }

    private void QEditorContourRefine()
    {
        QEditorContourRefine(_cEditor.CEditorTimbre.CTimbreContourRead(QEditorSoundField.Text));
    }

    private void QEditorContourRefine(IReadOnlyList<CContour> syllables)
    {
        PContour contour = QEditorSoundContour;
        contour.PContourScale = _cEditor.CEditorDisplay.CDisplayAccent.CDisplayAccentScale;
        contour.PContourSyllables = QContourInk.QContourInkBuild(syllables, contour);
    }

    private void QEditorPronunciationObserve(object sender, TextChangedEventArgs e)
    {
        _cEditor.CEditorEntry.CEntryPronunciationSet(QEditorSoundField.Text);
    }
}
