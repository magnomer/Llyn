using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

internal sealed class QDisplay
{
    private readonly FrameworkElement _qDisplaySurface;
    private QLectern _qDisplayLectern = null!;

    internal QDisplay(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qDisplaySurface = surface;
    }

    private TextBlock QDisplayEmpty => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayEmpty");

    private ScrollViewer QDisplayContents =>
        QContract.QContractFind<ScrollViewer>(_qDisplaySurface, "PDisplayContents");

    private Grid QDisplayHeader => QContract.QContractFind<Grid>(_qDisplaySurface, "PDisplayHeader");

    private TextBlock QDisplayHeadword => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayHeadword");

    private TextBlock QDisplayReading => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayReading");

    private Image QDisplayLanguageFlag => QContract.QContractFind<Image>(_qDisplaySurface, "PDisplayLanguageFlag");

    private Ellipse QDisplayLanguageGlobe =>
        QContract.QContractFind<Ellipse>(_qDisplaySurface, "PDisplayLanguageGlobe");

    private TextBlock QDisplayLanguage => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayLanguage");

    private ToggleButton QDisplayFavorite =>
        QContract.QContractFind<ToggleButton>(_qDisplaySurface, "PDisplayFavorite");

    private PGrasp QDisplayGrasp => QContract.QContractFind<PGrasp>(_qDisplaySurface, "PDisplayGrasp");

    private TextBlock QDisplayGraspLabel => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayGraspLabel");

    private ItemsControl QDisplayReflex => QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplayReflex");

    private TextBlock QDisplayReflexLoading =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayReflexLoading");

    private ToggleButton QDisplayReflexFold =>
        QContract.QContractFind<ToggleButton>(_qDisplaySurface, "PDisplayReflexFold");

    private Border QDisplayPronunciationSurface =>
        QContract.QContractFind<Border>(_qDisplaySurface, "PDisplayPronunciationSurface");

    private ColumnDefinition QDisplayPronunciationLead =>
        QContract.QContractFind<ColumnDefinition>(_qDisplaySurface, "PDisplayPronunciationLead");

    private Image QDisplayPronunciationFlag =>
        QContract.QContractFind<Image>(_qDisplaySurface, "PDisplayPronunciationFlag");

    private TextBlock QDisplayPronunciationLabel =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayPronunciationLabel");

    private TextBlock QDisplayPronunciationOpener =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayPronunciationOpener");

    private TextBlock QDisplayPronunciation =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayPronunciation");

    private TextBlock QDisplayPronunciationCloser =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayPronunciationCloser");

    private Button QDisplayPlaybackAction => QContract.QContractFind<Button>(_qDisplaySurface, "PPlaybackAction");

    private Border QDisplayPlayback => QContract.QContractFind<Border>(_qDisplaySurface, "PPlayback");

    private Slider QDisplayVolume => QContract.QContractFind<Slider>(_qDisplaySurface, "PVolume");

    private PContour QDisplayContour => QContract.QContractFind<PContour>(_qDisplaySurface, "PDisplayContour");

    private ItemsControl QDisplayAccent => QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplayAccent");

    private ItemsControl QDisplayTranscription =>
        QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplayTranscription");

    private Border QDisplayGlyphSection => QContract.QContractFind<Border>(_qDisplaySurface, "PDisplayGlyphSection");

    private ColumnDefinition QDisplayGlyphLead =>
        QContract.QContractFind<ColumnDefinition>(_qDisplaySurface, "PDisplayGlyphLead");

    private TextBlock QDisplayGlyphLabel => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayGlyphLabel");

    private ItemsControl QDisplayGlyph => QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplayGlyph");

    private StackPanel QDisplaySpeechSection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplaySpeechSection");

    private ItemsControl QDisplaySpeech => QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplaySpeech");

    private StackPanel QDisplayFrequencySection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayFrequencySection");

    private Border QDisplayFrequencyChip => QContract.QContractFind<Border>(_qDisplaySurface, "PDisplayFrequencyChip");

    private TextBlock QDisplayFrequency => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayFrequency");

    private TextBlock QDisplayFrequencyBand =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayFrequencyBand");

    private QParadigm QDisplayParadigm => QContract.QContractFind<QParadigm>(_qDisplaySurface, "PDisplayParadigm");

    private QFanqie QDisplayFanqie => QContract.QContractFind<QFanqie>(_qDisplaySurface, "PDisplayFanqie");

    private StackPanel QDisplayMeaningSection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayMeaningSection");

    private ItemsControl QDisplayMeaning => QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplayMeaning");

    private StackPanel QDisplayCollocationSection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayCollocationSection");

    private ItemsControl QDisplayCollocation =>
        QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplayCollocation");

    private StackPanel QDisplayIncomingSection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayIncomingSection");

    private ItemsControl QDisplayIncoming =>
        QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplayIncoming");

    private StackPanel QDisplayEtymologySection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayEtymologySection");

    private QEtymology QDisplayEtymology => QContract.QContractFind<QEtymology>(_qDisplaySurface, "PDisplayEtymology");

