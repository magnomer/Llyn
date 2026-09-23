using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LEntryFacade
{
    private readonly LEngine _lEntryFacadeEngine;
    private readonly object _lEntryFacadeGate;

    public LEntryFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lEntryFacadeEngine = engine;
        _lEntryFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LEntryFacadeStaff => _lEntryFacadeEngine.LEngineStaffHeld;

    internal LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkCreate(entry, forms, speeches);
        }
    }

    public LEntry? LEngineEntryRead(long id)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkRead(id);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(query);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(query, order);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(query, order, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(tag);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(tag, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, string query, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(tag, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(register);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(register, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, string query, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(register, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(situation);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation, string query, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(situation, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(example);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example, string query, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(example, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(reference);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference, string query, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(reference, query, filter);
        }
    }

    public LEntryDraft? LEngineEntryLoad(long id)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkLoad(id);
        }
    }

    internal LRevision LEngineEntryDelete(long id)
    {
        LRevision recorded;
        lock (_lEntryFacadeGate)
        {
            recorded = LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkDelete(id);
        }

        _lEntryFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectEntry, id);
        return recorded;
    }

    internal LTombstone? LEngineTombstoneRead(long entryId)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LTombstoneRead(entryId);
        }
    }

    internal LRevision? LEngineRevisionRead()
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LRevisionRead();
        }
    }

    internal IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LRevisionChangeRead(revisionId);
        }
    }
    public LGlyph? LEngineGlyphRead(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return null;
        }

        return _lEntryFacadeEngine.LEngineLanguage.LEngineLanguageLoad(language).LLanguageGlyph;
    }

    public LEntry LEngineGlyphResolve(string character, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string headword = character.Trim();
        lock (_lEntryFacadeGate)
        {
            foreach (LEntry entry in LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(headword))
            {
                if (string.Equals(entry.LEntryHeadword, headword, StringComparison.Ordinal)
                    && string.Equals(entry.LEntryLanguage, language, StringComparison.Ordinal))
                {
                    return entry;
                }
            }

            return _lEntryFacadeEngine.LEngineCard.LEngineTranslationCreate(headword, language);
        }
    }

    public int LEngineGraspStep => LEntryClerk.LEntryGraspStep;

    public string LEngineGraspFormat(int step)
    {
        return LEntryClerk.LEntryGraspFormat(step);
    }

    public int LEngineGraspRead(long entryId)
    {
        lock (_lEntryFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkRead(entryId)?.LEntryGrasp ?? 0;
        }
    }

    public void LEngineGraspSave(long entryId, int grasp)
    {
        lock (_lEntryFacadeGate)
        {
            LEntryFacadeStaff.LEngineStaffEntry.LEntryGraspSet(entryId, grasp);
        }

        _lEntryFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectGrasp, entryId);
    }

    public string LEngineEpithetRead(long entryId)
    {
        lock (_lEntryFacadeGate)
        {
            return _lEntryFacadeEngine.LEngineSettingsHeld.LSettingsEpithet
                ? LEntryFacadeStaff.LEngineStaffEntry.LEntryEpithetRead(entryId)
                : string.Empty;
        }
    }

    public LEstablishment LEngineEstablishmentRead()
    {
        lock (_lEntryFacadeGate)
        {
            int unsaved = 0;
            foreach (long id in LEntryFacadeStaff.LEngineStaffClaim.LClaimClerkHeld)
            {
                if (_lEntryFacadeEngine.LEngineDraft.LEngineDraftCheck(id))
                {
                    unsaved++;
                }
            }

            return new LEstablishment(
                unsaved,
                LEntryFacadeStaff.LEngineStaffEntry.LEntryCountRead(),
                LEntryFacadeStaff.LEngineStaffEntry.LWorkspaceSizeRead());
        }
    }

    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffUsage.LUsageClerkRead(owner);
        }
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)
    {
        lock (_lEntryFacadeGate)
        {
            bool epithet = _lEntryFacadeEngine.LEngineSettingsHeld.LSettingsEpithet;
            return LEntryFacadeStaff.LEngineStaffUsage.LUsageClerkRead(id, owner, epithet);
        }
    }

    internal LEntry LEngineEntrySave(LEntryDraft draft)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffOutcome.LOutcomeEntrySave(draft, []);
        }
    }

    internal LEntry LEngineEntryUpdate(long id, LEntryDraft draft)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffOutcome.LOutcomeEntryUpdate(id, draft, []);
        }
    }

    private void LEngineUpdatedSet(long entryId)
    {
        lock (_lEntryFacadeGate)
        {
            LEntryFacadeStaff.LEngineStaffEntry.LEntryUpdatedSet(entryId);
        }
    }

    internal void LEngineUpdatedSet(long ownerId, bool collocation)
    {
        lock (_lEntryFacadeGate)
        {
            LEntryFacadeStaff.LEngineStaffCard.LCardUpdatedSet(ownerId, collocation);
        }
    }
}
