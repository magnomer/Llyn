using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const int PEditorChangeDelay = 250;

    private CancellationTokenSource? _pEditorPending;

    private bool _pEditorFill;

    internal Action<bool>? PEditorChangeNotice;

    internal bool PEditorChangeCheck()
    {
        if (_pEditorPending is not null)
        {
            PEditorChangeSave();
        }

        return PEditorDraftCheck();
    }

    private void PEditorChangeAttach(PCard card)
    {
        card.PropertyChanged += PCardChangeHandle;
    }

    private void PCardChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not PCard card)
        {
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(PCard.PTitle):
                PEditorRequestDefer(
                    PEditorRequestFormat(card, nameof(PCard.PTitle)),
                    new LRequestCardTitle(_pEditorDraft, card.PCardId, card.PCardTitleRead()));
                break;
            case nameof(PCard.PCardExpression):
                PEditorRequestDefer(
                    PEditorRequestFormat(card, nameof(PCard.PCardExpression)),
                    new LRequestCardExpression(_pEditorDraft, card.PCardId, card.PCardExpressionRead()));
                break;
            case nameof(PCard.PCardDefinition):
                PEditorRequestDefer(
                    PEditorRequestFormat(card, nameof(PCard.PCardDefinition)),
                    new LRequestCardMeaning(_pEditorDraft, card.PCardId, card.PCardDefinitionRead()));
                break;
        }
    }

    private void PEditorTextHandle(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: PCard or PAccentItem })
        {
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PHeadword))
        {
            PEditorRequestDefer(
                PEditorRequestHeadword,
                new LRequestHeadword(_pEditorDraft, PHeadword.Text ?? string.Empty));
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PPronunciationField))
        {
            PEditorRequestDefer(
                PEditorRequestIpa,
                new LRequestIpa(_pEditorDraft, PPronunciationField.Text ?? string.Empty));
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PNoteContents))
        {
            PEditorRequestDefer(
                PEditorRequestNote,
                new LRequestNote(_pEditorDraft, PEditorNoteRead()));
            return;
        }

        if (ReferenceEquals(e.OriginalSource, PMarkerField))
        {
            PEditorRequestDefer(
                PEditorRequestSpeech,
                new LRequestSpeech(_pEditorDraft, PMarkerRead()));
            return;
        }

        PEditorChangeDefer();
    }

    private void PEditorFocusHandle(object sender, RoutedEventArgs e)
    {
        if (_pEditorFill || _pEditorRequestPending.Count == 0)
        {
            return;
        }

        PEditorChangeSave();
    }

    private void PEditorChangeDefer()
    {
        if (_pEditorFill || _pEditorHalted || _pEditorDraft == 0)
        {
            return;
        }

        PEditorChangeStop();

        CancellationTokenSource pending = new();
        _pEditorPending = pending;

        _ = PEditorChangeRun(pending.Token);
    }

    private async Task PEditorChangeRun(CancellationToken token)
    {
        try
        {
            await Task.Delay(PEditorChangeDelay, token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        try
        {
            PEditorChangeSave();
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
        }
    }

    private void PEditorChangeSave()
    {
        if (_pEditorFill)
        {
            return;
        }

        PEditorChangeStop();

        if (_pEditorHalted || _pEditorDraft == 0)
        {
            return;
        }

        PEditorRequestPersist();
        PEditorChangeUpdate();
    }

    private void PEditorChangeStop()
    {
        CancellationTokenSource? pending = _pEditorPending;
        _pEditorPending = null;

        if (pending is null)
        {
            return;
        }

        pending.Cancel();
        pending.Dispose();
    }

    private void PEditorChangeUpdate()
    {
        bool changed = PEditorDraftCheck();
        PEditorDiscard.IsEnabled = changed;
        PEditorStore.IsEnabled = changed;
        PEditorChangeNotice?.Invoke(changed);
    }
}
