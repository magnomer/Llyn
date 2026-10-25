using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TLoopNested
{
    [Fact]
    public void LoopRun_OuterEndsInnerHung_EndsInner()
    {
        List<string> faults = [];
        TConsoleFake.TConsoleFakeRun(
            () =>
            {
                BLoop loop = new(static _ => { });
                TaskCompletionSource never = new();
                loop.BLoopDefer(() =>
                {
                    loop.BLoopRun(never.Task);
                    faults.Add("inner ended");
                });

                loop.BLoopRun(
                    never.Task.WaitAsync(TimeSpan.FromMilliseconds(200)),
                    exception => faults.Add(exception.GetType().Name));
            },
            10);

        Assert.Equal(["inner ended", nameof(TimeoutException)], faults);
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
    public void LoopKeyAttach_RunEndsWhileSettling_KeepsKeys()
    {
        TConsoleFake console = new() { TConsoleFakeBatch = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));
        console.BConsoleFramePrint(string.Empty);
        List<string> seen = [];
        BLoop loop = new(static _ => { });
        TaskCompletionSource done = new();
        TaskCompletionSource answered = new();
        loop.BLoopKeyAttach(console, key =>
        {
            seen.Add($"{key.Key}");
            loop.BLoopDefer(key.Key == ConsoleKey.A ? done.SetResult : answered.SetResult);
        });

        loop.BLoopRun(done.Task.WaitAsync(TimeSpan.FromSeconds(10)));
        seen.Add("ended");
        loop.BLoopRun(answered.Task.WaitAsync(TimeSpan.FromSeconds(10)));

        Assert.Equal(["A", "ended", "F2"], seen);
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
            if (key.Key == ConsoleKey.F3)
            {
                done.TrySetResult();
            }
        });

        loop.BLoopRun(
            done.Task.WaitAsync(TimeSpan.FromSeconds(10)),
            exception => faults.Add(
                $"{exception.Message} {exception.StackTrace?.Contains(nameof(TLoop), StringComparison.Ordinal)}"));

        Assert.Equal(["broken True"], faults);
        Assert.Equal(["F2", "F3"], seen);
    }

    [Fact]
    public void LoopKeyAttach_SettleFaultInDialog_CallsDialogFault()
    {
        TConsoleFake console = new() { TConsoleFakeBatch = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F3, false, false, false));
        console.BConsoleFramePrint(string.Empty);
        List<string> faults = [];
        BLoop loop = new(exception => faults.Add($"loop {exception.Message}"));
        TaskCompletionSource answered = new();
        TaskCompletionSource done = new();
        loop.BLoopKeyAttach(console, key =>
        {
            if (key.Key == ConsoleKey.F2)
            {
                loop.BLoopRun(
                    answered.Task.WaitAsync(TimeSpan.FromSeconds(10)),
                    exception => faults.Add($"dialog {exception.Message}"));
                done.TrySetResult();
            }
            else if (key.Key == ConsoleKey.A)
            {
                loop.BLoopDefer(static () => throw new InvalidOperationException("broken"));
                loop.BLoopDefer(() =>
                {
                    TaskCompletionSource closed = new();
                    loop.BLoopDefer(closed.SetResult);
                    loop.BLoopRun(
                        closed.Task.WaitAsync(TimeSpan.FromSeconds(10)),
                        exception => faults.Add($"later {exception.Message}"));
                });
            }
            else
            {
                answered.TrySetResult();
            }
        });

        loop.BLoopRun(
            done.Task.WaitAsync(TimeSpan.FromSeconds(20)), exception => faults.Add($"outer {exception.Message}"));

        Assert.Equal(["dialog broken"], faults);
    }
}
