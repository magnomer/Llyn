using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay : UserControl
{
    private PWindow _pDisplayHost = null!;

    private LLectern _lLectern = null!;

    public PDisplay()
    {
        InitializeComponent();
        Resources.MergedDictionaries.Add(new PDisplayCompass(this));

        PDisplaySwath.PSwathAttach(PDisplayContents);
        AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(PDisplayMentionHandle));

        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, PDisplayEtymonHandle));
    }

    internal void PDisplayAttach(PWindow host, LLectern lectern)
    {
        _pDisplayHost = host;
        _lLectern = lectern;
        lectern.LLecternAttach(host.PWindowDeportment, PDisplayEmpty, PDisplayContents, PDisplaySwath.PSwathClear);
        lectern.LLecternHeaderAttach(
            PDisplayHeadword,
            PDisplayLanguage,
            PDisplayLanguageFlag,
            PDisplayLanguageGlobe,
            PDisplayFavorite,
            PDisplayGrasp,
            PGrasp.PGraspStepProperty,
            PGrasp.PGraspLimitProperty,
            PDisplayGraspLabel);
        lectern.LLecternStampAttach(PDisplayStampSection, PDisplayStampAdded, PDisplayStampUpdated);
        lectern.LLecternFrequencyAttach(
            PDisplayFrequencySection, PDisplayFrequencyChip, PDisplayFrequency, PDisplayFrequencyBand);
        lectern.LLecternSpeechAttach(PDisplaySpeechSection, PDisplaySpeech);
        lectern.LLecternNoteAttach(PDisplayNoteSection, PDisplayNote);
        PMedia.PMediaAttach(this, host.PWindowDeportment);
        lectern.LLecternPlayback.LLecternPlaybackAttach(
            host.PWindowDeportment,
            PPlaybackAction,
            PPlayback,
            PVolume,
            PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogSet);
        lectern.LLecternAccent.LLecternAccentAttach(
            host.PWindowDeportment,
            PDisplayPronunciationSurface,
            PDisplayPronunciationLead,
            PDisplayPronunciationFlag,
            PDisplayPronunciationLabel,
            PDisplayPronunciationOpener,
            PDisplayPronunciation,
            PDisplayPronunciationCloser,
            PDisplayAccent,
            PDisplayContour,
            PContour.PContourTonalProperty);
        lectern.LLecternSound.LLecternGlyphAttach(
            host.PWindowDeportment,
            PDisplayTranscription,
            PDisplayGlyphSection,
            PDisplayGlyphLead,
            PDisplayGlyphLabel,
            PDisplayGlyph,
            host.PWindowGlyphShow);
        lectern.LLecternSound.LLecternReflexAttach(PDisplayReflex, PDisplayReflexLoading, PDisplayReflexFold);
        lectern.LLecternSound.LLecternFanqieAttach(
            PDisplayFanqie,
            PDisplayReading,
            PDisplayFanqie.PFanqieShow,
            host.PWindowDiweiShow,
            host.PWindowStemShow);
        lectern.LLecternSound.LLecternScriptAttach(PDisplayScript, PDisplayScript.PScriptShow);
        lectern.LLecternSound.LLecternParadigmAttach(PDisplayParadigm, PDisplayParadigm.PParadigmShow);
        lectern.LLecternCompassAttach(
            this, PDisplayContents, PDisplayHeader, PCompass, PCompassSurface, PCompassSwitch, PCompassList);
        lectern.LLecternCompass.LCompassSectionAttach(
            PDisplaySpeechSection,
            PDisplayFrequencySection,
            PDisplayMeaningSection,
            PDisplayMeaning,
            PDisplayCollocationSection,
            PDisplayCollocation,
            PDisplayIncomingSection,
            PDisplayNoteSection);
        lectern.LLecternCard.LLecternCardAttach(
            host.PWindowDeportment,
            Resources,
            PDisplayMeaning,
            PDisplayMeaningSection,
            PDisplayCollocation,
            PDisplayCollocationSection,
            PDisplayContents,
            lectern.LLecternCompass);
        lectern.LLecternCard.LLecternLinkAttach(
            ((PLinkConverter)Resources["Display.Card.Translation"]).PLinkConverterShow,
            ((PCitationConverter)Resources["Display.Card.Citation"]).PCitationConverterShow,
            PDisplayFrameRead().PSentenceConverterApply);
        lectern.LLecternCard.LLecternIncomingAttach(PDisplayIncoming, PDisplayIncomingSection);
        lectern.LLecternCard.LLecternEtymologyAttach(
            PDisplayEtymology, PDisplayEtymologySection, PDisplayEtymology.PEtymologyShow);
        lectern.LLecternCard.LLecternRouteAttach(
            host.PWindowEntryShow,
            host.PWindowSituationShow,
            host.PWindowRegisterShow,
            host.PWindowTagShow,
            host.PWindowFailureShow);
        PDisplayFanqie.PFanqieNoticeAttach(
            lectern.LLecternSound.LLecternDiweiShow,
            lectern.LLecternSound.LLecternStemShow,
            lectern.LLecternSound.LLecternFanqieSet);
    }

    internal void PCompassRowHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternCompass.LCompassRowHandle(sender);
    }

    private void PReflexFoldHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternSound.LLecternFoldHandle(PLook.PLookCheckedRead(PDisplayReflexFold.IsChecked));
    }

    internal void PDisplayObserverAttach()
    {
        _lLectern.LLecternObserverAttach(this);
    }

    private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternFavoriteHandle();
    }

    private void PDisplayGraspHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternGraspHandle(PDisplayGrasp.PGraspStep);
    }

    private void PDisplayHoverHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternHoverHandle(PDisplayGrasp.PGraspPointed);
    }

    private void PPlaybackActionHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternPlayback.LLecternActionHandle();
    }

    internal void PDisplayPlaybackHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _lLectern.LLecternPlayback.LLecternPlaybackHandle(e.Parameter);
    }

    private void PDisplayGlyphHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _lLectern.LLecternSound.LLecternGlyphHandle(e.Parameter);
    }

    internal void PDisplayClose()
    {
        _lLectern.LLecternClose();
    }

    private PSentenceConverter PDisplayFrameRead()
    {
        return (PSentenceConverter)Resources["Display.Card.Frame"];
    }

    private void PDisplayCardHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternCard.LLecternCardHandle(e);
    }

    private void PDisplayIncomingHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternCard.LLecternIncomingHandle(sender);
    }

    private void PDisplayEtymonHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _lLectern.LLecternCard.LLecternEtymonHandle(e.Parameter);
    }

    private void PDisplayMentionHandle(object? sender, PMentionArgument e)
    {
        _pDisplayHost.PWindowMentionHandle(
            e.PMentionArgumentOrigin,
            _lLectern.LLecternCard.LLecternMentionFind(
                e.PMentionArgumentText,
                e.PMentionArgumentLanguage,
                e.PMentionArgumentOffset,
                e.PMentionArgumentMention));
    }

    internal void PDisplayCardScroll(long id)
    {
        _lLectern.LLecternCard.LLecternCardScroll(id);
    }
}
