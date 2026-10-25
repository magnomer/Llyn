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

    [Fact]
    public void LoopKeyAttach_KeysArriveWhileSettling_KeepsOrder()
    {
        TConsoleFake console = new();
        console.BConsoleFramePrint(string.Empty);
        ConsoleKeyInfo pause = new('\0', ConsoleKey.F12, false, false, false);
        Action<char> feed = letter =>
        {
            console.TConsoleFakeKeys.Add(new ConsoleKeyInfo(letter, ConsoleKey.NoName, false, false, false));
            console.TConsoleFakeKeys.Add(pause);
            Assert.True(SpinWait.SpinUntil(() => console.TConsoleFakeKeys.Count == 0, TimeSpan.FromSeconds(10)));
        };
        List<char> seen = [];
        BLoop loop = new(static _ => { });
        TaskCompletionSource done = new();
        loop.BLoopKeyAttach(console, key =>
        {
            if (key.Key != ConsoleKey.F12)
            {
                seen.Add(key.KeyChar);
            }

            if (seen.Count == 4)
            {
                done.TrySetResult();
            }
        });

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
        console.TConsoleFakeKeys.Add(pause);
        Assert.True(SpinWait.SpinUntil(() => console.TConsoleFakeKeys.Count == 0, TimeSpan.FromSeconds(10)));
        loop.BLoopDefer(() => feed('d'));
        feed('c');
        loop.BLoopRun(done.Task.WaitAsync(TimeSpan.FromSeconds(10)));

        Assert.Equal(['a', '\t', 'c', 'd'], seen);
    }

    [Fact]
    public void LoopKeyAttach_NestedLoop_ReceivesQueuedKeys()
    {
        TConsoleFake console = new() { TConsoleFakeBatch = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F3, false, false, false));
        console.BConsoleFramePrint(string.Empty);
        List<string> seen = [];
        bool nested = false;
        BLoop loop = new(static _ => { });
        TaskCompletionSource answered = new();
        TaskCompletionSource done = new();
        loop.BLoopKeyAttach(console, key =>
        {
            if (key.Key == ConsoleKey.F2)
            {
                nested = true;
                loop.BLoopRun(answered.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                nested = false;
                seen.Add("F2 closed");
                done.TrySetResult();
                return;
            }

            seen.Add($"{key.Key} nested {nested}");
            if (key.Key == ConsoleKey.F3)
            {
                answered.TrySetResult();
            }
        });

        loop.BLoopRun(done.Task.WaitAsync(TimeSpan.FromSeconds(20)));

        Assert.Equal(["X nested True", "F3 nested True", "F2 closed"], seen);
    }

    [Fact]
    public void LoopKeyAttach_SettleOpensLoop_LoopReceivesKeys()
    {
        TConsoleFake console = new() { TConsoleFakeBatch = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.X, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F3, false, false, false));
        console.BConsoleFramePrint(string.Empty);
        List<string> seen = [];
        bool nested = false;
        BLoop loop = new(static _ => { });
        TaskCompletionSource answered = new();
        TaskCompletionSource done = new();
        loop.BLoopKeyAttach(console, key =>
        {
            if (key.Key == ConsoleKey.A)
            {
                loop.BLoopDefer(() =>
                {
                    nested = true;
                    loop.BLoopRun(answered.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                    nested = false;
                    seen.Add("modal closed");
                });
                return;
            }

            seen.Add($"{key.Key} nested {nested}");
            if (key.Key == ConsoleKey.F3)
            {
                answered.TrySetResult();
            }
            else if (key.Key == ConsoleKey.F2)
            {
                done.TrySetResult();
            }
        });

        loop.BLoopRun(done.Task.WaitAsync(TimeSpan.FromSeconds(20)));

        Assert.Equal(["X nested True", "F3 nested True", "modal closed", "F2 nested False"], seen);
    }

    [Fact]
    public void LoopKeyAttach_SettleFault_CallsRunFault()
    {
        TConsoleFake console = new() { TConsoleFakeBatch = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F3, false, false, false));
        console.BConsoleFramePrint(string.Empty);
        List<string> seen = [];
        List<string> faults = [];
        BLoop loop = new(exception => faults.Add($"loop {exception.Message}"));
        TaskCompletionSource done = new();
        loop.BLoopKeyAttach(console, key =>
        {
            if (key.Key == ConsoleKey.A)
            {
                loop.BLoopDefer(static () => throw new InvalidOperationException("broken"));
                return;
            }

            seen.Add($"{key.Key}");
        });

        loop.BLoopRun(
            done.Task.WaitAsync(TimeSpan.FromSeconds(10)),
            exception =>
            {
                faults.Add(
                    $"{exception.Message} {exception.StackTrace?.Contains(nameof(TLoop), StringComparison.Ordinal)}");
                done.TrySetResult();
            });

        Assert.Equal(["broken True"], faults);
        Assert.Equal(["F2", "F3"], seen);
    }
}
