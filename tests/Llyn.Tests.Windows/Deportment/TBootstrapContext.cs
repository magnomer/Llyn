using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TBootstrapContext
{
    [Fact]
    public void BootstrapContext_ResumesAnUnfinishedAwaitOnTheStartupThread()
    {
        int startup = 0;
        int resumed = 0;
        bool bare = false;
        Exception? failure = null;

        Thread thread = new(() =>
        {
            try
            {
                startup = Environment.CurrentManagedThreadId;
                bare = SynchronizationContext.Current is null;
                QBootstrap.QBootstrapContextIntroduce();

                DispatcherFrame frame = new();
                Task awaited = TBootstrapResume();
                awaited.ContinueWith(_ => frame.Continue = false, TaskScheduler.Default);
                Dispatcher.PushFrame(frame);
                awaited.GetAwaiter().GetResult();
            }
            catch (Exception caught)
            {
                failure = caught;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)));

        Assert.Null(failure);
        Assert.True(bare);
        Assert.Equal(startup, resumed);

        async Task TBootstrapResume()
        {
            await Task.Delay(50);
            resumed = Environment.CurrentManagedThreadId;
        }
    }
}
