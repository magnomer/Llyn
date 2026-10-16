using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPhonographMedia
{
    [Fact]
    public void PhonographPlay_MissingFile_RaisesFinishedOnContext()
    {
        int constructing = 0;
        int finished = 0;
        Exception? failure = null;
        string missing = Path.Combine(Path.GetTempPath(), $"llyn-missing-{Guid.NewGuid():N}.mp3");

        Thread thread = new(() =>
        {
            try
            {
                constructing = Environment.CurrentManagedThreadId;
                LPhonograph phonograph = TInterfacePhonograph.TPhonographCreate();
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                DispatcherFrame frame = new();
                DispatcherTimer guard = new() { Interval = TimeSpan.FromSeconds(10) };
                guard.Tick += (_, _) => frame.Continue = false;

                phonograph.LPhonographFinished += () =>
                {
                    finished = Environment.CurrentManagedThreadId;
                    frame.Continue = false;
                };
                TInterfacePhonograph.TPhonographPlay(phonograph, missing);

                guard.Start();
                Dispatcher.PushFrame(frame);
                guard.Stop();
            }
            catch (Exception caught)
            {
                failure = caught;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        Assert.Null(failure);
        Assert.NotEqual(0, finished);
        Assert.Equal(constructing, finished);
    }
}
