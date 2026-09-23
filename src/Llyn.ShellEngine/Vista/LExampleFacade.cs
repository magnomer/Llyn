using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LExampleFacade
{
    private readonly LEngine _lExampleFacadeEngine;
    private readonly object _lExampleFacadeGate;

    public LExampleFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lExampleFacadeEngine = engine;
        _lExampleFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LExampleFacadeStaff => _lExampleFacadeEngine.LEngineStaffHeld;

    internal LExample LEngineExampleCreate(LExample example)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LExampleClerkCreate(example);
        }
    }

    internal LExample? LEngineExampleRead(long id)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LExampleClerkRead(id);
        }
    }

    internal IReadOnlyList<LExample> LEngineExampleRead()
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LExampleClerkRead();
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LExampleClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "")
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogExample> found = LEngineExampleFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogExample> rows = new(found.Count);
        string[] names = LVistaFacade.LEngineTwinRead(
            found,
            row => LVistaFacade.LEngineNameRead(row.LCatalogExampleStored.LExampleText, unknown, unwritten),
            row => row.LCatalogExampleStored.LExampleId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogExample row = found[index];
            rows.Add(row with
            {
                LCatalogExampleName = names[index],
                LCatalogExampleChosen = row.LCatalogExampleStored.LExampleId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    internal IReadOnlyList<LExample> LEngineExampleRead(long ownerId, LOwner owner)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LExampleClerkRead(ownerId, owner);
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long meaningId)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LSentenceRead(meaningId);
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long ownerId, LOwner owner)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LSentenceRead(ownerId, owner);
        }
    }

    internal void LEngineExampleUpdate(LExample example)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffExample.LExampleClerkUpdate(example);
        }
    }

    internal void LEngineExampleUpdate(long exampleId, LStateAnchor reference)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffExample.LExampleClerkUpdate(exampleId, reference);
        }
    }

    internal void LEngineExampleAttach(long ownerId, long exampleId, int position, LOwner owner)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffExample.LExampleClerkAttach(ownerId, exampleId, position, owner);
        }
    }

    internal void LEngineExampleDetach(long ownerId, long exampleId, LOwner owner)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffExample.LExampleClerkDetach(ownerId, exampleId, owner);
        }
    }

    internal void LEngineExampleRemove(long ownerId, long exampleId, LOwner owner)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffCard.LExampleRemove(ownerId, exampleId, owner);
        }
    }

    internal void LEngineExampleDelete(long id)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffExample.LExampleClerkDelete(id);
        }

        _lExampleFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal void LEngineExampleDelete(long id, bool detach)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffExample.LExampleClerkDelete(id, detach);
        }

        _lExampleFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal LDraft LEngineExampleStart(string origin, long? exampleId)
    {
        lock (_lExampleFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LExampleFacadeStaff.LEngineStaffCitation.LExampleStart(origin, exampleId);
        }
    }

    internal LExample LEngineExampleCommit(long id)
    {
        LExample settled;
        lock (_lExampleFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lExampleFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            settled = LExampleFacadeStaff.LEngineStaffCitation.LExampleCommit(id);
        }

        _lExampleFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectExample, settled.LExampleId);
        return settled;
    }
}