    private StackPanel QDisplayNoteSection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayNoteSection");

    private StackPanel QDisplayNote => QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayNote");

    private QScript QDisplayScript => QContract.QContractFind<QScript>(_qDisplaySurface, "PDisplayScript");

    private StackPanel QDisplayStampSection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayStampSection");

    private TextBlock QDisplayStampAdded => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayStampAdded");

    private TextBlock QDisplayStampUpdated =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayStampUpdated");

    private PSwath QDisplaySwath => QContract.QContractFind<PSwath>(_qDisplaySurface, "PDisplaySwath");

    private StackPanel QDisplayCompass => QContract.QContractFind<StackPanel>(_qDisplaySurface, "PCompass");

    private Border QDisplayCompassSurface => QContract.QContractFind<Border>(_qDisplaySurface, "PCompassSurface");

    private ToggleButton QDisplayCompassSwitch =>
        QContract.QContractFind<ToggleButton>(_qDisplaySurface, "PCompassSwitch");

    private ItemsControl QDisplayCompassList => QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PCompassList");

    private QIconImage QDisplayCompassIcon => QContract.QContractFind<QIconImage>(_qDisplaySurface, "PCompassIcon");

    internal void QDisplayVisibleRefine(Visibility visible)
    {
        _qDisplaySurface.Visibility = visible;
    }

