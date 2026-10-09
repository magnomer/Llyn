using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LReferenceFacade : LReferencePort
{
    private readonly LEngineHearth _lReferenceFacadeHearth;
    private readonly LDraftFacade _lReferenceFacadeDraft;
    private readonly object _lReferenceFacadeGate;

    internal LReferenceFacade(LEngineHearth hearth, LDraftFacade draft)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(draft);
        _lReferenceFacadeHearth = hearth;
        _lReferenceFacadeDraft = draft;
        _lReferenceFacadeGate = _lReferenceFacadeHearth.LEngineGate;
    }

    public long LEngineCitationResolve(long draftId, long cardId, long sentenceId, string title)
    {
        LReference stored;
        lock (_lReferenceFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(draftId);
            _lReferenceFacadeDraft.LEngineDraftValidate(draftId);

            long? found = LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference.LReferenceClerkResolve(
                title, _lReferenceFacadeDraft.LEngineDraftLoad(draftId), cardId, sentenceId);
            if (found is long id)
            {
                return id;
            }

            stored = LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference.LReferenceClerkCreate(title);
        }

        _lReferenceFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored.LReferenceId;
    }

    internal LReference? LEngineReferenceRead(long id)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference.LReferenceClerkRead(id);
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference.LReferenceClerkFind(query, order);
        }
    }

    public LReferenceOffer LEngineReferenceFind(LTenure held, long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference
                .LReferenceCitationFind(text, draft, card, sentence);
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind()
    {
        return LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);
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
        string[] names = LEntryClerkTwin.LTwinRead(
            found,
            row => row.LCatalogReferenceName,
            static _ => string.Empty,
            row => row.LCatalogReferenceStored.LReferenceId);
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
            LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference.LReferenceClerkDelete(id, detach);
        }

        _lReferenceFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    public LColophon LEngineColophonRead(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        lock (_lReferenceFacadeGate)
        {
            string tally = LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffUsage.LUsageTallyRead(
                draft.LDraftReference?.LReferenceId, LOwner.LOwnerReference);
            return LReferenceClerk.LReferenceColophonRead(draft, tally);
        }
    }

    public LImprint LEngineImprintRead(LDraft? draft)
    {
        return LReferenceClerk.LReferenceImprintRead(draft);
    }

    public IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> LEngineKindRead()
    {
        return LReferenceClerk.LReferenceMenuRead();
    }

    public IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown)
    {
        ArgumentNullException.ThrowIfNull(shown);

        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference.LCitationRead(shown);
        }
    }

    public string LEngineCitationRead(LDraft? draft)
    {
        lock (_lReferenceFacadeGate)
        {
            return LReferenceFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference.LCitationRead(draft);
        }
    }

    internal LDraft LEngineReferenceStart(string origin, long? referenceId)
    {
        lock (_lReferenceFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LReferenceFacadeStaff.LEngineStaffEntry.LEntryStaffCitation.LCitationClerkReference
                .LReferenceCitationStart(origin, referenceId);
        }
    }

    internal LReference LEngineReferenceCommit(long id)
    {
        LReference settled;
        lock (_lReferenceFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lReferenceFacadeDraft.LEngineDraftValidate(id);
            settled = LReferenceFacadeStaff.LEngineStaffEntry.LEntryStaffCitation
                .LCitationClerkReference.LReferenceCitationCommit(id);
        }

        _lReferenceFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectReference, settled.LReferenceId);
        return settled;
    }
    private LEngineStaff LReferenceFacadeStaff => _lReferenceFacadeHearth.LEngineStaffHeld;
}
