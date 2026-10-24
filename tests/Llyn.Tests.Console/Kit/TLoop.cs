using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TLoop
{
    [Fact]
    public void LoopRun_ThreeQueuedItems_ObservesOneBatch()
    {
        BLoop loop = new(static _ => { });
        TaskCompletionSource done = new();
        int batches = 0;
        loop.BLoopBatchAttach(() =>
        {
            batches++;
            done.TrySetResult();
        });
        loop.BLoopDefer(static () => { });
        loop.BLoopDefer(static () => { });
        loop.BLoopDefer(static () => { });

        loop.BLoopRun(done.Task);

        Assert.Equal(1, batches);
    }

    [Fact]
    public void LoopRun_RunFault_CallsRunFault()
    {
        List<string> faults = [];
        TaskCompletionSource done = new();
        BLoop loop = new(exception => faults.Add($"loop {exception.Message}"));
        loop.BLoopDefer(static () => throw new InvalidOperationException("broken"));
        loop.BLoopDefer(done.SetResult);

        loop.BLoopRun(done.Task, exception => faults.Add($"run {exception.Message}"));

        Assert.Equal(["run broken"], faults);
    }

    [Fact]
    public void LoopKeyAttach_TypedBatch_ReportsOneEdit()
    {
        TConsoleFake console = new() { TConsoleFakeBatch = true };
        foreach (char letter in "ab\rc가😀")
        {
            ConsoleKey key = letter == '\r' ? ConsoleKey.Enter : ConsoleKey.NoName;
            console.TConsoleFakeKeys.Add(new ConsoleKeyInfo(letter, key, false, false, false));
        }

        console.BConsoleFramePrint(string.Empty);
        List<string> edits = [];
        List<string> entered = [];
        BField field = new(edits.Add);
        BLoop loop = new(static _ => { });
        TaskCompletionSource done = new();
        loop.BLoopBatchAttach(() =>
        {
            if (field.BFieldText.Length == 6)
            {
                done.TrySetResult();
            }
        });
        loop.BLoopKeyAttach(console, key =>
        {
            if (key.Key == ConsoleKey.Enter)
            {
                entered.Add(field.BFieldText);
            }
            else
            {
                field.BSParleyBodyObserve(key);
            }
        });

        SynchronizationContext? outer = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            loop.BLoopRun(done.Task.WaitAsync(TimeSpan.FromSeconds(10)));
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(outer);
        }

        Assert.Equal(["ab"], entered);
        Assert.Equal(["ab", "abc가\U0001F600"], edits);
    }
}
