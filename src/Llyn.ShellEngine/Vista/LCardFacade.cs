using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LCardFacade
{
    private readonly LEngine _lCardFacadeEngine;
    private readonly object _lCardFacadeGate;
    private LEngineStaff LCardFacadeStaff => _lCardFacadeEngine.LEngineStaffHeld;

    public LCardFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lCardFacadeEngine = engine;
        _lCardFacadeGate = engine.LEngineGate;
    }

    public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)
    {
        lock (_lCardFacadeGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngine.LEngineOwnerRaise(owner);
            }

            return LCardFacadeStaff.LEngineStaffMeaning.LMeaningClerkScan(ownerId);
        }
    }

    internal IReadOnlyList<LTag> LEngineTagRead()
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffTag.LTagClerkRead();
        }
    }

    public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffTag.LTagClerkFind(query, order);
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

    public LTag LEngineTagCreate(string text)
    {
        LTag created;
        lock (_lCardFacadeGate)
        {
            created = LCardFacadeStaff.LEngineStaffTag.LTagClerkCreate(text);
        }

        _lCardFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectTag, created.LTagId);
        return created;
    }

    public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffRegister.LRegisterClerkFind(query, language);
        }
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffRegister.LRegisterClerkFind(query, order);
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
        lock (_lCardFacadeGate)
        {
            created = LCardFacadeStaff.LEngineStaffRegister.LRegisterClerkCreate(name);
        }

        _lCardFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectRegister, created.LRegisterId);
        return created;
    }

    public IReadOnlyList<LVistaRow> LEngineProspectFind(string query)
    {
        lock (_lCardFacadeGate)
        {
            IReadOnlyList<LEntry> entries =
                LCardFacadeStaff.LEngineStaffTranslation.LTranslationClerkFind(query, null);
            return _lCardFacadeEngine.LEngineVista.LEngineVistaBuild(entries, null);
        }
    }

    public LEntry? LEngineTranslationResolve(string word, long? entryId)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffTranslation.LTranslationClerkResolve(word, entryId);
        }
    }

    internal LEntry LEngineTranslationCreate(string headword, string language)
    {
        LEntry entry;
        lock (_lCardFacadeGate)
        {
            entry = LCardFacadeStaff.LEngineStaffTranslation.LTranslationClerkCreate(headword, language);
            _lCardFacadeEngine.LEnginePronunciation.LEngineFrequencyStart(entry.LEntryId);
        }

        _lCardFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectEntry, entry.LEntryId);
        return entry;
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<long> ids)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffTranslation.LTranslationTargetRead(ids);
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
        LEntryDraft? draft = _lCardFacadeEngine.LEngineDraft.LEngineDraftRead(ownerId)?.LDraftContent;
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
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffTranslation.LTranslationTargetRead(
                ownerId, ids, LCardFacadeStaff.LEngineStaffCourt.LCourtClerkScan(ownerId));
        }
    }

    public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffTranslation.LTranslationIncomingRead(
                entryId, _lCardFacadeEngine.LEngineSettingsHeld.LSettingsEpithet);
        }
    }
}
