using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkCreate(entry, forms, speeches);
        }
    }

    public LEntry? LEngineEntryRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkRead(id);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(query);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(query, order);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(query, order, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(tag);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(tag, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(tag, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(register);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(register, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(register, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(situation);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(situation, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(example);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(example, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(reference);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(reference, query, filter);
        }
    }

    public LEntryDraft? LEngineEntryLoad(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkLoad(id);
        }
    }

    internal LRevision LEngineEntryDelete(long id)
    {
        LRevision recorded;
        lock (_lEngineGate)
        {
            recorded = _lEngineEntryClerk.LEntryClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, id);
        return recorded;
    }

    internal LTombstone? LEngineTombstoneRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LTombstoneRead(entryId);
        }
    }

    internal LRevision? LEngineRevisionRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LRevisionRead();
        }
    }

    internal IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LRevisionChangeRead(revisionId);
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
        lock (_lEngineGate)
        {
            foreach (LEntry entry in _lEngineEntryClerk.LEntryClerkFind(headword))
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
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            return _lEngineEntryClerk.LEntryClerkRead(entryId)?.LEntryGrasp ?? 0;
        }
    }

    public void LEngineGraspSave(long entryId, int grasp)
    {
        lock (_lEngineGate)
        {
            _lEngineEntryClerk.LEntryGraspSet(entryId, grasp);
        }

        LEngineBulletinRaise(LSubject.LSubjectGrasp, entryId);
    }

    public string LEngineEpithetRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineSettings.LSettingsEpithet ? _lEngineEntryClerk.LEntryEpithetRead(entryId) : string.Empty;
        }
    }

    public LEstablishment LEngineEstablishmentRead()
    {
        lock (_lEngineGate)
        {
            int unsaved = 0;
            foreach (long id in _lEngineClaimClerk.LClaimClerkHeld)
            {
                if (LEngineDraftCheck(id))
                {
                    unsaved++;
                }
            }

            return new LEstablishment(
                unsaved, _lEngineEntryClerk.LEntryCountRead(), _lEngineEntryClerk.LWorkspaceSizeRead());
        }
    }

    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineUsageClerk.LUsageClerkRead(owner);
        }
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineUsageClerk.LUsageClerkRead(id, owner, _lEngineSettings.LSettingsEpithet);
        }
    }
}
