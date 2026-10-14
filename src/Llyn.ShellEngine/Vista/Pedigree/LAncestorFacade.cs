using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LAncestorFacade : LAncestorPort
{
    private readonly LEngineHearth _lAncestorFacadeHearth;
    private readonly object _lAncestorFacadeGate;
    private LEngineStaff LAncestorFacadeStaff => _lAncestorFacadeHearth.LEngineStaffHeld;

    internal LAncestorFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lAncestorFacadeHearth = hearth;
        _lAncestorFacadeGate = hearth.LEngineGate;
    }

    public IReadOnlyList<LLineage> LEngineLineageRead(long entryId, IReadOnlyList<LAncestorDraft> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        lock (_lAncestorFacadeGate)
        {
            return LAncestorFacadeStaff.LEngineStaffEntry.LEntryStaffLineage.LLineageClerkRead(entryId, map);
        }
    }

    public IReadOnlyList<LAncestorRelation> LEngineRelationRead()
    {
        lock (_lAncestorFacadeGate)
        {
            return LAncestorFacadeStaff.LEngineStaffEntry.LEntryStaffAncestor.LAncestorRelationRead();
        }
    }

    public LAncestorRelation LEngineRelationCreate(string name)
    {
        lock (_lAncestorFacadeGate)
        {
            return LAncestorFacadeStaff.LEngineStaffEntry.LEntryStaffAncestor.LAncestorRelationCreate(name);
        }
    }

    public int? LEngineRelationDelete(long relationId)
    {
        lock (_lAncestorFacadeGate)
        {
            return LAncestorFacadeStaff.LEngineStaffEntry.LEntryStaffAncestor.LAncestorRelationDelete(relationId);
        }
    }

    public IReadOnlyList<LAncestorLanguage> LEngineLanguageRead()
    {
        lock (_lAncestorFacadeGate)
        {
            return LAncestorFacadeStaff.LEngineStaffEntry.LEntryStaffAncestor.LAncestorLanguageRead();
        }
    }

    public LAncestorLanguage LEngineTongueCreate(string name)
    {
        lock (_lAncestorFacadeGate)
        {
            return LAncestorFacadeStaff.LEngineStaffEntry.LEntryStaffAncestor.LAncestorTongueCreate(name);
        }
    }

    public int? LEngineTongueDelete(long tongueId)
    {
        lock (_lAncestorFacadeGate)
        {
            return LAncestorFacadeStaff.LEngineStaffEntry.LEntryStaffAncestor.LAncestorTongueDelete(tongueId);
        }
    }
}
