using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LEntryFacade : LEntryPort, LGraspPort
{
    private readonly LEngineHearth _lEntryFacadeHearth;
    private readonly LCardFacade _lEntryFacadeCard;
    private readonly LDraftFacade _lEntryFacadeDraft;
    private readonly object _lEntryFacadeGate;

    internal LEntryFacade(LEngineHearth hearth, LCardFacade card, LDraftFacade draft)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(draft);
        _lEntryFacadeHearth = hearth;
        _lEntryFacadeCard = card;
        _lEntryFacadeDraft = draft;
        _lEntryFacadeGate = _lEntryFacadeHearth.LEngineGate;
    }

    private LEngineStaff LEntryFacadeStaff => _lEntryFacadeHearth.LEngineStaffHeld;

    public LEntry? LEngineEntryRead(long id)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffEntry.LEntryClerkRead(id);
        }
    }

    public (bool, string, string) LEngineStampRead(long entryId)
    {
        return LEngineEntryRead(entryId) is LEntry entry
            ? (true, LEngineStampFormat(entry.LEntryAddedUtc), LEngineStampFormat(entry.LEntryUpdatedUtc))
            : (false, string.Empty, string.Empty);
    }

    internal static string LEngineStampFormat(string? utc)
    {
        if (utc is null
            || !DateTimeOffset.TryParse(
                utc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset parsed))
        {
            return string.Empty;
        }

        return parsed.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(query, order, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSubject subject, long id)
    {
        lock (_lEntryFacadeGate)
        {
            return subject switch
            {
                LSubject.LSubjectTag => LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(
                    new LTag(id, string.Empty)),
                LSubject.LSubjectRegister => LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(
                    new LRegister(id, LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectExample => LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(
                    new LExample(id, string.Empty, LStateValue.LStateValueUnspecified,
                        LStateAnchor.LStateAnchorUnspecified)),
                LSubject.LSubjectSituation => LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(
                    new LSituation(id,
                        LStateValue.LStateValueUnspecified,
                        LStateValue.LStateValueUnspecified,
                        LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectReference => LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(
                    LReferenceClerk.LReferenceClerkBlank with { LReferenceId = id }),
                _ => [],
            };
        }
    }

    public LEntryDraft? LEngineEntryLoad(long id)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffEntry.LEntryClerkLoad(id);
        }
    }

    internal LRevision LEngineEntryDelete(long id)
    {
        LRevision recorded;
        lock (_lEntryFacadeGate)
        {
            recorded = LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffEntry.LEntryClerkDelete(id);
        }

        _lEntryFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectEntry, id);
        return recorded;
    }

    public IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return draft.LEntryDraftReflexes.Where(static reflex => reflex.LReflexDraftWritten).ToList();
    }

    public LEntry LEngineGlyphResolve(string character, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string headword = character.Trim();
        lock (_lEntryFacadeGate)
        {
            foreach (LEntry entry in LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(headword))
            {
                if (string.Equals(entry.LEntryHeadword, headword, StringComparison.Ordinal)
                    && string.Equals(entry.LEntryLanguage, language, StringComparison.Ordinal))
                {
                    return entry;
                }
            }

            return _lEntryFacadeCard.LEngineTranslationCreate(headword, language);
        }
    }

    long LEntryPort.LEngineGlyphResolve(string character, string language)
    {
        return LEngineGlyphResolve(character, language).LEntryId;
    }

    public string LEngineUnitFormat(LUnit unit)
    {
        return LUnitClerk.LUnitFormat(unit);
    }

    public (LOwner, int)? LEngineCardFind(LEntryDraft draft, long id)
    {
        return LDraftClerkCard.LCardOwnerFind(draft, id);
    }

    public int LEngineGraspStep => LGraspClerk.LGraspClerkStep;

    public string LEngineGraspFormat(int step)
    {
        return LGraspClerk.LGraspClerkFormat(step);
    }

    public int LEngineGraspRead(long entryId)
    {
        lock (_lEntryFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            return LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffEntry.LEntryClerkRead(entryId)?.LEntryGrasp ?? 0;
        }
    }

    public void LEngineGraspSave(long entryId, int grasp)
    {
        lock (_lEntryFacadeGate)
        {
            LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffGrasp.LGraspClerkSet(entryId, grasp);
        }

        _lEntryFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectGrasp, entryId);
    }

    public LEstablishment LEngineEstablishmentRead()
    {
        lock (_lEntryFacadeGate)
        {
            int unsaved = 0;
            foreach (long id in LEntryFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkHeld)
            {
                if (_lEntryFacadeDraft.LEngineDraftCheck(id))
                {
                    unsaved++;
                }
            }

            return new LEstablishment(
                unsaved,
                LEntryFacadeStaff.LEngineStaffEntry.LEntryStaffQuery.LEntryCountRead(),
                LEntryFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceSizeRead());
        }
    }

    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffCatalog.LCatalogStaffUsage.LUsageClerkRead(owner);
        }
    }

    public string LEngineTallyRead(long? reference)
    {
        return LEngineTallyRead(reference, LOwner.LOwnerReference);
    }

    public string LEngineTallyRead(long? id, LOwner owner)
    {
        lock (_lEntryFacadeGate)
        {
            return LEntryFacadeStaff.LEngineStaffCatalog.LCatalogStaffUsage.LUsageTallyRead(id, owner);
        }
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)
    {
        lock (_lEntryFacadeGate)
        {
            bool epithet = _lEntryFacadeHearth.LEngineSettingsHeld.LSettingsEpithet;
            return LEntryFacadeStaff.LEngineStaffCatalog.LCatalogStaffUsage.LUsageClerkRead(id, owner, epithet);
        }
    }
}
