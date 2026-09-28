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

    private QLectern _qLectern = null!;

    public PDisplay()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Display/View/PDisplay.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
        _pDisplayCompass = new PDisplayCompass(this);
        Resources.MergedDictionaries.Add(_pDisplayCompass);
        QLook.QLookStyleAttach(surface.Resources);
        QLook.QLookStyleAttach(_pDisplayCompass);

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
        PPlaybackAction.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
        PCompassIcon.QIconSource = QIcon.QIconResolve("compass", 24);
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

        QLookItem.QLookItemAttach(PDisplaySpeech, PSpeechApply);
        QLookItem.QLookItemAttach(PDisplayMeaning, _pLeaf.PLeafCardApply);
        QLookItem.QLookItemAttach(PDisplayCollocation, _pLeaf.PLeafCardApply);
        QLookItem.QLookItemAttach(PDisplayIncoming, PUsageApply);
        QLookItem.QLookItemAttach(PCompassList, PCompassApply);
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

    private QIconImage PCompassIcon => (QIconImage)FindName(nameof(PCompassIcon));

    internal void PDisplayAttach(PWindow host, QLectern lectern)
    {
        _pDisplayHost = host;
        _qLectern = lectern;
        lectern.QLecternIntroduce(host.PWindowAtelier, PDisplayEmpty, PDisplayContents, PDisplaySwath.PSwathClear);
        lectern.QLecternHeaderIntroduce(
            PDisplayHeadword,
            PDisplayLanguage,
            PDisplayLanguageFlag,
            PDisplayLanguageGlobe,
            PDisplayFavorite,
            PDisplayGrasp,
            PGrasp.PGraspStepProperty,
            PGrasp.PGraspLimitProperty,
            PDisplayGraspLabel);
        lectern.QLecternStampIntroduce(PDisplayStampSection, PDisplayStampAdded, PDisplayStampUpdated);
        lectern.QLecternFrequencyIntroduce(
            PDisplayFrequencySection, PDisplayFrequencyChip, PDisplayFrequency, PDisplayFrequencyBand);
        lectern.QLecternSpeechIntroduce(PDisplaySpeechSection, PDisplaySpeech);
        lectern.QLecternNoteIntroduce(PDisplayNoteSection, PDisplayNote);
        PMedia.PMediaAttach(this, host.PWindowAtelier);
        lectern.QLecternPlayback.QLecternPlaybackIntroduce(host.PWindowAtelier, PPlaybackAction, PPlayback, PVolume);
        lectern.QLecternAccent.QLecternAccentIntroduce(
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
        lectern.QLecternSound.QLecternGlyphIntroduce(
            PDisplayTranscription,
            PDisplayGlyphSection,
            PDisplayGlyphLead,
            PDisplayGlyphLabel,
            PDisplayGlyph);
        lectern.QLecternSound.QLecternReflexIntroduce(PDisplayReflex, PDisplayReflexLoading, PDisplayReflexFold);
        lectern.QLecternSound.QLecternFanqieIntroduce(
            PDisplayFanqie,
            PDisplayReading,
            PDisplayFanqie.PFanqieShow);
        lectern.QLecternSound.QLecternRouteIntroduce(host);
        lectern.QLecternSound.QLecternScriptIntroduce(PDisplayScript, PDisplayScript.PScriptShow);
        lectern.QLecternSound.QLecternParadigmIntroduce(PDisplayParadigm, PDisplayParadigm.PParadigmShow);
        lectern.QLecternCompassIntroduce(
            this, PDisplayContents, PDisplayHeader, PCompass, PCompassSurface, PCompassSwitch, PCompassList);
        lectern.QLecternCompass.QCompassSectionIntroduce(
            PDisplaySpeechSection,
            PDisplayFrequencySection,
            PDisplayMeaningSection,
            PDisplayMeaning,
            PDisplayCollocationSection,
            PDisplayCollocation,
            PDisplayIncomingSection,
            PDisplayNoteSection);
        lectern.QLecternCard.QLecternCardIntroduce(
            host.PWindowAtelier,
            Resources,
            PDisplayMeaning,
            PDisplayMeaningSection,
            PDisplayCollocation,
            PDisplayCollocationSection,
            PDisplayContents,
            lectern.QLecternCompass);
        lectern.QLecternCard.QLecternLinkIntroduce(
            _pLeaf.PLeafLink.PLinkConverterShow,
            _pLeaf.PLeafCitation.QCitationConverterShow,
            _pLeaf.PLeafFrame.PSentenceConverterApply);
        lectern.QLecternCard.QLecternIncomingIntroduce(PDisplayIncoming, PDisplayIncomingSection);
        lectern.QLecternCard.QLecternEtymologyIntroduce(
            PDisplayEtymology, PDisplayEtymologySection, PDisplayEtymology.PEtymologyShow);
        lectern.QLecternCard.QLecternRouteIntroduce(host);
        PDisplayFanqie.PFanqieNoticeAttach(
            lectern.QLecternSound.QLecternDiweiObserve,
            lectern.QLecternSound.QLecternStemObserve,
            lectern.QLecternSoundArea.CDisplayFanqieSet);
    }

    internal void PCompassRowHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternCompass.QCompassRowRefine(sender);
    }

    private void PReflexFoldHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternSound.QLecternFoldObserve();
    }

    private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternFavoriteObserve();
    }

    private void PDisplayGraspHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternGraspObserve(PDisplayGrasp.PGraspStep);
    }

    private void PDisplayHoverHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternHoverRefine(PDisplayGrasp.PGraspPointed);
    }

    private void PPlaybackActionHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternPlayback.QLecternActionObserve();
    }

    internal void PDisplayPlaybackHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _qLectern.QLecternPlayback.QLecternPlaybackObserve(e.Parameter);
    }

    private void PDisplayGlyphHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _qLectern.QLecternSound.QLecternGlyphObserve(e.Parameter);
    }

    internal void PDisplayClose()
    {
        _qLectern.QLecternSoundArea.CDisplayPlaybackCancel();
    }

    private void PDisplayCardHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternCard.QLecternChipObserve(e);
    }

    private void PDisplayIncomingHandle(object sender, RoutedEventArgs e)
    {
        _qLectern.QLecternCard.QLecternIncomingObserve(e);
    }

    private void PDisplayEtymonHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _qLectern.QLecternCard.QLecternEtymonObserve(e.Parameter);
    }

    private void PDisplayMentionHandle(object? sender, PMentionArgument e)
    {
        _qLectern.QLecternCard.QLecternMentionObserve(
            e.PMentionArgumentOrigin,
            e.PMentionArgumentText,
            e.PMentionArgumentLanguage,
            e.PMentionArgumentOffset,
            e.PMentionArgumentMention,
            _pDisplayHost.PWindowMentionHandle);
    }

    internal void PDisplayCardScroll(long id)
    {
        _qLectern.QLecternCard.QLecternSpotlightRefine(id);
    }

    private static void PSpeechApply(FrameworkElement container, object item, string? _)
    {
        if (item is string speech && QLook.QLookPartFind<TextBlock>(container, "PSpeechName") is TextBlock name)
        {
            name.Text = speech;
        }
    }

    private static void PUsageApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QUsageItem usage)
        {
            return;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PUsageIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("incoming", 24);
        }

        if (QLook.QLookPartFind<Run>(container, "PUsageName") is Run name)
        {
            name.Text = usage.QUsageItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PUsageEpithet") is Run epithet)
        {
            epithet.Text = "\u2002" + usage.QUsageItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageTitle") is TextBlock title)
        {
            title.Text = usage.QUsageItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageOwner") is TextBlock owner)
        {
            owner.Text = usage.QUsageItemOwner;
        }

        if (QLook.QLookPartFind<Image>(container, "PUsageFlag") is Image flag)
        {
            flag.Source = usage.QUsageItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUsageLanguage") is TextBlock language)
        {
            language.Text = usage.QUsageItemLanguage;
        }
    }

    private void PCompassApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QCompassItem compass)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PCompassRow") is Button row)
        {
            if (compass.QCompassItemCurrent)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= _pDisplayCompass.PCompassRowHandle;
            row.Click += _pDisplayCompass.PCompassRowHandle;
        }

        if (QLook.QLookPartFind<Grid>(container, "PCompassIndent") is Grid indent)
        {
            indent.Margin = compass.QCompassItemIndent;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCompassNumber") is TextBlock number)
        {
            number.Text = compass.QCompassItemNumber;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCompassName") is TextBlock name)
        {
            name.Text = compass.QCompassItemName;
        }
    }
}
