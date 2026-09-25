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

    public LEntry? LEngineEntryRead(long id)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkRead(id);
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

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(register);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(situation);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(example);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryClerkFind(reference);
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
}
