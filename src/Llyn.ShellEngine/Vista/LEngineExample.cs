using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LExample LEngineExampleCreate(LExample example)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffExample.LExampleClerkCreate(example);
        }
    }

    internal LExample? LEngineExampleRead(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffExample.LExampleClerkRead(id);
        }
    }

    internal IReadOnlyList<LExample> LEngineExampleRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffExample.LExampleClerkRead();
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffExample.LExampleClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "")
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogExample> found = LEngineExampleFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogExample> rows = new(found.Count);
        string[] names = LEngineTwinRead(
            found,
            row => LEngineNameRead(row.LCatalogExampleStored.LExampleText, unknown, unwritten),
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
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffExample.LExampleClerkRead(ownerId, owner);
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long meaningId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffExample.LSentenceRead(meaningId);
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffExample.LSentenceRead(ownerId, owner);
        }
    }

    internal void LEngineExampleUpdate(LExample example)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffExample.LExampleClerkUpdate(example);
        }
    }

    internal void LEngineExampleUpdate(long exampleId, LStateAnchor reference)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffExample.LExampleClerkUpdate(exampleId, reference);
        }
    }

    internal void LEngineExampleAttach(long ownerId, long exampleId, int position, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffExample.LExampleClerkAttach(ownerId, exampleId, position, owner);
        }
    }

    internal void LEngineExampleDetach(long ownerId, long exampleId, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffExample.LExampleClerkDetach(ownerId, exampleId, owner);
        }
    }

    internal void LEngineExampleRemove(long ownerId, long exampleId, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffCard.LExampleRemove(ownerId, exampleId, owner);
        }
    }

    internal void LEngineExampleDelete(long id)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffExample.LExampleClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal void LEngineExampleDelete(long id, bool detach)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffExample.LExampleClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal LDraft LEngineExampleStart(string origin, long? exampleId)
    {
        lock (LEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineStaff.LEngineStaffCitation.LExampleStart(origin, exampleId);
        }
    }

    internal LExample LEngineExampleCommit(long id)
    {
        LExample settled;
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineStaff.LEngineStaffCitation.LExampleCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, settled.LExampleId);
        return settled;
    }
}
