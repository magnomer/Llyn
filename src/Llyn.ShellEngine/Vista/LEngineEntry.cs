using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkCreate(entry, forms, speeches);
        }
    }

    public LEntry? LEngineEntryRead(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkRead(id);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(query);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(query, order);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(query, order, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(tag);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(tag, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, string query, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(tag, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(register);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(register, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, string query, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(register, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(situation);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation, string query, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(situation, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(example);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example, string query, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(example, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(reference);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference, string query, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(reference, query, filter);
        }
    }

    public LEntryDraft? LEngineEntryLoad(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkLoad(id);
        }
    }

    internal LRevision LEngineEntryDelete(long id)
    {
        LRevision recorded;
        lock (LEngineGate)
        {
            recorded = _lEngineStaff.LEngineStaffEntry.LEntryClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, id);
        return recorded;
    }

    internal LTombstone? LEngineTombstoneRead(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LTombstoneRead(entryId);
        }
    }

    internal LRevision? LEngineRevisionRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LRevisionRead();
        }
    }

    internal IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LRevisionChangeRead(revisionId);
        }
    }
    public LGlyph? LEngineGlyphRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? null : LEngineLanguageLoad(language).LLanguageGlyph;
    }

    public LEntry LEngineGlyphResolve(string character, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string headword = character.Trim();
        lock (LEngineGate)
        {
            foreach (LEntry entry in _lEngineStaff.LEngineStaffEntry.LEntryClerkFind(headword))
            {
                if (string.Equals(entry.LEntryHeadword, headword, StringComparison.Ordinal)
                    && string.Equals(entry.LEntryLanguage, language, StringComparison.Ordinal))
                {
                    return entry;
                }
            }

            return LEngineTranslationCreate(headword, language);
        }
    }

    public int LEngineGraspStep => LEntryClerk.LEntryGraspStep;

    public string LEngineGraspFormat(int step)
    {
        return LEntryClerk.LEntryGraspFormat(step);
    }

    public int LEngineGraspRead(long entryId)
    {
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            return _lEngineStaff.LEngineStaffEntry.LEntryClerkRead(entryId)?.LEntryGrasp ?? 0;
        }
    }

    public void LEngineGraspSave(long entryId, int grasp)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffEntry.LEntryGraspSet(entryId, grasp);
        }

        LEngineBulletinRaise(LSubject.LSubjectGrasp, entryId);
    }

    public string LEngineEpithetRead(long entryId)
    {
        lock (LEngineGate)
        {
            return LEngineSettingsHeld.LSettingsEpithet
                ? _lEngineStaff.LEngineStaffEntry.LEntryEpithetRead(entryId)
                : string.Empty;
        }
    }

    public LEstablishment LEngineEstablishmentRead()
    {
        lock (LEngineGate)
        {
            int unsaved = 0;
            foreach (long id in _lEngineStaff.LEngineStaffClaim.LClaimClerkHeld)
            {
                if (LEngineDraftCheck(id))
                {
                    unsaved++;
                }
            }

            return new LEstablishment(
                unsaved,
                _lEngineStaff.LEngineStaffEntry.LEntryCountRead(),
                _lEngineStaff.LEngineStaffEntry.LWorkspaceSizeRead());
        }
    }

    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffUsage.LUsageClerkRead(owner);
        }
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffUsage.LUsageClerkRead(id, owner, LEngineSettingsHeld.LSettingsEpithet);
        }
    }
}
