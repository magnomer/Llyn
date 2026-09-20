using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPostureFake : LPostureVault
{
    private readonly Dictionary<string, LPostureState> _tPostureFakeKept = new(StringComparer.Ordinal);

    public LPostureState? LPostureRead(string name) =>
        _tPostureFakeKept.TryGetValue(name, out LPostureState? state) ? state : null;

    public void LPostureSave(string name, LPostureState state)
    {
        _tPostureFakeKept[name] = state;
    }
}
