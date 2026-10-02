using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLectern
{
    private readonly LDisplay _qLecternDisplay;

    private CAtelier _qLecternAtelier = null!;

    private UIElement _qLecternEmpty = null!;

    private UIElement _qLecternContents = null!;

    private TextBlock _qLecternHeadword = null!;

    private TextBlock _qLecternLanguage = null!;

    private Image _qLecternFlag = null!;

    private UIElement _qLecternGlobe = null!;

    private ToggleButton _qLecternFavorite = null!;

    private DependencyObject _qLecternGrasp = null!;

    private DependencyProperty _qLecternStep = null!;

    private TextBlock _qLecternGraspLabel = null!;

    private UIElement _qLecternStamp = null!;

    private TextBlock _qLecternAdded = null!;

    private TextBlock _qLecternUpdated = null!;

    private UIElement _qLecternFrequencySection = null!;

    private FrameworkElement _qLecternChip = null!;

    private TextBlock _qLecternName = null!;

    private TextBlock _qLecternBand = null!;

    private UIElement _qLecternSpeechSection = null!;

    private ItemsControl _qLecternSpeech = null!;

    private UIElement _qLecternNoteSection = null!;

    private Panel _qLecternNote = null!;

    public QLectern(LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _qLecternDisplay = display;
        QLecternArea = display.CDisplayArea;
        QLecternSoundArea = display.CDisplaySound;
        QLecternAccent = new QLecternAccent(display.CDisplaySound);
        QLecternSound = new QLecternSound(display.CDisplaySound);
        QLecternPlayback = new QLecternPlayback(display.CDisplaySound);
        QLecternCard = new QLecternCard(display);
    }

    public QLectern(LDisplay display, CPanel panel)
        : this(display)
    {
        QLecternArea.CDisplayPanelAttach(panel);
    }

    public CDisplay QLecternArea { get; }

    public CDisplaySound QLecternSoundArea { get; }

    public QLecternAccent QLecternAccent { get; }

    public QLecternSound QLecternSound { get; }

    public QLecternPlayback QLecternPlayback { get; }

    public QLecternCard QLecternCard { get; }

    public QCompass QLecternCompass { get; private set; } = null!;

    public void QLecternCompassIntroduce(
        FrameworkElement view,
        ScrollViewer contents,
        FrameworkElement header,
        FrameworkElement compass,
        UIElement surface,
        ToggleButton toggle,
        ItemsControl list)
    {
        QLecternCompass = new QCompass(_qLecternDisplay, view, contents, header, compass, surface, toggle, list);
        QLecternArea.CDisplayOpened += QLecternCompass.QCompassRefine;
        QLecternArea.CDisplayClosed += QLecternCompass.QCompassEmptyRefine;
    }

    public void QLecternIntroduce(CAtelier atelier, UIElement empty, UIElement contents, Action swathSeam)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(empty);
        ArgumentNullException.ThrowIfNull(contents);
        ArgumentNullException.ThrowIfNull(swathSeam);

        _qLecternAtelier = atelier;
        _qLecternEmpty = empty;
        _qLecternContents = contents;

        QLecternArea.CDisplayOpened += swathSeam;
        QLecternArea.CDisplayOpened += QLecternEntryRefine;
        QLecternArea.CDisplayOpened += QLecternFontRefine;
        QLecternArea.CDisplayOpened += QLecternFlagRefine;
        QLecternArea.CDisplayOpened += QLecternNoteRefine;
        QLecternArea.CDisplayOpened += QLecternFavoriteRefine;
        QLecternArea.CDisplayOpened += QLecternGraspRefine;
        QLecternArea.CDisplayOpened += QLecternFrequencyRefine;
        QLecternArea.CDisplayOpened += QLecternPlayback.QLecternPlaybackRefine;
        QLecternArea.CDisplayOpened += QLecternAccent.QLecternAccentRefine;
        QLecternArea.CDisplayOpened += QLecternAccent.QLecternEnsignRefine;
        QLecternArea.CDisplayOpened += QLecternSound.QLecternGlyphRefine;
        QLecternArea.CDisplayOpened += QLecternSound.QLecternTranscriptionRefine;
        QLecternArea.CDisplayOpened += QLecternSound.QLecternReflexRefine;
        QLecternArea.CDisplayOpened += QLecternSound.QLecternFoldRefine;
        QLecternArea.CDisplayOpened += QLecternSound.QLecternParadigmRefine;
        QLecternArea.CDisplayOpened += QLecternSound.QLecternScriptRefine;
        QLecternArea.CDisplayOpened += QLecternSound.QLecternFanqieRefine;
        QLecternArea.CDisplayOpened += QLecternCard.QLecternExampleRefine;
        QLecternArea.CDisplayOpened += QLecternCard.QLecternGlossRefine;
        QLecternArea.CDisplayOpened += QLecternCard.QLecternCardRefine;
        QLecternArea.CDisplayOpened += QLecternCard.QLecternIncomingRefine;
        QLecternArea.CDisplayOpened += QLecternCard.QLecternEtymologyRefine;

        QLecternArea.CDisplayClosed += swathSeam;
        QLecternArea.CDisplayClosed += QLecternEmptyRefine;
        QLecternArea.CDisplayClosed += QLecternPlayback.QLecternTrayRefine;
        QLecternArea.CDisplayClosed += QLecternAccent.QLecternMuteRefine;
        QLecternArea.CDisplayClosed += QLecternSound.QLecternSilenceRefine;
        QLecternArea.CDisplayClosed += QLecternCard.QLecternBlankRefine;

        QLecternArea.CDisplayFavoriteChanged += QObserver.QObserverCreate<CBulletin>(contents, QLecternFavoriteRefine);
        QLecternArea.CDisplayGraspChanged += QObserver.QObserverCreate<CBulletin>(contents, QLecternGraspRefine);
        QLecternArea.CDisplayFrequencyChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternFrequencyRefine);
        QLecternArea.CDisplayParadigmChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternSound.QLecternParadigmRefine);
        QLecternArea.CDisplayReflexChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternSound.QLecternRenewalRefine);
        QLecternArea.CDisplayReflexChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternSound.QLecternFoldRefine);
        QLecternArea.CDisplayScriptChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternSound.QLecternScriptRefine);
        QLecternArea.CDisplayFanqieChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternSound.QLecternFanqieRefine);
        QLecternArea.CDisplayEntryChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternArea.CDisplayEntryResonate);
        QLecternArea.CDisplayWorkspaceChanged +=
            QObserver.QObserverCreate<CBulletin>(contents, QLecternArea.CDisplayWorkspaceResonate);
    }

    public void QLecternHeaderIntroduce(
        TextBlock headword,
        TextBlock language,
        Image flag,
        UIElement globe,
        ToggleButton favorite,
        DependencyObject grasp,
        DependencyProperty step,
        DependencyProperty limit,
        TextBlock graspLabel)
    {
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(flag);
        ArgumentNullException.ThrowIfNull(globe);
        ArgumentNullException.ThrowIfNull(favorite);
        ArgumentNullException.ThrowIfNull(grasp);
        ArgumentNullException.ThrowIfNull(step);
        ArgumentNullException.ThrowIfNull(limit);
        ArgumentNullException.ThrowIfNull(graspLabel);

        _qLecternHeadword = headword;
        _qLecternLanguage = language;
        _qLecternFlag = flag;
        _qLecternGlobe = globe;
        _qLecternFavorite = favorite;
        _qLecternGrasp = grasp;
        _qLecternStep = step;
        _qLecternGraspLabel = graspLabel;
        grasp.SetValue(limit, QLecternArea.CDisplayGraspStep);
    }

    public void QLecternStampIntroduce(UIElement section, TextBlock added, TextBlock updated)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(added);
        ArgumentNullException.ThrowIfNull(updated);

        _qLecternStamp = section;
        _qLecternAdded = added;
        _qLecternUpdated = updated;
    }

    public void QLecternFrequencyIntroduce(UIElement section, FrameworkElement chip, TextBlock name, TextBlock band)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(chip);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(band);

        _qLecternFrequencySection = section;
        _qLecternChip = chip;
        _qLecternName = name;
        _qLecternBand = band;
    }

    public void QLecternSpeechIntroduce(UIElement section, ItemsControl speech)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(speech);

        _qLecternSpeechSection = section;
        _qLecternSpeech = speech;
    }

    public void QLecternNoteIntroduce(UIElement section, Panel note)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(note);

        _qLecternNoteSection = section;
        _qLecternNote = note;
    }

    public void QLecternFavoriteObserve()
    {
        QLecternFavoriteRefine(
            QLecternArea.CDisplayFavoriteToggle(QLook.QLookCheckedRead(_qLecternFavorite.IsChecked)));
    }

    public void QLecternGraspObserve(int step)
    {
        QLecternGraspRefine(QLecternArea.CDisplayGraspSet(step));
    }

    public void QLecternHoverRefine(int pointed)
    {
        _qLecternGraspLabel.Text = QLecternArea.CDisplayGraspRead(pointed);
    }

    private void QLecternEntryRefine()
    {
        CLectern shown = QLecternArea.CDisplayShown;
        _qLecternHeadword.Text = shown.CLecternHeadword;
        _qLecternLanguage.Text = shown.CLecternLanguage;
        _qLecternSpeech.ItemsSource = shown.CLecternSpeeches;
        _qLecternSpeechSection.Visibility = QLook.QLookVisibleRead(shown.CLecternMarked);
        _qLecternNoteSection.Visibility = QLook.QLookVisibleRead(shown.CLecternNoted);
        _qLecternAdded.Text = shown.CLecternAdded;
        _qLecternUpdated.Text = shown.CLecternUpdated;
        _qLecternStamp.Visibility = QLook.QLookVisibleRead(shown.CLecternStamped);
        _qLecternEmpty.Visibility = Visibility.Collapsed;
        _qLecternContents.Visibility = Visibility.Visible;
    }

    private void QLecternFontRefine()
    {
        QFontFace.QFontRefine(QLecternSoundArea.CDisplayFontRead(CFontRole.CFontRoleHeadword), _qLecternHeadword);
        QFontFace.QFontBaselineRefine(_qLecternHeadword);
    }

    private async void QLecternFlagRefine()
    {
        QEnsignImage.QEnsignFlagRefine(_qLecternFlag, _qLecternGlobe, string.Empty);

        await _qLecternAtelier.CAtelierCatalog.CCatalogEnsignLoad(QEnsignImage.QEnsignDraw);

        QEnsignImage.QEnsignFlagRefine(_qLecternFlag, _qLecternGlobe, _qLecternLanguage.Text);
    }

    private void QLecternNoteRefine()
    {
        QMarkdownFace.QMarkdownRefine(_qLecternNote, QLecternArea.CDisplayShown.CLecternNote, _qLecternAtelier);
    }

    private void QLecternFavoriteRefine()
    {
        QLecternFavoriteRefine(QLecternArea.CDisplayFavoriteRead());
    }

    private void QLecternFavoriteRefine(bool marked)
    {
        _qLecternFavorite.IsChecked = marked;
    }

    private void QLecternGraspRefine()
    {
        QLecternGraspRefine(QLecternArea.CDisplayGraspRead());
    }

    private void QLecternGraspRefine(CGrasp grasp)
    {
        _qLecternGrasp.SetValue(_qLecternStep, grasp.CGraspStep);
        _qLecternGraspLabel.Text = grasp.CGraspLabel;
    }

    private void QLecternFrequencyRefine()
    {
        QFrequencyLabel.QFrequencyChipRefine(
            _qLecternFrequencySection,
            _qLecternChip,
            _qLecternName,
            _qLecternBand,
            QLecternArea.CDisplayFrequencyRead(QLocalizationCatalog.QLocalizationTextRead));
    }

    private void QLecternEmptyRefine()
    {
        _qLecternFavorite.IsChecked = false;
        _qLecternGrasp.SetValue(_qLecternStep, 0);
        _qLecternGraspLabel.Text = string.Empty;
        _qLecternLanguage.Text = string.Empty;
        QEnsignImage.QEnsignFlagRefine(_qLecternFlag, _qLecternGlobe, string.Empty);
        _qLecternSpeech.ItemsSource = null;
        _qLecternSpeechSection.Visibility = Visibility.Collapsed;
        QFrequencyLabel.QFrequencyChipRefine(
            _qLecternFrequencySection, _qLecternChip, _qLecternName, _qLecternBand, null);
        _qLecternNoteSection.Visibility = Visibility.Collapsed;
        _qLecternStamp.Visibility = Visibility.Collapsed;
        _qLecternContents.Visibility = Visibility.Collapsed;
        _qLecternEmpty.Visibility = Visibility.Visible;
    }
}
