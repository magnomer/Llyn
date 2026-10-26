using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.UIDemeanor;

public sealed class BLoop : SynchronizationContext
{
    private readonly BlockingCollection<Action> _bLoopQueue = new();
    private readonly Action<Exception> _bLoopFault;
    private readonly List<(Task, Action<Exception>)> _bLoopRuns = [];
    private Action _bLoopBatch = static () => { };

    public BLoop(Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(fault);

        _bLoopFault = fault;
    }

    internal List<BField> BLoopDeferred { get; } = [];

    public void BLoopRun(Task until, Action<Exception>? fault = null)
    {
        ArgumentNullException.ThrowIfNull(until);

        Action<Exception> observe = fault ?? _bLoopFault;
        _bLoopRuns.Add((until, observe));
        try
        {
            BLoopRun(until, observe, _bLoopRuns.Count - 1);
        }
        finally
        {
            _bLoopRuns.RemoveAt(_bLoopRuns.Count - 1);
        }
    }

    private void BLoopRun(Task until, Action<Exception> observe, int enclosing)
    {
        until.ContinueWith(
            static (_, loop) => ((BLoop)loop!).BLoopDefer(static () => { }),
            this,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

        if (_bLoopQueue.Count == 0)
        {
            BLoopWorkRun(_bLoopBatch, observe);
        }

        while (!until.IsCompleted && _bLoopRuns.FindIndex(0, enclosing, static run => run.Item1.IsCompleted) < 0)
        {
            Action work = _bLoopQueue.Take();
            BLoopWorkRun(work, observe);
            if (_bLoopQueue.Count == 0)
            {
                BLoopWorkRun(_bLoopBatch, observe);
            }
        }

        if (until.Exception is AggregateException failures)
        {
            foreach (Exception failure in failures.Flatten().InnerExceptions)
            {
                observe(failure);
            }
        }
    }

    private static void BLoopWorkRun(Action work, Action<Exception> fault)
    {
        try
        {
            work();
        }
        catch (Exception exception)
        {
            fault(exception);
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

        Task settling = Task.CompletedTask;
        ConcurrentQueue<(List<ConsoleKeyInfo>, bool)> waiting = new();
        List<ConsoleKeyInfo>? remainder = null;
        Action? apply = null;
        apply = () =>
        {
            if (!settling.IsCompleted)
            {
                BLoopDefer(apply!);
                return;
            }

            List<ConsoleKeyInfo>? keys = remainder;
            bool settle = false;
            remainder = null;
            if (keys is null)
            {
                if (!waiting.TryDequeue(out (List<ConsoleKeyInfo>, bool) next))
                {
                    return;
                }

                (keys, settle) = next;
            }

            if (settle)
            {
                TaskCompletionSource marker = new();
                BLoopDefer(marker.SetResult);
                settling = marker.Task;
                BLoopRun(marker.Task, _bLoopRuns[^1].Item2, _bLoopRuns.Count);
                if (!marker.Task.IsCompleted)
                {
                    remainder = keys;
                    BLoopDefer(apply!);
                    return;
                }
            }

            for (int index = 0; index < keys.Count; index++)
            {
                try
                {
                    observe(keys[index]);
                }
                catch (Exception)
                {
                    List<ConsoleKeyInfo> rest = keys[(index + 1)..];
                    if (rest.Count > 0)
                    {
                        remainder = rest;
                        BLoopDefer(apply!);
                    }

                    throw;
                }
            }
        };

        Thread reader = new(() =>
        {
            try
            {
                bool typed = false;
                while (true)
                {
                    List<List<ConsoleKeyInfo>> runs = [];
                    List<bool> settles = [];
                    do
                    {
                        ConsoleKeyInfo key = console.BConsoleKeyRead();
                        bool plain = !char.IsControl(key.KeyChar)
                            && !key.Modifiers.HasFlag(ConsoleModifiers.Alt)
                            && !key.Modifiers.HasFlag(ConsoleModifiers.Control);
                        if (plain && typed && runs.Count > 0)
                        {
                            runs[^1].Add(key);
                        }
                        else
                        {
                            runs.Add([key]);
                            settles.Add(!plain && typed);
                        }

                        typed = plain;
                    }
                    while (console.BConsoleKeyCheck());

                    for (int run = 0; run < runs.Count; run++)
                    {
                        waiting.Enqueue((runs[run], settles[run]));
                        BLoopDefer(apply);
                    }
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
