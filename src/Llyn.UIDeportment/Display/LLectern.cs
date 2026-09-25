using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LLectern
{
    private readonly LDisplay _lLecternDisplay;

    private LWindow _lLecternWindow = null!;

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

    public LCompass LLecternCompass { get; private set; } = null!;

    public void LLecternCompassAttach(
        FrameworkElement view,
        ScrollViewer contents,
        FrameworkElement header,
        FrameworkElement compass,
        UIElement surface,
        ToggleButton toggle,
        ItemsControl list)
    {
        LLecternCompass = new LCompass(_lLecternDisplay, view, contents, header, compass, surface, toggle, list);
    }

    public bool LLecternFoldOpened => _lLecternDisplay.LDisplaySound.LDisplayFoldOpened;

    public void LLecternAttach(LWindow window, UIElement empty, UIElement contents, Action swathSeam)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(empty);
        ArgumentNullException.ThrowIfNull(contents);
        ArgumentNullException.ThrowIfNull(swathSeam);

        _lLecternWindow = window;
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
            LSubject.LSubjectFavorite, LObserver.LObserverCreate(surface, LLecternFavoriteUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            LSubject.LSubjectGrasp, LObserver.LObserverCreate(surface, LLecternGraspUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            LSubject.LSubjectFrequency, LObserver.LObserverCreate(surface, LLecternFrequencyUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            LSubject.LSubjectInflection, LObserver.LObserverCreate(surface, LLecternSound.LLecternParadigmUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            LSubject.LSubjectReflex, LObserver.LObserverCreate(surface, LLecternSound.LLecternReflexUpdate));
        _lLecternDisplay.LDisplayChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectScript, LObserver.LObserverCreate(surface, LLecternSound.LLecternScriptUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectFanqie, LObserver.LObserverCreate(surface, LLecternSound.LLecternFanqieUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(surface, LLecternClear));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectExample, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectSituation, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectReference, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectAuthor, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectTag, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectRegister, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
        _lLecternDisplay.LDisplayObserverAttach(
            LSubject.LSubjectSettings, LObserver.LObserverCreate(surface, LLecternEntryUpdate));
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
        LMarkdownFace.LMarkdownShow(_lLecternNote, draft.LEntryDraftNote, _lLecternWindow);
        _lLecternNoteSection.Visibility = draft.LEntryDraftNoted ? Visibility.Visible : Visibility.Collapsed;
        LLecternStampShow(_lLecternDisplay.LDisplayStampRead());

        _lLecternEmpty.Visibility = Visibility.Collapsed;
        _lLecternContents.Visibility = Visibility.Visible;
        LLecternCompass.LCompassUpdate();
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
        LLecternCompass.LCompassClear();

        _lLecternLanguage.Text = string.Empty;
        LEnsignImage.LEnsignFlagShow(_lLecternFlag, _lLecternGlobe, string.Empty);
        _lLecternSpeech.ItemsSource = null;
        _lLecternSpeechSection.Visibility = Visibility.Collapsed;
        LFrequencyLabel.LFrequencyChipShow(_lLecternFrequencySection, _lLecternChip, _lLecternName, _lLecternBand, []);
        _lLecternNoteSection.Visibility = Visibility.Collapsed;
        _lLecternStamp.Visibility = Visibility.Collapsed;
        _lLecternContents.Visibility = Visibility.Collapsed;
        _lLecternEmpty.Visibility = Visibility.Visible;
    }

    public void LLecternClose() => _lLecternDisplay.LDisplaySound.LDisplayPlaybackStop();

    public void LLecternFavoriteHandle()
    {
        _lLecternDisplay.LDisplayFavoriteSave(_lLecternFavorite.IsChecked is true);
        LLecternFavoriteUpdate();
    }

    public void LLecternGraspHandle(int step)
    {
        _lLecternDisplay.LDisplayGraspSave(step);
        LLecternGraspUpdate();
    }

    public void LLecternHoverHandle(int pointed)
    {
        _lLecternGraspLabel.Text = _lLecternDisplay.LDisplayGraspFormat(pointed);
    }

    private void LLecternGraspShow(int step)
    {
        _lLecternGrasp.SetValue(_lLecternStep, step);
        _lLecternGraspLabel.Text = _lLecternDisplay.LDisplayGraspFormat(step);
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
        LEnsignImage.LEnsignFlagShow(_lLecternFlag, _lLecternGlobe, string.Empty);
        LFontFace.LFontApply(_lLecternWindow, language, _lLecternHeadword);
        LFontFace.LFontPlace(_lLecternHeadword);

        await LEnsignImage.LEnsignLoad(_lLecternWindow);

        LEnsignImage.LEnsignFlagShow(_lLecternFlag, _lLecternGlobe, _lLecternDisplay.LDisplayLanguageRead());
    }

    private void LLecternFavoriteUpdate()
    {
        _lLecternFavorite.IsChecked = _lLecternDisplay.LDisplayFavoriteRead();
    }

    private void LLecternGraspUpdate()
    {
        LLecternGraspShow(_lLecternDisplay.LDisplayGraspRead());
    }

    private void LLecternFrequencyUpdate()
    {
        LFrequencyLabel.LFrequencyChipShow(
            _lLecternFrequencySection,
            _lLecternChip,
            _lLecternName,
            _lLecternBand,
            _lLecternDisplay.LDisplayFrequencyRead());
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

    public IReadOnlyList<LTranslationTarget> LLecternEtymonRead(LEntryDraft draft) =>
        _lLecternDisplay.LDisplayEtymonRead(draft);

    public static bool LLecternNarrativeCheck(bool editable, string text) =>
        LDisplay.LDisplayNarrativeCheck(editable, text);

    public static bool LLecternEtymonCheck(bool editable, int count) => LDisplay.LDisplayEtymonCheck(editable, count);

    public void LLecternReflexStart(long id) => _lLecternDisplay.LDisplaySound.LDisplayReflexStart(id);

    public bool LLecternReflexCheck(long id) => _lLecternDisplay.LDisplaySound.LDisplayReflexCheck(id);

    public void LLecternReflexRebuild(long id) => _lLecternDisplay.LDisplaySound.LDisplayReflexRebuild(id);
}
