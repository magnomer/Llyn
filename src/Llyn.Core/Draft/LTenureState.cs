namespace Llyn.Core;

public sealed record LTenureState(
    bool LTenureStateChanged,
    string? LTenureStateRefusal,
    bool LTenureStateBackward,
    bool LTenureStateForward,
    bool LTenureStateHalted);
