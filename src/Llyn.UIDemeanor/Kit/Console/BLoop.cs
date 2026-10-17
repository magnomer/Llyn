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
    private Action _bLoopBatch = static () => { };

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
            BLoopWorkRun(work);
            if (_bLoopQueue.Count == 0)
            {
                BLoopWorkRun(_bLoopBatch);
            }
        }
    }

    private void BLoopWorkRun(Action work)
    {
        try
        {
            work();
        }
        catch (Exception exception)
        {
            _bLoopFault(exception);
        }
    }

    internal void BLoopBatchAttach(Action observe)
    {
        ArgumentNullException.ThrowIfNull(observe);

        _bLoopBatch = observe;
    }

    public void BLoopDefer(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);

        _bLoopQueue.Add(work);
    }

    internal void BLoopKeyAttach(BConsole console, Action<ConsoleKeyInfo> observe)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(observe);

        Thread reader = new(() =>
        {
            try
            {
                while (true)
                {
                    ConsoleKeyInfo key = console.BConsoleKeyRead();
                    BLoopDefer(() => observe(key));
                }
            }
            catch (Exception exception)
            {
                ExceptionDispatchInfo failure = ExceptionDispatchInfo.Capture(exception);
                BLoopDefer(failure.Throw);
            }
        })
        {
            IsBackground = true,
        };
        reader.Start();
    }

    internal void BLoopSizeAttach(BConsole console, Action<int, int> observe)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(observe);

        Thread ticker = new(() =>
        {
            using SemaphoreSlim ran = new(0);
            while (true)
            {
                Thread.Sleep(100);
                BLoopDefer(() =>
                {
                    try
                    {
                        (int width, int height) = console.BConsoleSizeRead();
                        observe(width, height);
                    }
                    finally
                    {
                        ran.Release();
                    }
                });
                ran.Wait();
            }
        })
        {
            IsBackground = true,
        };
        ticker.Start();
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
