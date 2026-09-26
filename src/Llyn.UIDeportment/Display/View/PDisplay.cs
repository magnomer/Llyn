using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

public class PDisplay : UserControl
{
    private readonly PDisplayCompass _pDisplayCompass;

    private readonly PLeaf _pLeaf = new();

    private PWindow _pDisplayHost = null!;

    private LLectern _lLectern = null!;

    public PDisplay()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Display/View/PDisplay.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
        _pDisplayCompass = new PDisplayCompass(this);
        Resources.MergedDictionaries.Add(_pDisplayCompass);
        PLook.PLookStyleAttach(surface.Resources);
        PLook.PLookStyleAttach(_pDisplayCompass);

        PDisplaySwath.PSwathAttach(PDisplayContents);
        AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(PDisplayMentionHandle));

        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, PDisplayEtymonHandle));
        PDisplayAccent.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandPlayback, PDisplayPlaybackHandle));
        PDisplayGlyph.CommandBindings.Add(new CommandBinding(
            PGlyphCommand.PGlyphCommandEntry, PDisplayGlyphHandle));

        PDisplayFavorite.Click += PDisplayFavoriteHandle;
        PDisplayGrasp.PGraspChanged += PDisplayGraspHandle;
        PDisplayGrasp.PGraspHovered += PDisplayHoverHandle;
        PDisplayReflexFold.Checked += PReflexFoldHandle;
        PDisplayReflexFold.Unchecked += PReflexFoldHandle;
        PPlaybackAction.Click += PPlaybackActionHandle;
        PPlaybackAction.Tag = PIcon.PIconResolve("play", 24);
        PCompassIcon.PIconSource = PIcon.PIconResolve("compass", 24);
        PDisplayMeaning.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PDisplayCardHandle));
        PDisplayCollocation.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PDisplayCardHandle));
        PDisplayIncoming.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PDisplayIncomingHandle));

        PVolume.SetBinding(
            RangeBase.ValueProperty,
            new Binding(nameof(PVolumeCatalog.PVolumeCatalogLevel))
            {
                Source = PVolumeCatalog.PVolumeCatalogCurrent,
                Mode = BindingMode.TwoWay,
            });
        PDisplayContour.SetBinding(
            PContour.PContourIpaProperty,
            new Binding(nameof(TextBlock.Text)) { Source = PDisplayPronunciation });

        PLookItem.PLookItemAttach(PDisplaySpeech, PSpeechApply);
        PLookItem.PLookItemAttach(PDisplayMeaning, _pLeaf.PLeafCardApply);
        PLookItem.PLookItemAttach(PDisplayCollocation, _pLeaf.PLeafCardApply);
        PLookItem.PLookItemAttach(PDisplayIncoming, PUsageApply);
        PLookItem.PLookItemAttach(PCompassList, PCompassApply);
    }

    private TextBlock PDisplayEmpty => (TextBlock)FindName(nameof(PDisplayEmpty));

    private ScrollViewer PDisplayContents => (ScrollViewer)FindName(nameof(PDisplayContents));

    private Grid PDisplayHeader => (Grid)FindName(nameof(PDisplayHeader));

    private TextBlock PDisplayHeadword => (TextBlock)FindName(nameof(PDisplayHeadword));

    private TextBlock PDisplayReading => (TextBlock)FindName(nameof(PDisplayReading));

    private Image PDisplayLanguageFlag => (Image)FindName(nameof(PDisplayLanguageFlag));

    private Ellipse PDisplayLanguageGlobe => (Ellipse)FindName(nameof(PDisplayLanguageGlobe));

    private TextBlock PDisplayLanguage => (TextBlock)FindName(nameof(PDisplayLanguage));

    private ToggleButton PDisplayFavorite => (ToggleButton)FindName(nameof(PDisplayFavorite));

    private PGrasp PDisplayGrasp => (PGrasp)FindName(nameof(PDisplayGrasp));

    private TextBlock PDisplayGraspLabel => (TextBlock)FindName(nameof(PDisplayGraspLabel));

    private PReflexList PDisplayReflex => (PReflexList)FindName(nameof(PDisplayReflex));

    private TextBlock PDisplayReflexLoading => (TextBlock)FindName(nameof(PDisplayReflexLoading));

    private ToggleButton PDisplayReflexFold => (ToggleButton)FindName(nameof(PDisplayReflexFold));

    private Border PDisplayPronunciationSurface => (Border)FindName(nameof(PDisplayPronunciationSurface));

    private ColumnDefinition PDisplayPronunciationLead => (ColumnDefinition)FindName(nameof(PDisplayPronunciationLead));

    private Image PDisplayPronunciationFlag => (Image)FindName(nameof(PDisplayPronunciationFlag));

    private TextBlock PDisplayPronunciationLabel => (TextBlock)FindName(nameof(PDisplayPronunciationLabel));

    private TextBlock PDisplayPronunciationOpener => (TextBlock)FindName(nameof(PDisplayPronunciationOpener));

    private TextBlock PDisplayPronunciation => (TextBlock)FindName(nameof(PDisplayPronunciation));

    private TextBlock PDisplayPronunciationCloser => (TextBlock)FindName(nameof(PDisplayPronunciationCloser));

    private Button PPlaybackAction => (Button)FindName(nameof(PPlaybackAction));

    private Border PPlayback => (Border)FindName(nameof(PPlayback));

    private Slider PVolume => (Slider)FindName(nameof(PVolume));

    private PContour PDisplayContour => (PContour)FindName(nameof(PDisplayContour));

    private ItemsControl PDisplayAccent => (ItemsControl)FindName(nameof(PDisplayAccent));

    private ItemsControl PDisplayTranscription => (ItemsControl)FindName(nameof(PDisplayTranscription));

    private Border PDisplayGlyphSection => (Border)FindName(nameof(PDisplayGlyphSection));

    private ColumnDefinition PDisplayGlyphLead => (ColumnDefinition)FindName(nameof(PDisplayGlyphLead));

    private TextBlock PDisplayGlyphLabel => (TextBlock)FindName(nameof(PDisplayGlyphLabel));

    private ItemsControl PDisplayGlyph => (ItemsControl)FindName(nameof(PDisplayGlyph));

    private StackPanel PDisplaySpeechSection => (StackPanel)FindName(nameof(PDisplaySpeechSection));

    private ItemsControl PDisplaySpeech => (ItemsControl)FindName(nameof(PDisplaySpeech));

    private StackPanel PDisplayFrequencySection => (StackPanel)FindName(nameof(PDisplayFrequencySection));

    private Border PDisplayFrequencyChip => (Border)FindName(nameof(PDisplayFrequencyChip));

    private TextBlock PDisplayFrequency => (TextBlock)FindName(nameof(PDisplayFrequency));

    private TextBlock PDisplayFrequencyBand => (TextBlock)FindName(nameof(PDisplayFrequencyBand));

    private PParadigm PDisplayParadigm => (PParadigm)FindName(nameof(PDisplayParadigm));

    private PFanqie PDisplayFanqie => (PFanqie)FindName(nameof(PDisplayFanqie));

    private StackPanel PDisplayMeaningSection => (StackPanel)FindName(nameof(PDisplayMeaningSection));

    private ItemsControl PDisplayMeaning => (ItemsControl)FindName(nameof(PDisplayMeaning));

    private StackPanel PDisplayCollocationSection => (StackPanel)FindName(nameof(PDisplayCollocationSection));

    private ItemsControl PDisplayCollocation => (ItemsControl)FindName(nameof(PDisplayCollocation));

    private StackPanel PDisplayIncomingSection => (StackPanel)FindName(nameof(PDisplayIncomingSection));

    private ItemsControl PDisplayIncoming => (ItemsControl)FindName(nameof(PDisplayIncoming));

    private StackPanel PDisplayEtymologySection => (StackPanel)FindName(nameof(PDisplayEtymologySection));

    private PEtymology PDisplayEtymology => (PEtymology)FindName(nameof(PDisplayEtymology));

    private StackPanel PDisplayNoteSection => (StackPanel)FindName(nameof(PDisplayNoteSection));

    private StackPanel PDisplayNote => (StackPanel)FindName(nameof(PDisplayNote));

    private PScript PDisplayScript => (PScript)FindName(nameof(PDisplayScript));

    private StackPanel PDisplayStampSection => (StackPanel)FindName(nameof(PDisplayStampSection));

    private TextBlock PDisplayStampAdded => (TextBlock)FindName(nameof(PDisplayStampAdded));

    private TextBlock PDisplayStampUpdated => (TextBlock)FindName(nameof(PDisplayStampUpdated));

    private PSwath PDisplaySwath => (PSwath)FindName(nameof(PDisplaySwath));

    private StackPanel PCompass => (StackPanel)FindName(nameof(PCompass));

    private Border PCompassSurface => (Border)FindName(nameof(PCompassSurface));

    private ToggleButton PCompassSwitch => (ToggleButton)FindName(nameof(PCompassSwitch));

    private ItemsControl PCompassList => (ItemsControl)FindName(nameof(PCompassList));

    private PIconImage PCompassIcon => (PIconImage)FindName(nameof(PCompassIcon));

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
        lectern.LLecternPlayback.LLecternPlaybackAttach(host.PWindowDeportment, PPlaybackAction, PPlayback, PVolume);
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
            _pLeaf.PLeafLink.PLinkConverterShow,
            _pLeaf.PLeafCitation.PCitationConverterShow,
            _pLeaf.PLeafFrame.PSentenceConverterApply);
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

    private void PDisplayCardHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternCard.LLecternCardHandle(e);
    }

    private void PDisplayIncomingHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternCard.LLecternIncomingHandle(e.OriginalSource);
    }

    private void PDisplayEtymonHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _lLectern.LLecternCard.LLecternEtymonHandle(e.Parameter);
    }

    private void PDisplayMentionHandle(object? sender, PMentionArgument e)
    {
        _lLectern.LLecternCard.LLecternMentionFind(
            e.PMentionArgumentOrigin,
            e.PMentionArgumentText,
            e.PMentionArgumentLanguage,
            e.PMentionArgumentOffset,
            e.PMentionArgumentMention,
            _pDisplayHost.PWindowMentionHandle);
    }

    internal void PDisplayCardScroll(long id)
    {
        _lLectern.LLecternCard.LLecternCardScroll(id);
    }

    private static void PSpeechApply(FrameworkElement container, object item, string? _)
    {
        if (item is string speech && PLook.PLookPartFind<TextBlock>(container, "PSpeechName") is TextBlock name)
        {
            name.Text = speech;
        }
    }

    private static void PUsageApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LUsageItem usage)
        {
            return;
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PUsageIcon") is PIconImage icon)
        {
            icon.PIconSource = PIcon.PIconResolve("incoming", 24);
        }

        if (PLook.PLookPartFind<Run>(container, "PUsageName") is Run name)
        {
            name.Text = usage.LUsageItemName;
        }

        if (PLook.PLookPartFind<Run>(container, "PUsageEpithet") is Run epithet)
        {
            epithet.Text = "\u2002" + usage.LUsageItemEpithet;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PUsageTitle") is TextBlock title)
        {
            title.Text = usage.LUsageItemTitle;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PUsageOwner") is TextBlock owner)
        {
            owner.Text = usage.LUsageItemOwner;
        }

        if (PLook.PLookPartFind<Image>(container, "PUsageFlag") is Image flag)
        {
            flag.Source = usage.LUsageItemFlag;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PUsageLanguage") is TextBlock language)
        {
            language.Text = usage.LUsageItemLanguage;
        }
    }

    private void PCompassApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LCompassItem compass)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PCompassRow") is Button row)
        {
            if (compass.LCompassItemCurrent)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(TagProperty);
            }

            row.Click -= _pDisplayCompass.PCompassRowHandle;
            row.Click += _pDisplayCompass.PCompassRowHandle;
        }

        if (PLook.PLookPartFind<Grid>(container, "PCompassIndent") is Grid indent)
        {
            indent.Margin = compass.LCompassItemIndent;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PCompassNumber") is TextBlock number)
        {
            number.Text = compass.LCompassItemNumber;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PCompassName") is TextBlock name)
        {
            name.Text = compass.LCompassItemName;
        }
    }
}
