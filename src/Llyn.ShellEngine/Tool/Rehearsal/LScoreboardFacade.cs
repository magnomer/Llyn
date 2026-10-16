using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LScoreboardFacade
{
    private readonly LEngineHearth _lScoreboardFacadeHearth;
    private readonly object _lScoreboardFacadeGate;

    public LScoreboardFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lScoreboardFacadeHearth = hearth;
        _lScoreboardFacadeGate = hearth.LEngineGate;
    }

    private LWorkspaceStaff LScoreboardFacadeStaff =>
        _lScoreboardFacadeHearth.LEngineStaffHeld.LEngineStaffWorkspace;

    public LDeckCensus LEngineCensusRead(long? deckId)
    {
        lock (_lScoreboardFacadeGate)
        {
            return LScoreboardFacadeStaff.LWorkspaceStaffCue.LCueCensusRead(deckId, TimeZoneInfo.Local);
        }
    }
}
