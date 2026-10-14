using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LDeckFacade
{
    private readonly LEngineHearth _lDeckFacadeHearth;
    private readonly object _lDeckFacadeGate;

    public LDeckFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lDeckFacadeHearth = hearth;
        _lDeckFacadeGate = hearth.LEngineGate;
    }

    private LWorkspaceStaff LDeckFacadeStaff => _lDeckFacadeHearth.LEngineStaffHeld.LEngineStaffWorkspace;

    public (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed) LEngineDeckRead()
    {
        lock (_lDeckFacadeGate)
        {
            LDeckClerk decks = LDeckFacadeStaff.LWorkspaceStaffDeck;
            return (decks.LDeckFolderRead(), decks.LDeckClerkRead());
        }
    }

    public (LDeck LDeckStored, IReadOnlyList<LBearing> LDeckBearings)? LEngineDeckRead(long deckId)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckClerkRead(deckId);
        }
    }

    public long LEngineDeckCreate(string name, long? folderId, LGrain grain)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckClerkCreate(name, folderId, grain).LDeckId;
        }
    }

    public void LEngineDeckUpdate(long deckId, string name)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckNameSet(deckId, name);
        }
    }

    public void LEngineDeckUpdate(LDeck deck, IReadOnlyList<LBearing> bearings)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckClerkUpdate(deck, bearings);
        }
    }

    public void LEngineDeckMove(long deckId, long? folderId)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckClerkMove(deckId, folderId);
        }
    }

    public void LEngineDeckMove(long? folderId, IReadOnlyList<long> order)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckOrderSet(folderId, order);
        }
    }

    public void LEngineDeckDelete(long deckId)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckClerkDelete(deckId);
        }
    }

    public long LEngineFolderCreate(string name)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckFolderCreate(name).LDeckFolderId;
        }
    }

    public void LEngineFolderUpdate(long folderId, string name)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckFolderUpdate(folderId, name);
        }
    }

    public void LEngineFolderDelete(long folderId)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffDeck.LDeckFolderDelete(folderId);
        }
    }

    public IReadOnlyList<LCue> LEngineCueRead(long deckId)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffCue.LCueClerkRead(deckId);
        }
    }

    public int LEngineCueAdd(long deckId, IReadOnlyList<long> entryIds)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffCue.LCueClerkAdd(deckId, entryIds).Count;
        }
    }

    public long LEngineCueCreate(long deckId, IReadOnlyList<LCueCell> cells)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffCue.LCueClerkCreate(deckId, cells).LCueId;
        }
    }

    public void LEngineCueRemove(IReadOnlyList<long> cueIds)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffCue.LCueClerkRemove(cueIds);
        }
    }

    public void LEngineCueMove(long deckId, long cueId, int position)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffCue.LCueClerkMove(deckId, cueId, position);
        }
    }

    public void LEngineCueSet(long cueId, int flag, bool excluded)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffCue.LCueClerkSet(cueId, flag, excluded);
        }
    }

    public LCueFace LEngineFaceRead(long cueId, LPortraitLabel label)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffStencil.LStencilClerkResolve(cueId, label);
        }
    }

    public Task<LDeckReceipt> LEngineDeckExport(long deckId, LPortraitLabel label, CancellationToken cancellation)
    {
        return LDeckFacadeStaff.LWorkspaceStaffDepot.LDepotClerkExport(deckId, label, cancellation);
    }

    public bool LEngineDepotCheck()
    {
        return LDeckFacadeStaff.LWorkspaceStaffLatch.LDepotLatchCheck();
    }

    public IReadOnlyList<LCueCell> LEngineCellRead(long cueId)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffCue.LCueCellRead(cueId);
        }
    }

    public void LEngineCellSave(long cueId, IReadOnlyList<LCueCell> cells)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffCue.LCueCellSave(cueId, cells);
        }
    }

    public LStencil LEngineStencilRead(long deckId)
    {
        lock (_lDeckFacadeGate)
        {
            return LDeckFacadeStaff.LWorkspaceStaffStencil.LStencilClerkRead(deckId);
        }
    }

    public void LEngineSlotSet(long deckId, LStencilSlot slot)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffStencil.LStencilSlotSet(deckId, slot);
        }
    }

    public void LEngineStencilSave(long deckId, string name)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffStencil.LStencilClerkSave(deckId, name);
        }
    }

    public void LEngineStencilApply(long deckId, long stencilId)
    {
        lock (_lDeckFacadeGate)
        {
            LDeckFacadeStaff.LWorkspaceStaffStencil.LStencilClerkApply(deckId, stencilId);
        }
    }
}