    internal void QDisplayIntroduce(QWindow host, QLectern lectern)
    {
        _qDisplayLectern = lectern;
        QLook.QLookStyleAttach(_qDisplaySurface);

        QDisplaySwath.PSwathAttach(QDisplayContents);

        _qDisplaySurface.CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, QDisplayEtymonObserve));
        QDisplayAccent.CommandBindings.Add(new CommandBinding(
            QAccentCommand.QAccentCommandPlayback, QDisplayPlaybackObserve));
        QDisplayGlyph.CommandBindings.Add(new CommandBinding(
            QGlyphCommand.QGlyphCommandEntry, QDisplayGlyphObserve));

        QDisplayFavorite.Click += QDisplayFavoriteObserve;
        QDisplayGrasp.PGraspChanged += QDisplayGraspObserve;
        QDisplayGrasp.PGraspHovered += QDisplayHoverRefine;
        QDisplayReflexFold.Checked += QDisplayFoldObserve;
        QDisplayReflexFold.Unchecked += QDisplayFoldObserve;
        QDisplayPlaybackAction.Click += QDisplayActionObserve;
        QDisplayPlaybackAction.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
        QDisplayCompassIcon.QIconSource = QIcon.QIconResolve("compass", 24);
        QDisplayMeaning.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QDisplayCardObserve));
        QDisplayCollocation.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QDisplayCardObserve));
        QDisplayIncoming.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QDisplayIncomingObserve));
        QDisplayMeaning.AddHandler(
            PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(QDisplayMentionObserve));
        QDisplayCollocation.AddHandler(
            PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(QDisplayMentionObserve));
        QDisplayEtymology.AddHandler(
            PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(QDisplayEtymologyObserve));

        host.QWindowVolume.QVolumeSliderAttach(_qDisplaySurface);

        QLookItem.QLookItemAttach(QDisplaySpeech, QDisplaySpeechRefine);
        QLookItem.QLookItemAttach(QDisplayMeaning, PLeaf.PLeafCardRefine);
        QLookItem.QLookItemAttach(QDisplayCollocation, PLeaf.PLeafCardRefine);
        QLookItem.QLookItemAttach(QDisplayIncoming, QDisplayUsageRefine);
        QLookItem.QLookItemAttach(QDisplayCompassList, QDisplayCompassRefine);

        QDisplayLecternAttach(host, lectern);
    }

    private void QDisplayLecternAttach(QWindow host, QLectern lectern)
    {
        lectern.QLecternIntroduce(host.QWindowAtelier, QDisplayEmpty, QDisplayContents, QDisplaySwath.PSwathClear);
        lectern.QLecternHeaderIntroduce(
            QDisplayHeadword,
            QDisplayLanguage,
            QDisplayLanguageFlag,
            QDisplayLanguageGlobe,
            QDisplayFavorite,
            QDisplayGrasp,
            PGrasp.PGraspStepProperty,
            PGrasp.PGraspLimitProperty,
            QDisplayGraspLabel);
        lectern.QLecternStampIntroduce(QDisplayStampSection, QDisplayStampAdded, QDisplayStampUpdated);
        lectern.QLecternFrequencyIntroduce(
            QDisplayFrequencySection, QDisplayFrequencyChip, QDisplayFrequency, QDisplayFrequencyBand);
        lectern.QLecternSpeechIntroduce(QDisplaySpeechSection, QDisplaySpeech);
        lectern.QLecternNoteIntroduce(QDisplayNoteSection, QDisplayNote);
        lectern.QLecternPlayback.QLecternPlaybackIntroduce(QDisplayPlaybackAction, QDisplayPlayback, QDisplayVolume);
        lectern.QLecternAccent.QLecternAccentIntroduce(
            QDisplayPronunciationSurface,
            QDisplayPronunciationLead,
            QDisplayPronunciationFlag,
            QDisplayPronunciationLabel,
            QDisplayPronunciationOpener,
            QDisplayPronunciation,
            QDisplayPronunciationCloser,
            QDisplayAccent,
            QDisplayContour,
            PContour.PContourSyllablesProperty,
            PContour.PContourScaleProperty);
        lectern.QLecternSound.QLecternGlyphIntroduce(
            QDisplayTranscription,
            QDisplayGlyphSection,
            QDisplayGlyphLead,
            QDisplayGlyphLabel,
            QDisplayGlyph);
        lectern.QLecternSound.QLecternReflexIntroduce(QDisplayReflex, QDisplayReflexLoading, QDisplayReflexFold);
        lectern.QLecternSound.QLecternFanqieIntroduce(
            QDisplayFanqie,
            QDisplayReading,
            QDisplayFanqie.QFanqieRefine);
        lectern.QLecternSound.QLecternScriptIntroduce(QDisplayScript, QDisplayScript.QScriptRefine);
        lectern.QLecternSound.QLecternParadigmIntroduce(QDisplayParadigm, QDisplayParadigm.QParadigmRefine);
        lectern.QLecternCompassIntroduce(
            _qDisplaySurface,
            QDisplayContents,
            QDisplayHeader,
            QDisplayCompass,
            QDisplayCompassSurface,
            QDisplayCompassSwitch,
            QDisplayCompassList);
        lectern.QLecternCompass.QCompassSectionIntroduce(
            QDisplaySpeechSection,
            QDisplayFrequencySection,
            QDisplayMeaningSection,
            QDisplayMeaning,
            QDisplayCollocationSection,
            QDisplayCollocation,
            QDisplayIncomingSection,
            QDisplayNoteSection);
        lectern.QLecternCard.QLecternCardIntroduce(
            _qDisplaySurface.Resources,
            QDisplayMeaning,
            QDisplayMeaningSection,
            QDisplayCollocation,
            QDisplayCollocationSection,
            QDisplayContents,
            lectern.QLecternCompass);
        lectern.QLecternCard.QLecternIncomingIntroduce(QDisplayIncoming, QDisplayIncomingSection);
        lectern.QLecternCard.QLecternEtymologyIntroduce(QDisplayEtymology, QDisplayEtymologySection);
        lectern.QLecternCard.QLecternRouteIntroduce(host);
        QDisplayFanqie.QFanqieDiweiNotice += lectern.QLecternSound.QLecternDiweiObserve;
        QDisplayFanqie.QFanqieStemNotice += lectern.QLecternSound.QLecternStemObserve;
        QDisplayFanqie.QFanqieRepresentativeNotice += lectern.QLecternSoundArea.CDisplayFanqieSet;
    }

    private void QDisplayFoldObserve(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternSound.QLecternFoldObserve();
    }

    private void QDisplayFavoriteObserve(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternFavoriteObserve();
    }

    private void QDisplayGraspObserve(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternGraspObserve(QDisplayGrasp.PGraspStep);
    }

    private void QDisplayHoverRefine(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternHoverRefine(QDisplayGrasp.PGraspPointed);
    }

    private void QDisplayActionObserve(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternPlayback.QLecternActionObserve();
    }

    private void QDisplayPlaybackObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qDisplayLectern.QLecternPlayback.QLecternPlaybackObserve(e.Parameter);
    }

    private void QDisplayGlyphObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qDisplayLectern.QLecternSound.QLecternGlyphObserve(e.Parameter);
    }

    private void QDisplayCardObserve(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternCard.QLecternChipObserve(e);
    }

    private void QDisplayIncomingObserve(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternCard.QLecternIncomingObserve(e);
    }

    private void QDisplayEtymonObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qDisplayLectern.QLecternCard.QLecternEtymonObserve(e.Parameter);
    }

    private void QDisplayMentionObserve(object? sender, PMentionArgument e)
    {
        _qDisplayLectern.QLecternCard.QLecternMentionObserve(e);
    }

    private void QDisplayEtymologyObserve(object? sender, PMentionArgument e)
    {
        _qDisplayLectern.QLecternCard.QLecternEtymologyObserve(e);
    }

    private static void QDisplaySpeechRefine(FrameworkElement container, object item, string? _)
    {
        if (item is string speech && QLook.QLookPartFind<TextBlock>(container, "PSpeechName") is TextBlock name)
        {
            name.Text = speech;
        }
    }

    private static void QDisplayUsageRefine(FrameworkElement container, object item, string? _)
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
            epithet.Text = QLook.QLookEpithetRead(usage.QUsageItemEpithet);
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

    private void QDisplayCompassRefine(FrameworkElement container, object item, string? _)
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

            row.Click -= QDisplayCompassObserve;
            row.Click += QDisplayCompassObserve;
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

    private void QDisplayCompassObserve(object sender, RoutedEventArgs e)
    {
        _qDisplayLectern.QLecternCompass.QCompassRowRefine(sender);
    }
}
