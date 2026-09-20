using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LReference LEngineReferenceCreate(LReference reference)
    {
        lock (_lEngineGate)
        {
            return _lEngineReferenceClerk.LReferenceClerkCreate(reference);
        }
    }

    public LReference LEngineCitationCreate(string title)
    {
        LReference stored;
        lock (_lEngineGate)
        {
            stored = _lEngineReferenceClerk.LReferenceClerkCreate(title);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored;
    }

    internal LReference? LEngineReferenceRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineReferenceClerk.LReferenceClerkRead(id);
        }
    }

    internal IReadOnlyList<LReference> LEngineReferenceRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineReferenceClerk.LReferenceClerkRead();
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineReferenceClerk.LReferenceClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        return LEngineReferenceRead(LEngineReferenceFind(vista.LVistaQuery, vista.LVistaOrder), vista.LVistaChosen);
    }

    private static IReadOnlyList<LCatalogReference> LEngineReferenceRead(
        IReadOnlyList<LCatalogReference> found, long? chosen)
    {
        List<LCatalogReference> rows = new(found.Count);
        string[] names = LEngineTwinRead(
            found, row => row.LCatalogReferenceName, row => row.LCatalogReferenceStored.LReferenceId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogReference row = found[index];
            rows.Add(row with
            {
                LCatalogReferenceName = names[index],
                LCatalogReferenceChosen = row.LCatalogReferenceStored.LReferenceId == chosen,
            });
        }

        return rows;
    }

    internal IReadOnlyList<LReference> LEngineReferenceRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineReferenceClerk.LReferenceClerkRead(ownerId, owner);
        }
    }

    internal void LEngineReferenceUpdate(LReference reference)
    {
        lock (_lEngineGate)
        {
            _lEngineReferenceClerk.LReferenceClerkUpdate(reference);
        }
    }

    internal void LEngineReferenceAttach(long ownerId, long referenceId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineReferenceClerk.LReferenceClerkAttach(ownerId, referenceId, owner);
        }
    }

    internal void LEngineReferenceDetach(long ownerId, long referenceId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineReferenceClerk.LReferenceClerkDetach(ownerId, owner);
        }
    }

    internal void LEngineReferenceDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineReferenceClerk.LReferenceClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    internal void LEngineReferenceDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            _lEngineReferenceClerk.LReferenceClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    public IReadOnlyDictionary<long, string> LEngineCitationRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineReferenceClerk.LCitationRead();
        }
    }

    internal LDraft LEngineReferenceStart(string origin, long? referenceId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineCitationClerk.LReferenceStart(origin, referenceId);
        }
    }

    internal LReference LEngineReferenceCommit(long id)
    {
        LReference settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineCitationClerk.LReferenceCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, settled.LReferenceId);
        return settled;
    }
}
