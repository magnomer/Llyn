using System.Threading;

namespace Llyn.Application;

public sealed class LDepotLatch
{
    private int _lDepotLatchBusy;

    public bool LDepotLatchStart()
    {
        return Interlocked.CompareExchange(ref _lDepotLatchBusy, 1, 0) == 0;
    }

    public void LDepotLatchFinish()
    {
        Volatile.Write(ref _lDepotLatchBusy, 0);
    }

    public bool LDepotLatchCheck()
    {
        return Volatile.Read(ref _lDepotLatchBusy) != 0;
    }
}
