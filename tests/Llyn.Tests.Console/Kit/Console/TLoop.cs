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

        loop.BLoopRun(done.Task.WaitAsync(TimeSpan.FromSeconds(10)));

        Assert.True(done.Task.IsCompleted);
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

        loop.BLoopRun(
            done.Task.WaitAsync(TimeSpan.FromSeconds(10)), exception => faults.Add($"run {exception.Message}"));

        Assert.True(done.Task.IsCompleted);
        Assert.Equal(["run broken"], faults);
    }

    [Fact]
    public void LoopRun_UntilFaults_CallsRunFault()
    {
        List<string> faults = [];
        BLoop loop = new(exception => faults.Add($"loop {exception.Message}"));

        loop.BLoopRun(
            Task.WhenAll(
                Task.FromException(new InvalidOperationException("broken")),
                Task.FromException(new TimeoutException("late"))),
            exception => faults.Add($"run {exception.Message}"));

        Assert.Equal(["run broken", "run late"], faults);
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

    [Fact]
    public void LoopKeyAttach_TypedRunThenKeys_KeepsOrder()
    {
        TConsoleFake console = new() { TConsoleFakeBatch = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('b', ConsoleKey.B, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('c', ConsoleKey.C, false, false, false));
        console.BConsoleFramePrint(string.Empty);
        List<string> edits = [];
        List<string> seen = [];
        BField field = new(edits.Add);
        BLoop loop = new(static _ => { });
        TaskCompletionSource done = new();
        loop.BLoopBatchAttach(() =>
        {
            if (field.BFieldText == "abc")
            {
                done.TrySetResult();
            }
        });
        loop.BLoopKeyAttach(console, key =>
        {
            if (char.IsControl(key.KeyChar))
            {
                seen.Add($"{key.Key}:{field.BFieldText}");
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

        Assert.Equal(["Enter:ab", "Tab:ab"], seen);
        Assert.Equal(["ab", "abc"], edits);
    }
}
