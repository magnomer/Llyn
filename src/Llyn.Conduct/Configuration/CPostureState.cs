namespace Llyn.Conduct;

public sealed record CPostureState(
    CWindowState? CPostureStateWindow,
    bool CPostureStateLinked,
    bool CPostureStateSplit,
    double CPostureStateVolume);
