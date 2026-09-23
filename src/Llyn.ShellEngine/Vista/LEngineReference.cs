using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LReference LEngineReferenceCreate(LReference reference)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffReference.LReferenceClerkCreate(reference);
        }
    }

    public LReference LEngineCitationCreate(string title)
    {
        LReference stored;
        lock (LEngineGate)
        {
            stored = _lEngineStaff.LEngineStaffReference.LReferenceClerkCreate(title);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored;
    }

    internal LReference? LEngineReferenceRead(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffReference.LReferenceClerkRead(id);
        }
    }

    internal IReadOnlyList<LReference> LEngineReferenceRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffReference.LReferenceClerkRead();
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffReference.LReferenceClerkFind(query, order);
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
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffReference.LReferenceClerkRead(ownerId, owner);
        }
    }

    internal void LEngineReferenceUpdate(LReference reference)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffReference.LReferenceClerkUpdate(reference);
        }
    }

    internal void LEngineReferenceAttach(long ownerId, long referenceId, int position, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffReference.LReferenceClerkAttach(ownerId, referenceId, owner);
        }
    }

    internal void LEngineReferenceDetach(long ownerId, long referenceId, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffReference.LReferenceClerkDetach(ownerId, owner);
        }
    }

    internal void LEngineReferenceDelete(long id)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffReference.LReferenceClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    internal void LEngineReferenceDelete(long id, bool detach)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffReference.LReferenceClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    public IReadOnlyDictionary<long, string> LEngineCitationRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffReference.LCitationRead();
        }
    }

    internal LDraft LEngineReferenceStart(string origin, long? referenceId)
    {
        lock (LEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineStaff.LEngineStaffCitation.LReferenceStart(origin, referenceId);
        }
    }

    internal LReference LEngineReferenceCommit(long id)
    {
        LReference settled;
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineStaff.LEngineStaffCitation.LReferenceCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, settled.LReferenceId);
        return settled;
    }
}
