using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor : UserControl, PImageHost, PVideoHost, QChronicleHost
{
    private PWindow _pEditorHost = null!;

    private QEditor _qEditor = null!;

    private readonly QRegard _qRegard;

    private readonly QCadence _qCadence;

    public PEditor()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Core/PEditor.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        _pSentenceTemplate = new PSentenceTemplate(this);
        Resources.MergedDictionaries.Add(_pSentenceTemplate);
        _pContextTemplate = new PContextTemplate();
        Resources.MergedDictionaries.Add(_pContextTemplate);
        _pRegisterTemplate = new PRegisterTemplate(this);
        Resources.MergedDictionaries.Add(_pRegisterTemplate);
        _pImageTemplate = new PImageTemplate(this);
        Resources.MergedDictionaries.Add(_pImageTemplate);
        _pVideoTemplate = new PVideoTemplate(this);
        Resources.MergedDictionaries.Add(_pVideoTemplate);
        _pLabelTemplate = new PLabelTemplate();
        Resources.MergedDictionaries.Add(_pLabelTemplate);
        _pLinkTemplate = new PLinkTemplate(this);
        Resources.MergedDictionaries.Add(_pLinkTemplate);
        _pProspectTemplate = new PProspectTemplate(this);
        Resources.MergedDictionaries.Add(_pProspectTemplate);
        _pCandidateTemplate = new PCandidateTemplate(this);
        Resources.MergedDictionaries.Add(_pCandidateTemplate);
        _pSlateTemplate = new PSlateTemplate(this);
        Resources.MergedDictionaries.Add(_pSlateTemplate);
        Resources.MergedDictionaries.Add(new PMeaningTemplate());
        _pCollocationTemplate = new PCollocationTemplate(this);
        Resources.MergedDictionaries.Add(_pCollocationTemplate);
        Resources.MergedDictionaries.Add(new PLanguageTemplate());
        _pMarkerTemplate = new PMarkerTemplate();
        Resources.MergedDictionaries.Add(_pMarkerTemplate);
        _pCategoryTemplate = new PCategoryTemplate(this);
        Resources.MergedDictionaries.Add(_pCategoryTemplate);
        _pNotationTemplate = new PNotationTemplate(this);
        Resources.MergedDictionaries.Add(_pNotationTemplate);
        _pClipTemplate = new PClipTemplate(this);
        Resources.MergedDictionaries.Add(_pClipTemplate);
        QLook.QLookStyleAttach(_pClipTemplate);
        QLook.QLookStyleAttach(surface.Resources);

        PCardListAttach();
        PNotationAttach();
        PAccentAttach();
        PTranscriptionAttach();
        PGlyphAttach();
        PReflexAttach();
        PAnchorAttach();
        PClipAttach();
        PSpeakerAttach();
        PProspectList.ItemsSource = _pProspectItem;
        QLookItem.QLookItemAttach(PProspectList, PProspectApply);
        PCandidateAttach();
        PSlateAttach();
        PCategoryAttach();
        PMarkerAttach();
        PStackAttach();
        AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PEditorTextObserve));
        PHeadword.TextChanged += PEditorHeadwordObserve;
        PPronunciationField.TextChanged += PContourRefine;
        PPronunciationField.TextChanged += PEditorPronunciationObserve;
        QField.QFieldGhostAttach(PHeadwordGhost, PHeadword);
        QField.QFieldGhostAttach(PPronunciationMeasure, PPronunciationField);
        PNoteContents.TextChanged += PEditorNoteObserve;
        _qRegard = new QRegard(this);
        _qCadence = new QCadence(this);
        PEditorBackward.Click += PEditorUndoObserve;
        PHeadword.SetResourceReference(QField.QFieldHintProperty, "Input.Headword");
        PPronunciationField.SetResourceReference(QField.QFieldHintProperty, "Input.Pronunciation");
        PNoteContents.SetResourceReference(QField.QFieldHintProperty, "Input.NoteHint");
        PEditorBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PEditorForward.Click += PEditorRedoObserve;
        PEditorForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PEditorDiscard.Click += PEditorDiscardObserve;
        PEditorDiscard.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PEditorStore.Click += PEditorStoreObserve;
        PEditorStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PPlaybackAction.Click += PPlaybackActionHandle;
        PPlaybackAction.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
        PEtymologyAttach();
        AddHandler(LostFocusEvent, new RoutedEventHandler(PEditorFocusObserve));
        _pDownloaderPlayer.MediaEnded += PClipEndHandle;
        _pDownloaderPlayer.MediaFailed += PClipEndHandle;
    }

    private TextBlock PHeadwordGhost => (TextBlock)FindName(nameof(PHeadwordGhost));

    private TextBox PHeadword => (TextBox)FindName(nameof(PHeadword));

    private Border PEditorCommand => (Border)FindName(nameof(PEditorCommand));

    private Button PEditorBackward => (Button)FindName(nameof(PEditorBackward));

    private Button PEditorForward => (Button)FindName(nameof(PEditorForward));

    private Button PEditorDiscard => (Button)FindName(nameof(PEditorDiscard));

    private Button PEditorStore => (Button)FindName(nameof(PEditorStore));

    private StackPanel PEditorSound => (StackPanel)FindName(nameof(PEditorSound));

    private Border PPronunciation => (Border)FindName(nameof(PPronunciation));

    private TextBlock PPronunciationOpener => (TextBlock)FindName(nameof(PPronunciationOpener));

    private TextBlock PPronunciationMeasure => (TextBlock)FindName(nameof(PPronunciationMeasure));

    internal TextBox PPronunciationField => (TextBox)FindName(nameof(PPronunciationField));

    private TextBlock PPronunciationCloser => (TextBlock)FindName(nameof(PPronunciationCloser));

    private PContour PContour => (PContour)FindName(nameof(PContour));

    private Border PContents => (Border)FindName(nameof(PContents));

    private TextBox PNoteContents => (TextBox)FindName(nameof(PNoteContents));

    private long PEditorDraft => _qEditor.QEditorArea.CEditorDesk.CDeskId;

    internal void PEditorIntroduce(PWindow host, QEditor driver)
    {
        _pEditorHost = host;
        _qEditor = driver;
        _qRegard.QRegardIntroduce(driver.QEditorArea);
        _qCadence.QCadenceIntroduce(driver.QEditorArea);
        PVolumeAttach();
        driver.QEditorIntroduce(host, this);
    }

    internal void PEditorClose()
    {
        _qEditor.QEditorArea.CEditorClose();
        _pDownloaderPlayer.Close();
    }

    internal event Action? PEditorChronicleChanged;

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_qEditor.QEditorArea.CEditorDesk.CDeskUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_qEditor.QEditorArea.CEditorDesk.CDeskRedo);
    }

    private void PEditorRequestDefer(LRequest request)
    {
        _qEditor.QEditorArea.CEditorDesk.CDeskDefer(request);
    }

    private void PEditorRequestSend(LRequest request)
    {
        _qEditor.QEditorArea.CEditorDesk.CDeskSend(request);
    }

    internal void PEditorStartRefine()
    {
        _pMeaningList.Clear();
        _pCollocationList.Clear();
    }

    internal void PEditorDraftRefine(CEntryDraft draft)
    {
        QField.QFieldTextShow(PHeadword, draft.CEntryDraftHeadword);
        QField.QFieldNoteShow(PNoteContents, draft.CEntryDraftNote);
        PEditorCommand.Visibility = QLook.QLookVisibleRead(_qEditor.QEditorArea.CEditorOwned);
        PReadingRefine();
    }

    internal void PReadingRefine()
    {
        _qCadence.QCadenceReadingRefine(PHeadword.Text);
    }

    internal void PPronunciationRefine(CEntryDraft _)
    {
        QField.QFieldTextShow(PPronunciationField, _qEditor.QEditorArea.CEditorPronunciationRead());
    }

    internal void PTimbreRefine(CEntryDraft _)
    {
        CTimbre timbre = _qEditor.QEditorArea.CEditorTimbre;
        PPronunciationOpener.Text = QLook.QLookFirstRead(timbre.CTimbrePhonemic, "/", "[");
        PPronunciationCloser.Text = QLook.QLookFirstRead(timbre.CTimbrePhonemic, "/", "]");
        PContour.PContourTonal = timbre.CTimbreTonal;
        PPronunciation.Visibility = QLook.QLookVisibleRead(timbre.CTimbreSpoken);
        PAccent.Visibility = QLook.QLookVisibleRead(timbre.CTimbreSpoken);
    }

    internal void PSpeakerRefine(CEntryDraft _)
    {
        PSpeakerName.Text = _qEditor.QEditorArea.CEditorLanguage;
        PSpeakerFlagRefine();
    }

    internal void PHeadwordFontRefine(CEntryDraft _)
    {
        LFontFace.LFontRefine(
            _pEditorHost.PWindowAtelier,
            _qEditor.QEditorArea.CEditorLanguage,
            CFontRole.CFontRoleHeadword,
            PHeadword,
            PHeadwordGhost);
        LFontFace.LFontPlace(PHeadword, PHeadwordGhost);
    }

    internal void PExampleFontRefine(CEntryDraft _)
    {
        LFontFace.LFontExampleRefine(Resources, _pEditorHost.PWindowAtelier, _qEditor.QEditorArea.CEditorLanguage);
    }

    internal void PGlyphFontRefine(CEntryDraft _)
    {
        LFontFace.LFontGlyphRefine(PGlyph.Resources, _pEditorHost.PWindowAtelier, _qEditor.QEditorArea.CEditorLanguage);
    }

    internal void PMeaningRefine(CEntryDraft draft)
    {
        PCardShow(_pMeaningList, "Meaning", draft.CEntryDraftMeanings);
    }

    internal void PCollocationRefine(CEntryDraft draft)
    {
        PCardShow(_pCollocationList, "Collocation", draft.CEntryDraftCollocations);
    }

    internal void PPlaybackRefine(CEntryDraft draft)
    {
        PEditorRecordingShow(draft);
        PPlaybackTrayShow();
    }

    private void PEditorHeadwordObserve(object sender, TextChangedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorHeadwordSet(PHeadword.Text);
    }

    private void PContourRefine(object sender, TextChangedEventArgs e)
    {
        PContour.PContourIpa = PPronunciationField.Text;
    }

    private void PEditorPronunciationObserve(object sender, TextChangedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorPronunciationSet(PPronunciationField.Text);
    }

    private void PEditorNoteObserve(object sender, TextChangedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorNoteSet(PNoteContents.Text);
    }

    private void PEditorTextObserve(object sender, TextChangedEventArgs e)
    {
        switch (e.OriginalSource)
        {
            case TextBox { DataContext: PCard or PSentence or PGloss or PImage or PVideo } box:
                PEditorFieldObserve(box);
                break;
            case TextBox { DataContext: PContextCaret caret } box:
                PContextTextObserve(caret, box.Text);
                break;
            case TextBox { DataContext: PRegisterCaret caret } box:
                PRegisterTextObserve(caret, box.Text);
                break;
            case TextBox { DataContext: PLinkCaret caret } box:
                PLinkTextObserve(caret, box.Text);
                break;
            case TextBox { DataContext: PLabelCaret caret } box:
                PLabelTextObserve(caret, box.Text);
                break;
        }
    }

    private void PEditorFocusObserve(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorDesk.CDeskPersist();
    }

    internal void PEditorStateRefine()
    {
        CDesk desk = _qEditor.QEditorArea.CEditorDesk;
        IsEnabled = desk.CDeskRunning;
        PEditorDiscard.IsEnabled = desk.CDeskChanged;
        PEditorStore.IsEnabled = desk.CDeskStorable;
        (bool undo, bool redo) = desk.CDeskChronicleRead();
        PEditorBackward.IsEnabled = undo;
        PEditorForward.IsEnabled = redo;
        PEditorChronicleChanged?.Invoke();
    }

    private void PEditorUndoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleUndo();
    }

    private void PEditorRedoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleRedo();
    }

    private void PEditorStoreObserve(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorEntrySave();
    }

    private void PEditorDiscardObserve(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorEntryUndo();
    }
}
