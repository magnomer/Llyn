using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const int PEditorChangeDelay = 250;

    private CancellationTokenSource? _pEditorPending;

    private bool _pEditorFill;

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
        card.PropertyChanged += (_, _) => PEditorChangeDefer();
        card.PCardSentence.CollectionChanged += PEditorChangeHandle;
        card.PCardContext.CollectionChanged += PEditorChangeHandle;
        card.PCardLabel.CollectionChanged += PEditorChangeHandle;
        card.PCardImage.CollectionChanged += PEditorChangeHandle;
        card.PCardVideo.CollectionChanged += PEditorChangeHandle;
        card.PCardLink.CollectionChanged += PLinkChipChange;
    }

    private void PEditorChangeHandle(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PEditorChangeDefer();
    }

    private void PEditorTextHandle(object sender, TextChangedEventArgs e)
    {
        PEditorChangeDefer();
    }

    private void PEditorChangeDefer()
    {
        if (_pEditorFill || _pEditorHalted || _pEditorDraft.Length == 0)
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
        PEditorChangeStop();

        if (_pEditorFill || _pEditorHalted || _pEditorDraft.Length == 0)
        {
            return;
        }

        PEditorDraftSave();
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
    }
}
