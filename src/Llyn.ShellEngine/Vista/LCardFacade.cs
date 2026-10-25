using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LCardFacade : LCardPort
{
    private readonly LEngineHearth _lCardFacadeHearth;
    private readonly LPronunciationFacade _lCardFacadePronunciation;
    private readonly LSettingsFacade _lCardFacadeSettings;
    private readonly LVistaRowFacade _lCardFacadeRow;
    private readonly object _lCardFacadeGate;
    private LEngineStaff LCardFacadeStaff => _lCardFacadeHearth.LEngineStaffHeld;

    internal LCardFacade(
        LEngineHearth hearth, LPronunciationFacade pronunciation, LSettingsFacade settings, LVistaRowFacade row)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(row);
        _lCardFacadeHearth = hearth;
        _lCardFacadePronunciation = pronunciation;
        _lCardFacadeSettings = settings;
        _lCardFacadeRow = row;
        _lCardFacadeGate = _lCardFacadeHearth.LEngineGate;
    }

    public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)
    {
        lock (_lCardFacadeGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return LCardFacadeStaff.LEngineStaffEntry.LEntryStaffMeaning.LMeaningClerkScan(ownerId);
        }
    }

    public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LEngineMeaningRead(
        long entryId, string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        return LMeaningClerk.LMeaningClerkSort(
            LEngineMeaningRead(entryId, LOwner.LOwnerEntry), _lCardFacadeSettings.LEngineTextRead(key));
    }

    public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)>? LEngineSenseRead(
        LTenure held, LExampleSite site, string text, int start, int length, string key)
    {
        ArgumentNullException.ThrowIfNull(held);

        return new LQuillMention(held).LQuillSenseRead(site, text, start, length) is long entry
            ? LEngineMeaningRead(entry, key)
            : null;
    }

    public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth, string LMeaningPath)>
        LEngineMeaningScan(LTenure held, string key)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(key);

        if (held.LTenureRead() is not LDraft draft)
        {
            return [];
        }

        return LMeaningClerk.LMeaningClerkScan(
            draft.LDraftStored ?? 0,
            draft.LDraftContent.LEntryDraftMeanings,
            _lCardFacadeSettings.LEngineTextRead(key));
    }

    public LCollocationLayout LEngineCollocationRead(
        LEntryDraft shown, IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> targets)
    {
        return LDraftClerkSense.LCollocationLayoutRead(shown, targets);
    }

    public LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen)
    {
        ArgumentNullException.ThrowIfNull(held);

        long? self = held.LTenureRead()?.LDraftStored;
        IReadOnlyList<string> known;
        lock (_lCardFacadeGate)
        {
            known = LCardFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageClerkRead();
        }

        IReadOnlyList<string> languages = LTranslationClerk.LTranslationLanguageRead(known, held.LTenureLanguageRead());
        IReadOnlyList<LVistaRow> rows;
        lock (_lCardFacadeGate)
        {
            IReadOnlyList<LEntry> entries =
                LCardFacadeStaff.LEngineStaffCatalog.LCatalogStaffTranslation.LTranslationClerkFind(word, null);
            rows = _lCardFacadeRow.LEngineVistaBuild(entries, null);
        }

        return new LTranslationOffer(
            text, word, rows.Where(row => row.LVistaRowId != self).ToList(), true, chosen, languages);
    }

    public IReadOnlyList<LVistaRow> LEngineProspectFind(LTenure held, string word)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lCardFacadeGate)
        {
            IReadOnlyList<LEntry> entries =
                LCardFacadeStaff.LEngineStaffCatalog.LCatalogStaffTranslation.LTranslationMentionFind(word, draft);
            return _lCardFacadeRow.LEngineVistaBuild(entries, null);
        }
    }

    public LEntry? LEngineTranslationResolve(string word, long? entryId)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffCatalog.LCatalogStaffTranslation
                .LTranslationClerkResolve(word, entryId);
        }
    }

    internal LEntry LEngineTranslationCreate(string headword, string language)
    {
        LEntry entry;
        lock (_lCardFacadeGate)
        {
            entry = LCardFacadeStaff.LEngineStaffCatalog.LCatalogStaffTranslation
                .LTranslationClerkCreate(headword, language);
            _lCardFacadePronunciation.LEngineFrequencyStart(entry.LEntryId);
        }

        _lCardFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectEntry, entry.LEntryId);
        return entry;
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<long> ids)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffCatalog.LCatalogStaffTranslation.LTranslationTargetRead(ids);
        }
    }

    public IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(
        LEntryDraft draft)
    {
        return LEngineTranslationResolve(draft, LEngineTargetRead);
    }

    internal static IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>>
        LEngineTranslationResolve(
            LEntryDraft draft, Func<IReadOnlyList<long>, IReadOnlyList<LTranslationTarget>> read)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(read);

        List<(LCardKind, LCardDraft)> cards = [];
        LEngineTranslationResolve(cards, LCardKind.LCardKindMeaning, draft.LEntryDraftMeanings);
        LEngineTranslationResolve(cards, LCardKind.LCardKindCollocation, draft.LEntryDraftCollocations);
        List<long> ids = cards.SelectMany(static held => held.Item2.LCardDraftTranslation).Distinct().ToList();
        IReadOnlyList<LTranslationTarget> answered = ids.Count == 0 ? [] : read(ids);

        Dictionary<long, LTranslationTarget> found = [];
        foreach (LTranslationTarget target in answered)
        {
            found[target.LTranslationTargetId] = target;
        }

        Dictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> targets = [];
        foreach ((LCardKind kind, LCardDraft card) in cards)
        {
            targets[(kind, card.LCardDraftId)] = card.LCardDraftTranslation
                .Where(found.ContainsKey)
                .Select(id => found[id])
                .ToList();
        }

        return targets;
    }

    private static void LEngineTranslationResolve(
        List<(LCardKind, LCardDraft)> cards, LCardKind kind, IReadOnlyList<LCardDraft> level)
    {
        foreach (LCardDraft card in level)
        {
            cards.Add((kind, card));
            LEngineTranslationResolve(cards, kind, card.LCardDraftChild);
        }
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(long ownerId, IReadOnlyList<long> ids)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffCatalog.LCatalogStaffTranslation.LTranslationTargetRead(
                ownerId, ids, LCardFacadeStaff.LEngineStaffClaim.LClaimStaffCourt.LCourtClerkScan(ownerId));
        }
    }

    public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffCatalog.LCatalogStaffTranslation.LTranslationIncomingRead(
                entryId, _lCardFacadeHearth.LEngineSettingsHeld.LSettingsEpithet);
        }
    }

    public IReadOnlySet<(LCardKind, long)> LEngineFoldRead(long entryId)
    {
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkRead(entryId);
        }
    }

    public void LEngineFoldSave(long entryId, LCardKind kind, long cardId)
    {
        lock (_lCardFacadeGate)
        {
            LCardFacadeStaff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkSave(kind, cardId);
        }

        _lCardFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectFold, entryId);
    }

    public void LEngineFoldDelete(long entryId, LCardKind kind, long cardId)
    {
        lock (_lCardFacadeGate)
        {
            LCardFacadeStaff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkDelete(kind, cardId);
        }

        _lCardFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectFold, entryId);
    }

    public (LOwner, int)? LEngineCardFind(LEntryDraft draft, LCardKind kind, long id)
    {
        return LDraftClerkCard.LCardOwnerFind(draft, kind, id);
    }

    internal static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
    }
}
