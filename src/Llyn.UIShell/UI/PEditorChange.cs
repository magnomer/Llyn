using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PEditor
{
    private bool _pEditorFill;

    private PRespelling _pEditorRespelling = PRespelling.PRespellingPlain;

    internal Action<bool>? PEditorChangeNotice;

    internal bool PEditorChangeCheck()
    {
        if (_pEditorTenure is not LTenure held)
        {
            return false;
        }

        held.LTenurePersist();
        return held.LTenureStateRead().LTenureStateChanged;
    }

    private void PCardChangeHandle(PCard card, string field, LStateWritten written)
    {
        switch (field)
        {
            case nameof(PCard.PTitle):
                PEditorRequestDefer(new LRequestCardTitle(PEditorDraft, card.PCardId, written));
                break;
            case nameof(PCard.PCardExpression):
                PEditorRequestDefer(new LRequestCardExpression(PEditorDraft, card.PCardId, written));
                break;
            case nameof(PCard.PCardDefinition):
                PEditorRequestDefer(new LRequestCardMeaning(PEditorDraft, card.PCardId, written));
                break;
        }
    }

    internal static string PEditorFieldRead(TextBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        BindingBase? bound = box.TemplatedParent is ComboBox choice
            ? BindingOperations.GetBindingBase(choice, ComboBox.TextProperty)
            : BindingOperations.GetBindingBase(box, TextBox.TextProperty);

        return bound switch
        {
            Binding single => single.Path.Path,
            MultiBinding { Bindings: [Binding first, ..] } => first.Path.Path,
            _ => string.Empty,
        };
    }

    private void PEditorTextHandle(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is not TextBox box || box.DataContext is PAccentItem or PTranscriptionItem)
        {
            return;
        }

        if (box.DataContext is PCard or PSentence or PGloss or PImage or PVideo)
        {
            PEditorFieldHandle(box);
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PHeadword))
        {
            PEditorRequestDefer(new LRequestHeadword(PEditorDraft, PHeadword.Text ?? string.Empty));
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PPronunciationField))
        {
            string text = PPronunciationField.Text ?? string.Empty;
            PEditorRequestDefer(
                _pEditorRespelling.PRespellingShown
                    ? new LRequestRespelling(PEditorDraft, text)
                    : new LRequestIpa(PEditorDraft, text));
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PNoteContents))
        {
            PEditorRequestDefer(new LRequestNote(PEditorDraft, PEditorNoteRead()));
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PMarkerField))
        {
            PEditorRequestDefer(new LRequestSpeech(PEditorDraft, PMarkerRead()));
        }
    }

    private void PEditorFieldHandle(TextBox box)
    {
        UIElement owner = box.TemplatedParent as ComboBox ?? (UIElement)box;
        if (!owner.IsKeyboardFocusWithin)
        {
            return;
        }

        string field = PEditorFieldRead(box);
        LStateWritten written = new(box.Text);
        switch (box.DataContext)
        {
            case PCard card:
                PCardChangeHandle(card, field, written);
                break;
            case PSentence row when PCardSentenceFind(row) is PCard card:
                PSentenceChangeHandle(card, row, field, box);
                break;
            case PGloss gloss:
                PGlossChangeHandle(gloss, written);
                break;
            case PImage row:
                PEditorRequestDefer(new LRequestImageLocation(PEditorDraft, row.PImageId, written));
                break;
            case PVideo row when field == nameof(PVideo.PVideoLocation):
                PEditorRequestDefer(new LRequestVideoLocation(PEditorDraft, row.PVideoId, written));
                break;
            case PVideo row:
                PEditorRequestDefer(new LRequestVideoSpan(PEditorDraft, row.PVideoId, written));
                break;
        }
    }

    private void PEditorFocusHandle(object sender, RoutedEventArgs e)
    {
        if (_pEditorFill)
        {
            return;
        }

        PEditorChangeSave();
    }

    private void PEditorChangeSave()
    {
        if (_pEditorFill || _pEditorTenure is not LTenure held)
        {
            return;
        }

        held.LTenurePersist();
    }

    private void PEditorChangeUpdate()
    {
        LTenureState? state = _pEditorTenure?.LTenureStateRead();
        bool changed = state is { LTenureStateChanged: true };
        bool storable = changed && state?.LTenureStateRefusal is null;
        PEditorDiscard.IsEnabled = changed;
        PEditorStore.IsEnabled = storable;
        PEditorChangeNotice?.Invoke(storable);
        if (state is not null)
        {
            PEditorHoldShow(!state.LTenureStateHalted);
        }

        PChronicleUpdate();
    }
}
