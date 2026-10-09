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
        LTenure held, long card, long sentence, string text, int start, int length, string key)
    {
        ArgumentNullException.ThrowIfNull(held);

        return new LQuillMention(held).LQuillSenseRead(card, sentence, text, start, length) is long entry
            ? LEngineMeaningRead(entry, key)
            : null;
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

    public IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        IReadOnlyList<LCardDraft> cards = [.. draft.LEntryDraftMeanings, .. draft.LEntryDraftCollocations];
        List<long> ids = cards.SelectMany(static card => card.LCardDraftTranslation).Distinct().ToList();
        IReadOnlyList<LTranslationTarget> read = ids.Count == 0 ? [] : LEngineTargetRead(ids);
        return LEngineTranslationResolve(cards, read);
    }

    internal static IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationResolve(
        IReadOnlyList<LCardDraft> cards, IReadOnlyList<LTranslationTarget> read)
    {
        Dictionary<long, LTranslationTarget> found = [];
        foreach (LTranslationTarget target in read)
        {
            found[target.LTranslationTargetId] = target;
        }

        Dictionary<long, IReadOnlyList<LTranslationTarget>> targets = [];
        foreach (LCardDraft card in cards)
        {
            targets[card.LCardDraftId] = card.LCardDraftTranslation
                .Where(found.ContainsKey)
                .Select(id => found[id])
                .ToList();
        }

        return targets;
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

    public LEtymologyResult LEngineEtymologyRead(LEntryDraft draft)
    {
        bool narrated = draft.LEntryDraftEtymology.LEtymologyDraftNarrated;
        return new LEtymologyResult(LEngineEtymonRead(draft), narrated);
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

    internal static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
    }
}
