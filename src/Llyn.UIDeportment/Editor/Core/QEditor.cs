using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEditor : QChronicleHost
{
    private readonly FrameworkElement _qEditorSurface;

    private readonly QEditorCard _qEditorCard;

    private readonly QMarker _qMarker;

    private readonly QRegard _qRegard;

    private readonly QEditorSound _qEditorSound;

    private readonly QEditorFont _qEditorFont;

    private readonly QCategory _qCategory;

    private readonly QUnit _qUnit;

    private CEditor _cEditor = null!;

    internal QEditor(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qEditorSurface = surface;
        QLook.QLookStyleAttach(surface);
        QChronicle.QChronicleIntroduce(surface, this);
        _qEditorCard = new QEditorCard(surface);
        _qMarker = new QMarker(surface);
        new QStack(surface);
        QEditorHeadword.TextChanged += QEditorHeadwordObserve;
        QField.QFieldGhostAttach(QEditorHeadwordGhost, QEditorHeadword);
        QEditorNote.TextChanged += QEditorNoteObserve;
        _qRegard = new QRegard(surface);
        _qEditorSound = new QEditorSound(surface);
        _qEditorFont = new QEditorFont(surface);
        _qCategory = new QCategory(surface, _qMarker);
        _qUnit = new QUnit(surface);
        QEditorBackward.Click += QEditorUndoObserve;
        QEditorHeadword.SetResourceReference(QField.QFieldHintProperty, "Input.Headword");
        QEditorNote.SetResourceReference(QField.QFieldHintProperty, "Input.NoteHint");
        QEditorBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QEditorForward.Click += QEditorRedoObserve;
        QEditorForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QEditorDiscard.Click += QEditorDiscardObserve;
        QEditorDiscard.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QEditorStore.Click += QEditorStoreObserve;
        QEditorStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        surface.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(QEditorFocusObserve));
    }

    private TextBlock QEditorHeadwordGhost => QContract.QContractFind<TextBlock>(_qEditorSurface, "PHeadwordGhost");

    private TextBox QEditorHeadword => QContract.QContractFind<TextBox>(_qEditorSurface, "PHeadword");

    private Border QEditorCommand => QContract.QContractFind<Border>(_qEditorSurface, "PEditorCommand");

    private Button QEditorBackward => QContract.QContractFind<Button>(_qEditorSurface, "PEditorBackward");

    private Button QEditorForward => QContract.QContractFind<Button>(_qEditorSurface, "PEditorForward");

    private Button QEditorDiscard => QContract.QContractFind<Button>(_qEditorSurface, "PEditorDiscard");

    private Button QEditorStore => QContract.QContractFind<Button>(_qEditorSurface, "PEditorStore");

    private TextBox QEditorNote => QContract.QContractFind<TextBox>(_qEditorSurface, "PNoteContents");

    internal QProspect QEditorProspect => _qEditorCard.QEditorCardProspect;

    internal event Action? QEditorChronicleChanged;

    internal void QEditorIntroduce(
        CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu, CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(editor);

        _cEditor = editor;
        _qRegard.QRegardIntroduce(editor.CEditorDesk, editor.CEditorEsteem);
        _qEditorSound.QEditorSoundIntroduce(editor, volume, atelier.CAtelierLedger, envoy);
        _qEditorCard.QEditorCardIntroduce(
            atelier,
            envoy,
            mentionMenu,
            editor.CEditorEntry,
            editor.CEditorCard,
            editor.CEditorSentence,
            editor.CEditorList,
            editor.CEditorField,
            editor.CEditorImage,
            editor.CEditorVideo);
        _qCategory.QCategoryIntroduce(editor.CEditorSpeech);
        _qUnit.QUnitIntroduce(editor.CEditorEntry);
        _qMarker.QMarkerIntroduce(editor.CEditorSpeech, editor.CEditorEntry, _qCategory, _qUnit);

        CDesk desk = editor.CEditorDesk;
        desk.CDeskStarted += _qEditorCard.QEditorStartRefine;
        desk.CDeskStarted += _qEditorSound.QEditorReadingRefine;
        desk.CDeskStateChanged += QEditorStateRefine;
        editor.CEditorSounding.CSoundingChanged += _qEditorSound.QEditorReadingRefine;

        editor.CEditorEntry.CEntryDraftChanged += QEditorDraftRefine;
        editor.CEditorEntry.CEntryDraftChanged += _qEditorSound.QEditorPronunciationRefine;
        editor.CEditorEntry.CEntryDraftChanged += _qEditorSound.QEditorTimbreRefine;
        _qEditorFont.QEditorFontIntroduce(editor.CEditorEntry, editor.CEditorTimbre);
        editor.CEditorEntry.CEntryDraftChanged += _qEditorCard.QEditorCardSentence.QSentenceFrameRefine;
        editor.CEditorEntry.CEntryDraftChanged += _qEditorCard.QEditorMeaningRefine;
        editor.CEditorEntry.CEntryDraftChanged += _qEditorCard.QEditorCollocationRefine;
        editor.CEditorEntry.CEntryDraftChanged += _qEditorCard.QEditorCardSentence.QSentenceMentionRefine;
    }

    internal void QEditorVisibleRefine(Visibility visible)
    {
        _qEditorSurface.Visibility = visible;
    }

    internal void QEditorExitRefine()
    {
        _cEditor.CEditorClose();
        QEditorPlayerRefine();
    }

    internal void QEditorPlayerRefine()
    {
        _qEditorSound.QEditorPlayerRefine();
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cEditor.CEditorDesk.CDeskChronicle.CDeskChronicleUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRedo);
    }

    private void QEditorDraftRefine(CEntryDraft draft)
    {
        QField.QFieldTextShow(QEditorHeadword, draft.CEntryDraftHeadword);
        if (!CEntry.CEntryNoteCheck(QEditorNote.Text, draft.CEntryDraftNote))
        {
            QField.QFieldTextShow(QEditorNote, draft.CEntryDraftNote);
        }

        QEditorCommand.Visibility = QLook.QLookVisibleRead(_cEditor.CEditorOwned);
        _qEditorSound.QEditorReadingRefine();
    }

    private void QEditorHeadwordObserve(object sender, TextChangedEventArgs e)
    {
        _cEditor.CEditorEntry.CEntryHeadwordSet(QEditorHeadword.Text);
    }

    private void QEditorNoteObserve(object sender, TextChangedEventArgs e)
    {
        _cEditor.CEditorEntry.CEntryNoteSet(QEditorNote.Text);
    }

    private void QEditorFocusObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorDesk.CDeskDraft.CDeskDraftPersist();
    }

    private void QEditorStateRefine()
    {
        CDesk desk = _cEditor.CEditorDesk;
        _qEditorSurface.IsEnabled = desk.CDeskChronicle.CDeskChronicleRunning;
        QEditorDiscard.IsEnabled = desk.CDeskDraft.CDeskDraftAltered;
        QEditorStore.IsEnabled = desk.CDeskDraft.CDeskDraftStorable;
        (bool undo, bool redo) = desk.CDeskChronicle.CDeskChronicleRead();
        QEditorBackward.IsEnabled = undo;
        QEditorForward.IsEnabled = redo;
        QEditorChronicleChanged?.Invoke();
    }

    private void QEditorUndoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleUndoObserve();
    }

    private void QEditorRedoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleRedoObserve();
    }

    private void QEditorStoreObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorEntrySave();
    }

    private void QEditorDiscardObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorEntryUndo();
    }
}
