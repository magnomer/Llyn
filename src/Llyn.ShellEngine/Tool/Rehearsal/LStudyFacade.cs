using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LStudyFacade
{
    private readonly LEngineHearth _lStudyFacadeHearth;
    private readonly object _lStudyFacadeGate;

    public LStudyFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lStudyFacadeHearth = hearth;
        _lStudyFacadeGate = hearth.LEngineGate;
    }

    private LWorkspaceStaff LStudyFacadeStaff => _lStudyFacadeHearth.LEngineStaffHeld.LEngineStaffWorkspace;

    public (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed,
        IReadOnlyDictionary<long, int> LDeckActive) LEngineActiveRead()
    {
        lock (_lStudyFacadeGate)
        {
            LDeckClerk decks = LStudyFacadeStaff.LWorkspaceStaffDeck;
            return (decks.LDeckFolderRead(), decks.LDeckClerkRead(),
                LStudyFacadeStaff.LWorkspaceStaffCue.LCueActiveRead());
        }
    }

    public IReadOnlyList<(LCue LCueStored, LCueSide LCueFront, LCueSide LCueBack)> LEngineFaceRead(
        long deckId, LPortraitLabel label)
    {
        lock (_lStudyFacadeGate)
        {
            return
            [
                .. LStudyFacadeStaff.LWorkspaceStaffStencil.LStencilFaceRead(deckId, label)
                    .Select(static face => (face.LCueStored, face.LCueFront, face.LCueBack)),
            ];
        }
    }
}
