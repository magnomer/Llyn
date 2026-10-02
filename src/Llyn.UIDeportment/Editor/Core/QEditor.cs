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

    internal void QEditorIntroduce(QWindow host, CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(editor);

        _cEditor = editor;
        _qRegard.QRegardIntroduce(editor);
        _qEditorSound.QEditorSoundIntroduce(editor, host.QWindowVolume);
        _qEditorCard.QEditorCardIntroduce(host, editor);
        _qCategory.QCategoryIntroduce(editor);
        _qMarker.QMarkerIntroduce(editor, _qCategory);

        CDesk desk = editor.CEditorDesk;
        desk.CDeskStarted += _qEditorCard.QEditorStartRefine;
        desk.CDeskStarted += _qEditorSound.QEditorReadingRefine;
        desk.CDeskStateChanged += QEditorStateRefine;
        editor.CEditorSounding.CSoundingChanged += _qEditorSound.QEditorReadingRefine;

        editor.CEditorDraftChanged += QEditorDraftRefine;
        editor.CEditorDraftChanged += _qEditorSound.QEditorPronunciationRefine;
        editor.CEditorDraftChanged += _qEditorSound.QEditorTimbreRefine;
        _qEditorFont.QEditorFontIntroduce(editor);
        editor.CEditorDraftChanged += _qEditorCard.QEditorCardSentence.QSentenceFrameRefine;
        editor.CEditorDraftChanged += _qEditorCard.QEditorMeaningRefine;
        editor.CEditorDraftChanged += _qEditorCard.QEditorCollocationRefine;
        editor.CEditorDraftChanged += _qEditorCard.QEditorCardSentence.QSentenceMentionRefine;

        editor.CEditorObserverAttach(QObserver.QObserverCreate<Action>(static run => run()));
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
        QChronicle.QChronicleCaretRefine(_cEditor.CEditorDesk.CDeskUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cEditor.CEditorDesk.CDeskRedo);
    }

    private void QEditorDraftRefine(CEntryDraft draft)
    {
        QField.QFieldTextShow(QEditorHeadword, draft.CEntryDraftHeadword);
        if (!CEditor.CEditorNoteCheck(QEditorNote.Text, draft.CEntryDraftNote))
        {
            QField.QFieldTextShow(QEditorNote, draft.CEntryDraftNote);
        }

        QEditorCommand.Visibility = QLook.QLookVisibleRead(_cEditor.CEditorOwned);
        _qEditorSound.QEditorReadingRefine();
    }

    private void QEditorHeadwordObserve(object sender, TextChangedEventArgs e)
    {
        _cEditor.CEditorHeadwordSet(QEditorHeadword.Text);
    }

    private void QEditorNoteObserve(object sender, TextChangedEventArgs e)
    {
        _cEditor.CEditorNoteSet(QEditorNote.Text);
    }

    private void QEditorFocusObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorDesk.CDeskPersist();
    }

    private void QEditorStateRefine()
    {
        CDesk desk = _cEditor.CEditorDesk;
        _qEditorSurface.IsEnabled = desk.CDeskRunning;
        QEditorDiscard.IsEnabled = desk.CDeskChanged;
        QEditorStore.IsEnabled = desk.CDeskStorable;
        (bool undo, bool redo) = desk.CDeskChronicleRead();
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
