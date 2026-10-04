using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal sealed class QSounding
{
    private readonly FrameworkElement _qSoundingSurface;
    private QLectern _qSoundingLectern = null!;

    internal QSounding(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qSoundingSurface = surface;
    }

    private ItemsControl QSoundingReflex => QContract.QContractFind<ItemsControl>(_qSoundingSurface, "PDisplayReflex");

    private TextBlock QSoundingReflexLoading =>
        QContract.QContractFind<TextBlock>(_qSoundingSurface, "PDisplayReflexLoading");

    private ToggleButton QSoundingReflexFold =>
        QContract.QContractFind<ToggleButton>(_qSoundingSurface, "PDisplayReflexFold");

    private Border QSoundingPronunciationSurface =>
        QContract.QContractFind<Border>(_qSoundingSurface, "PDisplayPronunciationSurface");

    private ColumnDefinition QSoundingPronunciationLead =>
        QContract.QContractFind<ColumnDefinition>(_qSoundingSurface, "PDisplayPronunciationLead");

    private Image QSoundingPronunciationFlag =>
        QContract.QContractFind<Image>(_qSoundingSurface, "PDisplayPronunciationFlag");

    private TextBlock QSoundingPronunciationLabel =>
        QContract.QContractFind<TextBlock>(_qSoundingSurface, "PDisplayPronunciationLabel");

    private TextBlock QSoundingPronunciationOpener =>
        QContract.QContractFind<TextBlock>(_qSoundingSurface, "PDisplayPronunciationOpener");

    private TextBlock QSoundingPronunciation =>
        QContract.QContractFind<TextBlock>(_qSoundingSurface, "PDisplayPronunciation");

    private TextBlock QSoundingPronunciationCloser =>
        QContract.QContractFind<TextBlock>(_qSoundingSurface, "PDisplayPronunciationCloser");

    private Button QSoundingPlaybackAction => QContract.QContractFind<Button>(_qSoundingSurface, "PPlaybackAction");

    private Border QSoundingPlayback => QContract.QContractFind<Border>(_qSoundingSurface, "PPlayback");

    private Slider QSoundingVolume => QContract.QContractFind<Slider>(_qSoundingSurface, "PVolume");

    private PContour QSoundingContour => QContract.QContractFind<PContour>(_qSoundingSurface, "PDisplayContour");

    private ItemsControl QSoundingAccent => QContract.QContractFind<ItemsControl>(_qSoundingSurface, "PDisplayAccent");

    private ItemsControl QSoundingTranscription =>
        QContract.QContractFind<ItemsControl>(_qSoundingSurface, "PDisplayTranscription");

    private Border QSoundingGlyphSection =>
        QContract.QContractFind<Border>(_qSoundingSurface, "PDisplayGlyphSection");

    private ColumnDefinition QSoundingGlyphLead =>
        QContract.QContractFind<ColumnDefinition>(_qSoundingSurface, "PDisplayGlyphLead");

    private TextBlock QSoundingGlyphLabel =>
        QContract.QContractFind<TextBlock>(_qSoundingSurface, "PDisplayGlyphLabel");

    private ItemsControl QSoundingGlyph => QContract.QContractFind<ItemsControl>(_qSoundingSurface, "PDisplayGlyph");

    private QParadigm QSoundingParadigm =>
        QContract.QContractFind<QParadigm>(_qSoundingSurface, "PDisplayParadigm");

    private QScript QSoundingScript => QContract.QContractFind<QScript>(_qSoundingSurface, "PDisplayScript");

    internal void QSoundingIntroduce(QWindow host, QLectern lectern)
    {
        _qSoundingLectern = lectern;

        QSoundingAccent.CommandBindings.Add(new CommandBinding(
            QAccentCommand.QAccentCommandPlayback, QSoundingPlaybackObserve));
        QSoundingGlyph.CommandBindings.Add(new CommandBinding(
            QGlyphCommand.QGlyphCommandEntry, QSoundingGlyphObserve));

        QSoundingReflexFold.Checked += QSoundingFoldObserve;
        QSoundingReflexFold.Unchecked += QSoundingFoldObserve;
        QSoundingPlaybackAction.Click += QSoundingActionObserve;
        QSoundingPlaybackAction.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));

        host.QWindowVolume.QVolumeSliderAttach(_qSoundingSurface);

        lectern.QLecternPlayback.QLecternPlaybackIntroduce(
            QSoundingPlaybackAction, QSoundingPlayback, QSoundingVolume);
        lectern.QLecternAccent.QLecternAccentIntroduce(
            QSoundingPronunciationSurface,
            QSoundingPronunciationLead,
            QSoundingPronunciationFlag,
            QSoundingPronunciationLabel,
            QSoundingPronunciationOpener,
            QSoundingPronunciation,
            QSoundingPronunciationCloser,
            QSoundingAccent,
            QSoundingContour,
            PContour.PContourSyllablesProperty,
            PContour.PContourScaleProperty);
        lectern.QLecternSound.QLecternGlyphIntroduce(
            QSoundingTranscription,
            QSoundingGlyphSection,
            QSoundingGlyphLead,
            QSoundingGlyphLabel,
            QSoundingGlyph);
        lectern.QLecternSound.QLecternReflexIntroduce(QSoundingReflex, QSoundingReflexLoading, QSoundingReflexFold);
        lectern.QLecternSound.QLecternScriptIntroduce(QSoundingScript, QSoundingScript.QScriptRefine);
        lectern.QLecternSound.QLecternParadigmIntroduce(QSoundingParadigm, QSoundingParadigm.QParadigmRefine);
    }

    private void QSoundingFoldObserve(object sender, RoutedEventArgs e)
    {
        _qSoundingLectern.QLecternSound.QLecternFoldObserve();
    }

    private void QSoundingActionObserve(object sender, RoutedEventArgs e)
    {
        _qSoundingLectern.QLecternPlayback.QLecternActionObserve();
    }

    private void QSoundingPlaybackObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qSoundingLectern.QLecternPlayback.QLecternPlaybackObserve(e.Parameter);
    }

    private void QSoundingGlyphObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qSoundingLectern.QLecternSound.QLecternGlyphObserve(e.Parameter);
    }
}
