using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

internal sealed class QDisplay
{
    private readonly FrameworkElement _qDisplaySurface;
    private readonly QSounding _qDisplaySounding;
    private readonly QRoster _qDisplayRoster = new();
    private QLectern _qDisplayLectern = null!;

    internal QDisplay(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qDisplaySurface = surface;
        _qDisplaySounding = new QSounding(surface);
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

    private StackPanel QDisplaySpeechSection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplaySpeechSection");

    private ItemsControl QDisplaySpeech => QContract.QContractFind<ItemsControl>(_qDisplaySurface, "PDisplaySpeech");

    private StackPanel QDisplayFrequencySection =>
        QContract.QContractFind<StackPanel>(_qDisplaySurface, "PDisplayFrequencySection");

    private Border QDisplayFrequencyChip => QContract.QContractFind<Border>(_qDisplaySurface, "PDisplayFrequencyChip");

    private TextBlock QDisplayFrequency => QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayFrequency");

    private TextBlock QDisplayFrequencyBand =>
        QContract.QContractFind<TextBlock>(_qDisplaySurface, "PDisplayFrequencyBand");

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

        QDisplayFavorite.Click += QDisplayFavoriteObserve;
        QDisplayGrasp.PGraspChanged += QDisplayGraspObserve;
        QDisplayGrasp.PGraspHovered += QDisplayHoverRefine;
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

        _qDisplayRoster.QRosterIntroduce(
            lectern, QDisplaySpeech, QDisplayMeaning, QDisplayCollocation, QDisplayIncoming, QDisplayCompassList);

        QDisplayLecternAttach(host, lectern);
    }

    private void QDisplayLecternAttach(QWindow host, QLectern lectern)
    {
        lectern.QLecternIntroduce(
            host.QWindowAtelier, host.QWindowEnvoy, QDisplayEmpty, QDisplayContents, QDisplaySwath.PSwathClear);
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
        _qDisplaySounding.QSoundingIntroduce(host, lectern);
        lectern.QLecternSound.QLecternFanqieIntroduce(
            QDisplayFanqie,
            QDisplayReading,
            QDisplayFanqie.QFanqieRefine);
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
}
