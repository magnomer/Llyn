namespace Llyn.Conduct;

public sealed record CCourierState(
    bool CCourierStateBusy,
    bool CCourierStateAllowed,
    bool CCourierStateAttached,
    string CCourierStateLine);
