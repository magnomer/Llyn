namespace Llyn.Conduct;

public sealed record CSittingState(
    string CSittingStatePosition,
    CSittingSide CSittingStateFront,
    CSittingSide CSittingStateBack,
    bool CSittingStateTurned,
    bool CSittingStateEnd,
    bool CSittingStateShuffled,
    string? CSittingStateBlank);
