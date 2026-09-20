using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LMeaning LEngineMeaningCreate(LMeaning meaning)
    {
        lock (_lEngineGate)
        {
            return _lEngineMeaningClerk.LMeaningClerkCreate(meaning);
        }
    }

    public LMeaning? LEngineMeaningRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineMeaningClerk.LMeaningClerkRead(id);
        }
    }

    public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return _lEngineMeaningClerk.LMeaningClerkScan(ownerId);
        }
    }

    internal void LEngineMeaningUpdate(LMeaning meaning)
    {
        lock (_lEngineGate)
        {
            _lEngineMeaningClerk.LMeaningClerkUpdate(meaning);
        }
    }

    internal void LEngineMeaningMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            _lEngineMeaningClerk.LMeaningClerkMove(id, position);
        }
    }

    internal void LEngineMeaningDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineMeaningClerk.LMeaningClerkDelete(id);
        }
    }

    internal LCollocation LEngineCollocationCreate(LCollocation collocation)
    {
        lock (_lEngineGate)
        {
            return _lEngineCardClerk.LCollocationCreate(collocation);
        }
    }

    internal IReadOnlyList<LCollocation> LEngineCollocationRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return _lEngineCardClerk.LCollocationRead(ownerId);
        }
    }

    internal void LEngineCollocationUpdate(LCollocation collocation)
    {
        lock (_lEngineGate)
        {
            _lEngineCardClerk.LCollocationUpdate(collocation);
        }
    }

    internal void LEngineCollocationMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            _lEngineCardClerk.LCollocationMove(id, position);
        }
    }

    internal void LEngineCollocationDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineCardClerk.LCollocationDelete(id);
        }
    }

    internal IReadOnlyList<LTag> LEngineTagRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineTagClerk.LTagClerkRead();
        }
    }

    public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineTagClerk.LTagClerkFind(query, order);
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
        lock (_lEngineGate)
        {
            return _lEngineTagClerk.LTagClerkRead(ownerId, LEngineOwnerCheck(owner));
        }
    }

    internal void LEngineTagSave(long ownerId, IReadOnlyList<LTag> written, LOwner owner)
    {
        lock (_lEngineGate)
        {
            bool collocation = LEngineOwnerCheck(owner);
            _lEngineTagClerk.LTagClerkSave(ownerId, written, collocation);
            LEngineUpdatedSet(ownerId, collocation);
        }
    }

    public LTag LEngineTagCreate(string text)
    {
        LTag created;
        lock (_lEngineGate)
        {
            created = _lEngineTagClerk.LTagClerkCreate(text);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, created.LTagId);
        return created;
    }

    internal void LEngineTagChange(long tagId, string renamed)
    {
        lock (_lEngineGate)
        {
            _lEngineTagClerk.LTagClerkChange(tagId, renamed);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }

    internal void LEngineTagDelete(long tagId)
    {
        lock (_lEngineGate)
        {
            _lEngineTagClerk.LTagClerkDelete(tagId);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }

    internal IReadOnlyList<LRegister> LEngineRegisterRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineRegisterClerk.LRegisterClerkRead(ownerId, LEngineOwnerCheck(owner));
        }
    }

    public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineRegisterClerk.LRegisterClerkFind(query, language);
        }
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineRegisterClerk.LRegisterClerkFind(query, order);
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
        lock (_lEngineGate)
        {
            created = _lEngineRegisterClerk.LRegisterClerkCreate(name);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, created.LRegisterId);
        return created;
    }

    internal void LEngineRegisterChange(long registerId, string renamed)
    {
        lock (_lEngineGate)
        {
            _lEngineRegisterClerk.LRegisterClerkChange(registerId, renamed);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    internal void LEngineRegisterDelete(long registerId)
    {
        lock (_lEngineGate)
        {
            _lEngineRegisterClerk.LRegisterClerkDelete(registerId);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    internal IReadOnlyList<LTranslation> LEngineTranslationRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranslationClerk.LTranslationClerkRead(ownerId, LEngineOwnerCheck(owner));
        }
    }

    internal void LEngineTranslationSave(long ownerId, IReadOnlyList<long> ids, LOwner owner)
    {
        lock (_lEngineGate)
        {
            bool collocation = LEngineOwnerCheck(owner);
            _lEngineTranslationClerk.LTranslationClerkSave(ownerId, ids, collocation);
            LEngineUpdatedSet(ownerId, collocation);
        }
    }

    private void LEngineTranslationSave(long ownerId, IReadOnlyList<long> ids, bool collocation)
    {
        _lEngineTranslationClerk.LTranslationClerkSave(ownerId, ids, collocation);
    }

    internal IReadOnlyList<LEntry> LEngineTranslationFind(string query, long? entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranslationClerk.LTranslationClerkFind(query, entryId);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineProspectFind(string query)
    {
        lock (_lEngineGate)
        {
            return LEngineVistaBuild(_lEngineTranslationClerk.LTranslationClerkFind(query, null), null);
        }
    }

    public LEntry? LEngineTranslationResolve(string word, long? entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranslationClerk.LTranslationClerkResolve(word, entryId);
        }
    }

    internal LEntry LEngineTranslationCreate(string headword, string language)
    {
        LEntry entry;
        lock (_lEngineGate)
        {
            entry = _lEngineTranslationClerk.LTranslationClerkCreate(headword, language);
            LEngineFrequencyStart(entry.LEntryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, entry.LEntryId);
        return entry;
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<long> ids)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranslationClerk.LTranslationTargetRead(ids);
        }
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LEngineTargetRead(draft.LEntryDraftTargets);
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(long ownerId)
    {
        LEntryDraft? draft = LEngineDraftRead(ownerId)?.LDraftContent;
        return draft is null || !draft.LEntryDraftTargeted ? [] : LEngineTargetRead(ownerId, draft.LEntryDraftTargets);
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
        lock (_lEngineGate)
        {
            return _lEngineTranslationClerk.LTranslationTargetRead(
                ownerId, ids, _lEngineCourtClerk.LCourtClerkScan(ownerId));
        }
    }

    public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranslationClerk.LTranslationIncomingRead(entryId, _lEngineSettings.LSettingsEpithet);
        }
    }
}
