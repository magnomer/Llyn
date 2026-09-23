using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LMeaning LEngineMeaningCreate(LMeaning meaning)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffMeaning.LMeaningClerkCreate(meaning);
        }
    }

    public LMeaning? LEngineMeaningRead(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffMeaning.LMeaningClerkRead(id);
        }
    }

    public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return _lEngineStaff.LEngineStaffMeaning.LMeaningClerkScan(ownerId);
        }
    }

    internal void LEngineMeaningUpdate(LMeaning meaning)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffMeaning.LMeaningClerkUpdate(meaning);
        }
    }

    internal void LEngineMeaningMove(long id, int position)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffMeaning.LMeaningClerkMove(id, position);
        }
    }

    internal void LEngineMeaningDelete(long id)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffMeaning.LMeaningClerkDelete(id);
        }
    }

    internal LCollocation LEngineCollocationCreate(LCollocation collocation)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffCard.LCollocationCreate(collocation);
        }
    }

    internal IReadOnlyList<LCollocation> LEngineCollocationRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return _lEngineStaff.LEngineStaffCard.LCollocationRead(ownerId);
        }
    }

    internal void LEngineCollocationUpdate(LCollocation collocation)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffCard.LCollocationUpdate(collocation);
        }
    }

    internal void LEngineCollocationMove(long id, int position)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffCard.LCollocationMove(id, position);
        }
    }

    internal void LEngineCollocationDelete(long id)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffCard.LCollocationDelete(id);
        }
    }

    internal IReadOnlyList<LTag> LEngineTagRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTag.LTagClerkRead();
        }
    }

    public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTag.LTagClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        List<LCatalogTag> rows = [];
        bool kept = false;
        foreach (LTag tag in LEngineTagFind(vista.LVistaQuery, vista.LVistaOrder))
        {
            bool chosen = vista.LVistaMatch(tag.LTagId);
            kept |= chosen;
            rows.Add(new LCatalogTag(tag, chosen));
        }

        if (!kept)
        {
            vista.LVistaSelect(null);
        }

        return rows;
    }

    internal IReadOnlyList<LTag> LEngineTagRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTag.LTagClerkRead(ownerId, LEngineOwnerCheck(owner));
        }
    }

    internal void LEngineTagSave(long ownerId, IReadOnlyList<LTag> written, LOwner owner)
    {
        lock (LEngineGate)
        {
            bool collocation = LEngineOwnerCheck(owner);
            _lEngineStaff.LEngineStaffTag.LTagClerkSave(ownerId, written, collocation);
            LEngineUpdatedSet(ownerId, collocation);
        }
    }

    public LTag LEngineTagCreate(string text)
    {
        LTag created;
        lock (LEngineGate)
        {
            created = _lEngineStaff.LEngineStaffTag.LTagClerkCreate(text);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, created.LTagId);
        return created;
    }

    internal void LEngineTagChange(long tagId, string renamed)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffTag.LTagClerkChange(tagId, renamed);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }

    internal void LEngineTagDelete(long tagId)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffTag.LTagClerkDelete(tagId);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }

    internal IReadOnlyList<LRegister> LEngineRegisterRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffRegister.LRegisterClerkRead(ownerId, LEngineOwnerCheck(owner));
        }
    }

    public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffRegister.LRegisterClerkFind(query, language);
        }
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffRegister.LRegisterClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        List<LCatalogRegister> rows = [];
        bool kept = false;
        foreach (LCatalogRegister row in LEngineRegisterFind(vista.LVistaQuery, vista.LVistaOrder))
        {
            bool chosen = vista.LVistaMatch(row.LCatalogRegisterStored.LRegisterId);
            kept |= chosen;
            rows.Add(row with { LCatalogRegisterChosen = chosen });
        }

        if (!kept)
        {
            vista.LVistaSelect(null);
        }

        return rows;
    }

    public LRegister LEngineRegisterCreate(string name)
    {
        LRegister created;
        lock (LEngineGate)
        {
            created = _lEngineStaff.LEngineStaffRegister.LRegisterClerkCreate(name);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, created.LRegisterId);
        return created;
    }

    internal void LEngineRegisterChange(long registerId, string renamed)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffRegister.LRegisterClerkChange(registerId, renamed);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    internal void LEngineRegisterDelete(long registerId)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffRegister.LRegisterClerkDelete(registerId);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    internal IReadOnlyList<LTranslation> LEngineTranslationRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranslation.LTranslationClerkRead(ownerId, LEngineOwnerCheck(owner));
        }
    }

    internal void LEngineTranslationSave(long ownerId, IReadOnlyList<long> ids, LOwner owner)
    {
        lock (LEngineGate)
        {
            bool collocation = LEngineOwnerCheck(owner);
            _lEngineStaff.LEngineStaffTranslation.LTranslationClerkSave(ownerId, ids, collocation);
            LEngineUpdatedSet(ownerId, collocation);
        }
    }

    private void LEngineTranslationSave(long ownerId, IReadOnlyList<long> ids, bool collocation)
    {
        _lEngineStaff.LEngineStaffTranslation.LTranslationClerkSave(ownerId, ids, collocation);
    }

    internal IReadOnlyList<LEntry> LEngineTranslationFind(string query, long? entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranslation.LTranslationClerkFind(query, entryId);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineProspectFind(string query)
    {
        lock (LEngineGate)
        {
            return LEngineVistaBuild(_lEngineStaff.LEngineStaffTranslation.LTranslationClerkFind(query, null), null);
        }
    }

    public LEntry? LEngineTranslationResolve(string word, long? entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranslation.LTranslationClerkResolve(word, entryId);
        }
    }

    internal LEntry LEngineTranslationCreate(string headword, string language)
    {
        LEntry entry;
        lock (LEngineGate)
        {
            entry = _lEngineStaff.LEngineStaffTranslation.LTranslationClerkCreate(headword, language);
            LEngineFrequencyStart(entry.LEntryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, entry.LEntryId);
        return entry;
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<long> ids)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranslation.LTranslationTargetRead(ids);
        }
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LEngineTargetRead([.. draft.LEntryDraftTargets, .. draft.LEntryDraftSources]);
    }

    public IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        IReadOnlyList<long> etymons = draft.LEntryDraftEtymology.LEtymologyDraftEtymons;
        Dictionary<long, LTranslationTarget> found = [];
        foreach (LTranslationTarget target in LEngineTargetRead(etymons))
        {
            found[target.LTranslationTargetId] = target;
        }

        return [.. etymons.Where(found.ContainsKey).Select(id => found[id])];
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(long ownerId)
    {
        LEntryDraft? draft = LEngineDraftRead(ownerId)?.LDraftContent;
        if (draft is null)
        {
            return [];
        }

        List<long> ids = [.. draft.LEntryDraftTargets, .. draft.LEntryDraftSources];
        return ids.Count == 0 ? [] : LEngineTargetRead(ownerId, ids);
    }

    public IReadOnlyDictionary<long, LTranslationTarget> LEngineTargetFind(long ownerId)
    {
        Dictionary<long, LTranslationTarget> targets = [];
        foreach (LTranslationTarget target in LEngineTargetRead(ownerId))
        {
            targets[target.LTranslationTargetId] = target;
        }

        return targets;
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(long ownerId, IReadOnlyList<long> ids)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranslation.LTranslationTargetRead(
                ownerId, ids, _lEngineStaff.LEngineStaffCourt.LCourtClerkScan(ownerId));
        }
    }

    public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranslation.LTranslationIncomingRead(
                entryId, LEngineSettingsHeld.LSettingsEpithet);
        }
    }
}
