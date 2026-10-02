using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
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

    public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LEngineMeaningRead(
        long entryId, string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        return LMeaningClerk.LMeaningClerkSort(
            LEngineMeaningRead(entryId, LOwner.LOwnerEntry), _lCardFacadeEngine.LEngineSettings.LEngineTextRead(key));
    }

    public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)>? LEngineSenseRead(
        LTenure held, long card, long sentence, string text, int start, int length, string key)
    {
        ArgumentNullException.ThrowIfNull(held);

        return held.LTenureSenseRead(card, sentence, text, start, length) is long entry
            ? LEngineMeaningRead(entry, key)
            : null;
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

    public LTagOffer LEngineTagFind(LTenure held, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffTag.LTagClerkFind(text, draft, card);
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

    public LRegisterOffer LEngineRegisterFind(LTenure held, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        string language = held.LTenureLanguageRead();
        lock (_lCardFacadeGate)
        {
            return LCardFacadeStaff.LEngineStaffRegister.LRegisterClerkFind(text, language, draft, card);
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

    public LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen)
    {
        ArgumentNullException.ThrowIfNull(held);

        long? self = held.LTenureRead()?.LDraftStored;
        IReadOnlyList<string> languages = LTranslationClerk.LTranslationLanguageRead(
            _lCardFacadeEngine.LEngineLanguage.LEngineLanguageRead(), held.LTenureLanguageRead());
        IReadOnlyList<LVistaRow> rows;
        lock (_lCardFacadeGate)
        {
            IReadOnlyList<LEntry> entries =
                LCardFacadeStaff.LEngineStaffTranslation.LTranslationClerkFind(word, null);
            rows = _lCardFacadeEngine.LEngineVista.LEngineVistaBuild(entries, null);
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
                LCardFacadeStaff.LEngineStaffTranslation.LTranslationMentionFind(word, draft);
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

    public IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        IReadOnlyList<LCardDraft> cards = [.. draft.LEntryDraftMeanings, .. draft.LEntryDraftCollocations];
        List<long> ids = cards.SelectMany(static card => card.LCardDraftTranslation).Distinct().ToList();
        IReadOnlyList<LTranslationTarget> read;
        try
        {
            read = ids.Count == 0 ? [] : LEngineTargetRead(ids);
        }
        catch (Exception)
        {
            read = [];
        }

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
        try
        {
            return new LEtymologyResult(LEngineEtymonRead(draft), narrated);
        }
        catch (Exception)
        {
            return new LEtymologyResult([], narrated);
        }
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
