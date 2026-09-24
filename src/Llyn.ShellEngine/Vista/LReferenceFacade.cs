using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LReferenceFacade
{
    private readonly LEngine _lReferenceFacadeEngine;
    private readonly object _lReferenceFacadeGate;

    public LReferenceFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lReferenceFacadeEngine = engine;
        _lReferenceFacadeGate = engine.LEngineGate;
    }

    internal LReference LEngineReferenceCreate(LReference reference)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkCreate(reference);
        }
    }

    public LReference LEngineCitationCreate(string title)
    {
        LReference stored;
        lock (_lReferenceFacadeGate)
        {
            stored = LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkCreate(title);
        }

        _lReferenceFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored;
    }

    public long LEngineCitationResolve(long draftId, long cardId, long sentenceId, string title)
    {
        LReference stored;
        lock (_lReferenceFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(draftId);
            _lReferenceFacadeEngine.LEngineDraft.LEngineDraftValidate(draftId);

            long held = _lReferenceFacadeEngine.LEngineDraft.LEngineDraftLoad(draftId)
                .LDraftExampleRead(cardId, sentenceId)?.LExampleDraftReference.LStateAnchorShow() ?? 0;
            long? found = LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkResolve(title, held);
            if (found is long id)
            {
                return id;
            }

            stored = LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkCreate(title);
        }

        _lReferenceFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored.LReferenceId;
    }

    internal LReference? LEngineReferenceRead(long id)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkRead(id);
        }
    }

    internal IReadOnlyList<LReference> LEngineReferenceRead()
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkRead();
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        return LEngineReferenceRead(LEngineReferenceFind(vista.LVistaQuery, vista.LVistaOrder), vista.LVistaChosen);
    }

    internal IReadOnlyList<LCatalogReference> LEngineReferenceRead(
        IReadOnlyList<LCatalogReference> found, long? chosen)
    {
        List<LCatalogReference> rows = new(found.Count);
        string[] names = LVistaFacade.LEngineTwinRead(
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
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkRead(ownerId, owner);
        }
    }

    internal void LEngineReferenceUpdate(LReference reference)
    {
        lock (_lReferenceFacadeGate)
        {
            LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkUpdate(reference);
        }
    }

    internal void LEngineReferenceAttach(long ownerId, long referenceId, int position, LOwner owner)
    {
        lock (_lReferenceFacadeGate)
        {
            LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkAttach(ownerId, referenceId, owner);
        }
    }

    internal void LEngineReferenceDetach(long ownerId, long referenceId, LOwner owner)
    {
        lock (_lReferenceFacadeGate)
        {
            LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkDetach(ownerId, owner);
        }
    }

    internal void LEngineReferenceDelete(long id)
    {
        lock (_lReferenceFacadeGate)
        {
            LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkDelete(id);
        }

        _lReferenceFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    internal void LEngineReferenceDelete(long id, bool detach)
    {
        lock (_lReferenceFacadeGate)
        {
            LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkDelete(id, detach);
        }

        _lReferenceFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    public IReadOnlyDictionary<long, string> LEngineCitationRead()
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LCitationRead();
        }
    }

    internal LDraft LEngineReferenceStart(string origin, long? referenceId)
    {
        lock (_lReferenceFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LReferenceFacadeStaff.LEngineStaffCitation.LReferenceStart(origin, referenceId);
        }
    }

    internal LReference LEngineReferenceCommit(long id)
    {
        LReference settled;
        lock (_lReferenceFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lReferenceFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            settled = LReferenceFacadeStaff.LEngineStaffCitation.LReferenceCommit(id);
        }

        _lReferenceFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectReference, settled.LReferenceId);
        return settled;
    }
    private LEngineStaff LReferenceFacadeStaff => _lReferenceFacadeEngine.LEngineStaffHeld;
}
