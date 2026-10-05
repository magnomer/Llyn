using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using Xunit;

namespace Llyn.Tests;

internal static class TWindow
{
    internal static void TWindowRun(Action run)
    {
        ArgumentNullException.ThrowIfNull(run);

        ExceptionDispatchInfo? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                run();
            }
            catch (Exception caught)
            {
                failure = ExceptionDispatchInfo.Capture(caught);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "The STA run did not finish in time.");
        failure?.Throw();
    }
}
