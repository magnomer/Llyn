using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LExample LEngineExampleCreate(LExample example)
    {
        lock (_lEngineGate)
        {
            return _lEngineExampleClerk.LExampleClerkCreate(example);
        }
    }

    internal LExample? LEngineExampleRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineExampleClerk.LExampleClerkRead(id);
        }
    }

    internal IReadOnlyList<LExample> LEngineExampleRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineExampleClerk.LExampleClerkRead();
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineExampleClerk.LExampleClerkFind(query, order);
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
        lock (_lEngineGate)
        {
            return _lEngineExampleClerk.LExampleClerkRead(ownerId, owner);
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long meaningId)
    {
        lock (_lEngineGate)
        {
            return _lEngineExampleClerk.LSentenceRead(meaningId);
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineExampleClerk.LSentenceRead(ownerId, owner);
        }
    }

    internal void LEngineExampleUpdate(LExample example)
    {
        lock (_lEngineGate)
        {
            _lEngineExampleClerk.LExampleClerkUpdate(example);
        }
    }

    internal void LEngineExampleUpdate(long exampleId, LStateAnchor reference)
    {
        lock (_lEngineGate)
        {
            _lEngineExampleClerk.LExampleClerkUpdate(exampleId, reference);
        }
    }

    internal void LEngineExampleAttach(long ownerId, long exampleId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineExampleClerk.LExampleClerkAttach(ownerId, exampleId, position, owner);
        }
    }

    internal void LEngineExampleDetach(long ownerId, long exampleId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineExampleClerk.LExampleClerkDetach(ownerId, exampleId, owner);
        }
    }

    internal void LEngineExampleRemove(long ownerId, long exampleId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineCardClerk.LExampleRemove(ownerId, exampleId, owner);
        }
    }

    internal void LEngineExampleDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineExampleClerk.LExampleClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal void LEngineExampleDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            _lEngineExampleClerk.LExampleClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal LDraft LEngineExampleStart(string origin, long? exampleId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineCitationClerk.LExampleStart(origin, exampleId);
        }
    }

    internal LExample LEngineExampleCommit(long id)
    {
        LExample settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineCitationClerk.LExampleCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, settled.LExampleId);
        return settled;
    }
}
