using System.Runtime.CompilerServices;

namespace Llyn.Performance;

internal abstract class LDrill
{
    public abstract void LDrillPrepare();

    public abstract void LDrillCycleRun();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void LDrillRun(int repeat)
    {
        for (int cycle = 0; cycle < repeat; cycle++)
        {
            LDrillCycleRun();
        }
    }
}
