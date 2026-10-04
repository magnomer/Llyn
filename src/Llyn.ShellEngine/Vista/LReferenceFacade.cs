using System;
using System.Collections.Generic;
using Llyn.Application;
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

    public long LEngineCitationResolve(long draftId, long cardId, long sentenceId, string title)
    {
        LReference stored;
        lock (_lReferenceFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(draftId);
            _lReferenceFacadeEngine.LEngineDraft.LEngineDraftValidate(draftId);

            long? found = LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkResolve(
                title, _lReferenceFacadeEngine.LEngineDraft.LEngineDraftLoad(draftId), cardId, sentenceId);
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

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkFind(query, order);
        }
    }

    public LReferenceOffer LEngineReferenceFind(LTenure held, long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LReferenceCitationFind(text, draft, card, sentence);
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

    internal void LEngineReferenceDelete(long id, bool detach)
    {
        lock (_lReferenceFacadeGate)
        {
            LReferenceFacadeStaff.LEngineStaffReference.LReferenceClerkDelete(id, detach);
        }

        _lReferenceFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    public LColophon LEngineColophonRead(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        lock (_lReferenceFacadeGate)
        {
            string tally = LReferenceFacadeStaff.LEngineStaffUsage.LUsageTallyRead(
                draft.LDraftReference?.LReferenceId, LOwner.LOwnerReference);
            return LReferenceClerk.LReferenceColophonRead(draft, tally);
        }
    }

    public static LImprint LEngineImprintRead(LDraft? draft)
    {
        return LReferenceClerk.LReferenceImprintRead(draft);
    }

    public IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown)
    {
        ArgumentNullException.ThrowIfNull(shown);

        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LCitationRead(shown);
        }
    }

    public string LEngineCitationRead(LDraft? draft)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffReference.LCitationRead(draft);
        }
    }

    internal LDraft LEngineReferenceStart(string origin, long? referenceId)
    {
        lock (_lReferenceFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LReferenceFacadeStaff.LEngineStaffCitation.LCitationClerkReference
                .LReferenceCitationStart(origin, referenceId);
        }
    }

    internal LReference LEngineReferenceCommit(long id)
    {
        LReference settled;
        lock (_lReferenceFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lReferenceFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            settled = LReferenceFacadeStaff.LEngineStaffCitation.LCitationClerkReference.LReferenceCitationCommit(id);
        }

        _lReferenceFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectReference, settled.LReferenceId);
        return settled;
    }
    private LEngineStaff LReferenceFacadeStaff => _lReferenceFacadeEngine.LEngineStaffHeld;
}
