using System;
using System.Collections.Generic;

namespace Llyn.Tests;

internal sealed class TFaultStage : IDisposable
{
    private readonly Stack<IDisposable> _tFaultStageHeld = new();

    internal TFaultStage(string member, bool thrown)
    {
        TFaultStageMember = member;
        TFaultStageThrown = thrown;
    }

    internal string TFaultStageMember { get; }

    internal bool TFaultStageThrown { get; }

    internal List<string> TFaultStageHeard { get; } = [];

    internal TFaultKind TFaultStageAdd<TFaultKind>(TFaultKind held) where TFaultKind : IDisposable
    {
        _tFaultStageHeld.Push(held);
        return held;
    }

    public void Dispose()
    {
        while (_tFaultStageHeld.TryPop(out IDisposable? held))
        {
            held.Dispose();
        }
    }
}
