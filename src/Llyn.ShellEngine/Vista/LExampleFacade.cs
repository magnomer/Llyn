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

    internal LExample? LEngineExampleRead(long id)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffExample.LExampleClerkRead(id);
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
