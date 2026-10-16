using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.UIDemeanor;

public sealed class BLoop : SynchronizationContext
{
    private readonly BlockingCollection<Action> _bLoopQueue = new();
    private readonly Action<Exception> _bLoopFault;

    public BLoop(Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(fault);

        _bLoopFault = fault;
    }

    public void BLoopRun(Task until)
    {
        ArgumentNullException.ThrowIfNull(until);

        until.ContinueWith(
            static (_, loop) => ((BLoop)loop!).BLoopDefer(static () => { }),
            this,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

        while (!until.IsCompleted)
        {
            Action work = _bLoopQueue.Take();
            try
            {
                work();
            }
            catch (Exception exception)
            {
                _bLoopFault(exception);
            }
        }
    }

    public void BLoopDefer(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);

        _bLoopQueue.Add(work);
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);

        BLoopDefer(() => d(state));
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);

        if (Current == this)
        {
            d(state);
            return;
        }

        ExceptionDispatchInfo? failure = null;
        using ManualResetEventSlim done = new();
        BLoopDefer(() =>
        {
            try
            {
                d(state);
            }
            catch (Exception exception)
            {
                failure = ExceptionDispatchInfo.Capture(exception);
            }
            finally
            {
                done.Set();
            }
        });
        done.Wait();
        failure?.Throw();
    }

    public override SynchronizationContext CreateCopy()
    {
        return this;
    }
}
