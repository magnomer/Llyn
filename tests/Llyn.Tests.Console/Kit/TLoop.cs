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
}
