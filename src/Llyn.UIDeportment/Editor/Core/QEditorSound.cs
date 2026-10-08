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

    private CLedger _qEditorSoundLedger = null!;

    private CEnvoy _qEditorSoundEnvoy = null!;

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

    internal void QEditorSoundIntroduce(CEditor editor, QVolume volume, CLedger ledger, CEnvoy envoy)
    {
        _cEditor = editor;
        CDesk desk = editor.CEditorDesk;
        CErrand errand = desk.CDeskErrand;
        CEntry entry = editor.CEditorEntry;
        CTimbre timbre = editor.CEditorTimbre;
        CTranscription transcription = editor.CEditorTranscription;
        _qCadence.QCadenceIntroduce(desk, timbre, editor.CEditorSounding, editor.CEditorFold, ledger, envoy);
        _qNotation.QNotationIntroduce(errand);
        _qTranscription.QTranscriptionIntroduce(errand, entry, transcription);
        _qGlyph.QGlyphIntroduce(errand, entry, timbre, transcription);
        _qAccent.QAccentIntroduce(errand, entry, editor.CEditorPlayback, timbre);
        _qReflex.QReflexIntroduce(
            desk, editor.CEditorDisplay.CDisplaySound, entry, editor.CEditorKindred, editor.CEditorSounding);
        _qAnchor.QAnchorIntroduce(CSoundingAnchor.CSoundingAnchorCreate(editor.CEditorKindred));
        _qClip.QClipIntroduce(errand);
        _qPlayback.QPlaybackIntroduce(entry, editor.CEditorPlayback);
        volume.QVolumeSliderAttach(_qEditorSoundSurface);
        volume.QVolumePlayerAttach(_qEditorSoundPlayer);
        _qEditorSoundLedger = ledger;
        _qEditorSoundEnvoy = envoy;
        _qEditorSoundPlayer.MediaFailed += QEditorFailureObserve;
    }

    private void QEditorFailureObserve(object? sender, ExceptionEventArgs e)
    {
        _qEditorSoundLedger.CLedgerFailureShow(_qEditorSoundEnvoy, "Sound.PlayFailed", e.ErrorException);
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
