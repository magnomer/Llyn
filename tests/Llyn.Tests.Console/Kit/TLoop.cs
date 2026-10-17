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
}
