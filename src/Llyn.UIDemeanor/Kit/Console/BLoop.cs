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
    private Action _bLoopBatch = static () => { };

    public BLoop(Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(fault);

        _bLoopFault = fault;
    }

    public void BLoopRun(Task until, Action<Exception>? fault = null)
    {
        ArgumentNullException.ThrowIfNull(until);

        Action<Exception> observe = fault ?? _bLoopFault;
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

        while (!until.IsCompleted)
        {
            Action work = _bLoopQueue.Take();
            BLoopWorkRun(work, observe);
            if (_bLoopQueue.Count == 0)
            {
                BLoopWorkRun(_bLoopBatch, observe);
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

        bool settling = false;
        Action<List<ConsoleKeyInfo>, bool>? apply = null;
        apply = (keys, settle) =>
        {
            if (settling)
            {
                BLoopDefer(() => apply!(keys, settle));
                return;
            }

            if (settle)
            {
                TaskCompletionSource marker = new();
                BLoopDefer(marker.SetResult);
                settling = true;
                try
                {
                    BLoopRun(marker.Task);
                }
                finally
                {
                    settling = false;
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
                        BLoopDefer(() => apply!(rest, false));
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
                        (List<ConsoleKeyInfo> keys, bool settle) = (runs[run], settles[run]);
                        BLoopDefer(() => apply(keys, settle));
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
