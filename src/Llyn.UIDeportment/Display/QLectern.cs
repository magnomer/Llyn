using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLectern
{
    private readonly TextBlock _qLecternEmpty;

    private readonly ScrollViewer _qLecternContents;

    public QLectern(FrameworkElement surface, CDisplay display, CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _qLecternEmpty = QContract.QContractFind<TextBlock>(surface, "PDisplayEmpty");
        _qLecternContents = QContract.QContractFind<ScrollViewer>(surface, "PDisplayContents");

        display.CDisplayOpened += QLecternContentsRefine;
        display.CDisplayClosed += QLecternEmptyRefine;
        _ = new QLecternHeader(surface, display, atelier, envoy);
        QLecternGrasp grasp = new(surface, display.CDisplayGrasp);
        display.CDisplayOpened += grasp.QLecternGraspRefine;
        display.CDisplayClosed += grasp.QLecternGraspRefine;
        _ = new QLecternFrequency(surface, display);
        _ = new QLecternEntry(surface, display, atelier);
        QLecternPlayback playback = new(surface, display.CDisplayPlayback);
        display.CDisplayOpened += playback.QLecternPlaybackRefine;
        display.CDisplayClosed += playback.QLecternPlaybackRefine;
        QLecternAccent accent = new(surface, display.CDisplayAccent);
        display.CDisplayOpened += accent.QLecternAccentRefine;
        display.CDisplayClosed += accent.QLecternAccentRefine;
        display.CDisplayOpened += accent.QLecternEnsignRefine;
        QLecternGlyph glyph = new(surface, display.CDisplaySound);
        display.CDisplayOpened += glyph.QLecternGlyphRefine;
        display.CDisplayClosed += glyph.QLecternGlyphRefine;
        QLecternTranscription transcription = new(surface, display.CDisplaySound);
        display.CDisplayOpened += transcription.QLecternTranscriptionRefine;
        display.CDisplayClosed += transcription.QLecternTranscriptionRefine;
        QLecternReflex reflex = new(surface, display.CDisplaySound);
        display.CDisplayOpened += reflex.QLecternReflexRefine;
        display.CDisplayOpened += reflex.QLecternFoldRefine;
        display.CDisplayClosed += reflex.QLecternReflexRefine;
        display.CDisplayClosed += reflex.QLecternFoldRefine;
        display.CDisplayReflexChanged += QObserver.QObserverCreate<CBulletin>(surface, reflex.QLecternRenewalRefine);
        display.CDisplayReflexChanged += QObserver.QObserverCreate<CBulletin>(surface, reflex.QLecternFoldRefine);
        display.CDisplayFoldChanged += QObserver.QObserverCreate<CBulletin>(surface, reflex.QLecternFoldRefine);
        display.CDisplayFanqieChanged += QObserver.QObserverCreate<CBulletin>(surface, reflex.QLecternAnchorRefine);
        QLecternSound sound = new(
            surface, display.CDisplaySound, display.CDisplayFold, atelier.CAtelierLedger, envoy);
        display.CDisplayOpened += sound.QLecternParadigmRefine;
        display.CDisplayOpened += sound.QLecternScriptRefine;
        display.CDisplayOpened += sound.QLecternFanqieRefine;
        display.CDisplayClosed += sound.QLecternParadigmRefine;
        display.CDisplayClosed += sound.QLecternScriptRefine;
        display.CDisplayClosed += sound.QLecternFanqieRefine;
        display.CDisplayOpened += sound.QLecternBoxRefine;
        display.CDisplayClosed += sound.QLecternBoxRefine;
        display.CDisplayFoldChanged += QObserver.QObserverCreate<CBulletin>(surface, sound.QLecternBoxRefine);
        display.CDisplayParadigmChanged += QObserver.QObserverCreate<CBulletin>(surface, sound.QLecternParadigmRefine);
        display.CDisplayScriptChanged += QObserver.QObserverCreate<CBulletin>(surface, sound.QLecternScriptRefine);
        display.CDisplayFanqieChanged += QObserver.QObserverCreate<CBulletin>(surface, sound.QLecternFanqieRefine);
        QLecternCard = new QLecternCard(surface, display.CDisplayCard, display.CDisplayRoute, display.CDisplaySound);
        display.CDisplayOpened += QLecternCard.QLecternExampleRefine;
        display.CDisplayOpened += QLecternCard.QLecternGlossRefine;
        display.CDisplayOpened += QLecternCard.QLecternCardRefine;
        display.CDisplayClosed += QLecternCard.QLecternCardRefine;
        display.CDisplayFoldChanged += QObserver.QObserverCreate<CBulletin>(surface, QLecternCard.QLecternCardRefine);
        QLecternIncoming incoming = new(surface, display.CDisplayCard, atelier.CAtelierNavigation);
        display.CDisplayOpened += incoming.QLecternIncomingRefine;
        display.CDisplayClosed += incoming.QLecternIncomingRefine;
        QLecternEtymology = new QLecternEtymology(surface, display.CDisplayCard, display.CDisplayRoute);
        display.CDisplayOpened += QLecternEtymology.QLecternEtymologyRefine;
        display.CDisplayClosed += QLecternEtymology.QLecternEtymologyRefine;

        QLecternCompass = new QCompass(display.CDisplayCompass, surface);
        display.CDisplayOpened += QLecternCompass.QCompassRefine;
        display.CDisplayClosed += QLecternCompass.QCompassEmptyRefine;
        _ = new QRoster(surface, QLecternCompass);

        display.CDisplayEntryChanged +=
            QObserver.QObserverCreate<CBulletin>(surface, display.CDisplayEntryResonate);
        display.CDisplayWorkspaceChanged +=
            QObserver.QObserverCreate<CBulletin>(surface, display.CDisplayWorkspaceResonate);
    }

    public QLecternCard QLecternCard { get; }

    public QLecternEtymology QLecternEtymology { get; }

    public QCompass QLecternCompass { get; }

    private void QLecternContentsRefine()
    {
        _qLecternEmpty.Visibility = Visibility.Collapsed;
        _qLecternContents.Visibility = Visibility.Visible;
    }

    private void QLecternEmptyRefine()
    {
        _qLecternContents.Visibility = Visibility.Collapsed;
        _qLecternEmpty.Visibility = Visibility.Visible;
    }
}
