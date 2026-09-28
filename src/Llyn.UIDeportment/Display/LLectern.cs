using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LLectern
{
    private readonly LDisplay _lLecternDisplay;

    private CAtelier _lLecternAtelier = null!;

    private UIElement _lLecternEmpty = null!;

    private UIElement _lLecternContents = null!;

    private Action _lLecternSwath = null!;

    private TextBlock _lLecternHeadword = null!;

    private TextBlock _lLecternLanguage = null!;

    private Image _lLecternFlag = null!;

    private UIElement _lLecternGlobe = null!;

    private ToggleButton _lLecternFavorite = null!;

    private DependencyObject _lLecternGrasp = null!;

    private DependencyProperty _lLecternStep = null!;

    private TextBlock _lLecternGraspLabel = null!;

    private UIElement _lLecternStamp = null!;

    private TextBlock _lLecternAdded = null!;

    private TextBlock _lLecternUpdated = null!;

    private UIElement _lLecternFrequencySection = null!;

    private FrameworkElement _lLecternChip = null!;

    private TextBlock _lLecternName = null!;

    private TextBlock _lLecternBand = null!;

    private UIElement _lLecternSpeechSection = null!;

    private ItemsControl _lLecternSpeech = null!;

    private UIElement _lLecternNoteSection = null!;

    private Panel _lLecternNote = null!;

    public LLectern(LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternDisplay = display;
        LLecternAccent = new LLecternAccent(display.LDisplaySound);
        LLecternSound = new LLecternSound(display.LDisplaySound);
        LLecternPlayback = new LLecternPlayback(display.LDisplaySound);
        LLecternCard = new LLecternCard(display);
    }

    public LLecternAccent LLecternAccent { get; }

    public LLecternSound LLecternSound { get; }

    public LLecternPlayback LLecternPlayback { get; }

    public LLecternCard LLecternCard { get; }

    public QCompass LLecternCompass { get; private set; } = null!;

    public void LLecternCompassAttach(
        FrameworkElement view,
        ScrollViewer contents,
        FrameworkElement header,
        FrameworkElement compass,
        UIElement surface,
        ToggleButton toggle,
        ItemsControl list)
    {
        LLecternCompass = new QCompass(_lLecternDisplay, view, contents, header, compass, surface, toggle, list);
    }

    public bool LLecternFoldOpened => _lLecternDisplay.LDisplaySound.LDisplayFoldOpened;

    public void LLecternAttach(CAtelier atelier, UIElement empty, UIElement contents, Action swathSeam)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(empty);
        ArgumentNullException.ThrowIfNull(contents);
        ArgumentNullException.ThrowIfNull(swathSeam);

        _lLecternAtelier = atelier;
        _lLecternEmpty = empty;
        _lLecternContents = contents;
        _lLecternSwath = swathSeam;
    }

    private bool LLecternAttachCheck()
    {
        if (_lLecternSwath is null)
        {
            return false;
        }

        return LLecternCompass is null
            ? throw new InvalidOperationException("LLecternCompassAttach must run before the lectern shows.")
            : true;
    }

    public void LLecternHeaderAttach(
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

        _lLecternHeadword = headword;
        _lLecternLanguage = language;
        _lLecternFlag = flag;
        _lLecternGlobe = globe;
        _lLecternFavorite = favorite;
        _lLecternGrasp = grasp;
        _lLecternStep = step;
        _lLecternGraspLabel = graspLabel;
        grasp.SetValue(limit, _lLecternDisplay.LDisplayGraspStep);
    }

    public void LLecternStampAttach(UIElement section, TextBlock added, TextBlock updated)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(added);
        ArgumentNullException.ThrowIfNull(updated);

        _lLecternStamp = section;
        _lLecternAdded = added;
        _lLecternUpdated = updated;
    }

    public void LLecternFrequencyAttach(UIElement section, FrameworkElement chip, TextBlock name, TextBlock band)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(chip);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(band);

        _lLecternFrequencySection = section;
        _lLecternChip = chip;
        _lLecternName = name;
        _lLecternBand = band;
    }

    public void LLecternSpeechAttach(UIElement section, ItemsControl speech)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(speech);

        _lLecternSpeechSection = section;
        _lLecternSpeech = speech;
    }

    public void LLecternNoteAttach(UIElement section, Panel note)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(note);

        _lLecternNoteSection = section;
        _lLecternNote = note;
    }

    public void LLecternObserverAttach(DispatcherObject surface)
    {
        _lLecternDisplay.LDisplayChosenAttach(
            CSubject.CSubjectFavorite, LObserver.LObserverCreate<CBulletin>(surface, LLecternFavoriteUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            CSubject.CSubjectGrasp, LObserver.LObserverCreate<CBulletin>(surface, LLecternGraspUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            CSubject.CSubjectFrequency, LObserver.LObserverCreate<CBulletin>(surface, LLecternFrequencyUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            CSubject.CSubjectInflection,
            LObserver.LObserverCreate<CBulletin>(surface, LLecternSound.LLecternParadigmUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, LLecternSound.LLecternReflexUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectScript, LObserver.LObserverCreate<CBulletin>(surface, LLecternSound.LLecternScriptUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectFanqie, LObserver.LObserverCreate<CBulletin>(surface, LLecternSound.LLecternFanqieUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, LLecternClear));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectExample, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectSituation, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectReference, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectAuthor, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectTag, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectRegister, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, LLecternEntryUpdate));
    }

    public void LLecternShow(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (!LLecternAttachCheck())
        {
            return;
        }

        if (_lLecternDisplay.LDisplayChosen is null)
        {
            LLecternClear();
            return;
        }

        _lLecternSwath();
        _lLecternDisplay.LDisplayShow(draft);
        LLecternFavoriteUpdate();
        LLecternGraspUpdate();
        LLecternPlayback.LLecternPlaybackShow();
        LLecternAccent.LLecternAccentShow();
        LLecternSound.LLecternSoundShow();
        LLecternCard.LLecternCardShow();

        _lLecternHeadword.Text = draft.LEntryDraftHeadword;
        LLecternLanguageShow(draft.LEntryDraftLanguage);
        _lLecternSpeech.ItemsSource = _lLecternDisplay.LDisplaySpeechRead();
        _lLecternSpeechSection.Visibility = draft.LEntryDraftMarked ? Visibility.Visible : Visibility.Collapsed;
        LLecternFrequencyUpdate();
        LMarkdownFace.LMarkdownRefine(_lLecternNote, draft.LEntryDraftNote, _lLecternAtelier);
        _lLecternNoteSection.Visibility = draft.LEntryDraftNoted ? Visibility.Visible : Visibility.Collapsed;
        LLecternStampShow(_lLecternDisplay.LDisplayStampRead());

        _lLecternEmpty.Visibility = Visibility.Collapsed;
        _lLecternContents.Visibility = Visibility.Visible;
        LLecternCompass.QCompassRefine();
    }

    public void LLecternClear()
    {
        if (!LLecternAttachCheck())
        {
            return;
        }

        _lLecternSwath();
        _lLecternDisplay.LDisplayClear();
        _lLecternFavorite.IsChecked = false;
        _lLecternGrasp.SetValue(_lLecternStep, 0);
        _lLecternGraspLabel.Text = string.Empty;
        LLecternPlayback.LLecternPlaybackClear();
        LLecternAccent.LLecternAccentClear();
        LLecternSound.LLecternSoundClear();
        LLecternCard.LLecternCardClear();
        LLecternCompass.QCompassEmptyRefine();

        _lLecternLanguage.Text = string.Empty;
        LEnsignImage.LEnsignFlagRefine(_lLecternFlag, _lLecternGlobe, string.Empty);
        _lLecternSpeech.ItemsSource = null;
        _lLecternSpeechSection.Visibility = Visibility.Collapsed;
        QFrequencyLabel.QFrequencyChipRefine(
            _lLecternFrequencySection, _lLecternChip, _lLecternName, _lLecternBand, null);
        _lLecternNoteSection.Visibility = Visibility.Collapsed;
        _lLecternStamp.Visibility = Visibility.Collapsed;
        _lLecternContents.Visibility = Visibility.Collapsed;
        _lLecternEmpty.Visibility = Visibility.Visible;
    }

    public void LLecternClose() => _lLecternDisplay.LDisplaySound.LDisplayPlaybackStop();

    public void LLecternFavoriteHandle()
    {
        _lLecternDisplay.LDisplayFavoriteSave(_lLecternDisplay.LDisplayChosen, _lLecternFavorite.IsChecked is true);
        LLecternFavoriteUpdate();
    }

    public void LLecternGraspHandle(int step)
    {
        _lLecternDisplay.CDisplayGraspSet(step);
        LLecternGraspUpdate();
    }

    public void LLecternHoverHandle(int pointed)
    {
        _lLecternGraspLabel.Text = _lLecternDisplay.LDisplayGraspFormat(_lLecternDisplay.LDisplayChosen, pointed);
    }

    private void LLecternGraspShow(int step)
    {
        _lLecternGrasp.SetValue(_lLecternStep, step);
        _lLecternGraspLabel.Text = _lLecternDisplay.LDisplayGraspFormat(_lLecternDisplay.LDisplayChosen, step);
    }

    private void LLecternStampShow(LDisplayStamp stamp)
    {
        _lLecternAdded.Text = stamp.LDisplayStampAdded;
        _lLecternUpdated.Text = stamp.LDisplayStampUpdated;
        _lLecternStamp.Visibility = stamp.LDisplayStampShown ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void LLecternLanguageShow(string language)
    {
        _lLecternLanguage.Text = language;
        LEnsignImage.LEnsignFlagRefine(_lLecternFlag, _lLecternGlobe, string.Empty);
        LFontFace.LFontRefine(_lLecternAtelier, language, CFontRole.CFontRoleHeadword, _lLecternHeadword);
        LFontFace.LFontPlace(_lLecternHeadword);

        await LEnsignImage.LEnsignLoad(_lLecternAtelier);

        LEnsignImage.LEnsignFlagRefine(_lLecternFlag, _lLecternGlobe, _lLecternDisplay.LDisplayLanguageRead());
    }

    private void LLecternFavoriteUpdate()
    {
        _lLecternFavorite.IsChecked = _lLecternDisplay.LDisplayFavoriteRead(_lLecternDisplay.LDisplayChosen);
    }

    private void LLecternGraspUpdate()
    {
        LLecternGraspShow(_lLecternDisplay.LDisplayGraspRead(_lLecternDisplay.LDisplayChosen));
    }

    private void LLecternFrequencyUpdate()
    {
        QFrequencyLabel.QFrequencyChipRefine(
            _lLecternFrequencySection,
            _lLecternChip,
            _lLecternName,
            _lLecternBand,
            _lLecternDisplay.LDisplayFrequencyRead(
                _lLecternDisplay.LDisplayChosen, QLocalizationCatalog.QLocalizationTextRead("Frequency.Once")));
    }

    private void LLecternEntryUpdate()
    {
        if (_lLecternDisplay.LDisplayChosen is null)
        {
            return;
        }

        _lLecternDisplay.LDisplayDraftLoad(LLecternDraftShow);
    }

    private void LLecternDraftShow(LEntryDraft? draft)
    {
        if (draft is null)
        {
            LLecternClear();
            return;
        }

        LLecternShow(draft);
    }

    public void LLecternLoadedShow()
    {
        LLecternDraftShow(_lLecternDisplay.LDisplayLoaded);
    }

    public void LLecternDraftShow(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LLecternShow(draft.LDraftContent);
    }

    public void LLecternFoldSet(bool opened) => _lLecternDisplay.LDisplaySound.LDisplayFoldSet(opened);

    public static bool LLecternNarrativeCheck(bool editable, string text) =>
        LDisplay.LDisplayNarrativeCheck(editable, text);

    public static bool LLecternEtymonCheck(bool editable, int count) => LDisplay.LDisplayEtymonCheck(editable, count);

    public void LLecternReflexStart(long id) => _lLecternDisplay.LDisplaySound.LDisplayReflexStart(id);

    public bool LLecternReflexCheck(long id) => _lLecternDisplay.LDisplaySound.LDisplayReflexCheck(id);

    public void LLecternReflexRebuild(long id) => _lLecternDisplay.LDisplaySound.LDisplayReflexRebuild(id);
}
