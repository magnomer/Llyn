using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LLiveryFacade
{
    private readonly LEngineHearth _lLiveryFacadeHearth;
    private readonly LCardFacade _lLiveryFacadeCard;
    private readonly LCatalogFacade _lLiveryFacadeCatalog;
    private readonly LEntryFacade _lLiveryFacadeEntry;
    private readonly LFanqieFacade _lLiveryFacadeFanqie;
    private readonly LLanguageFacade _lLiveryFacadeLanguage;
    private readonly LReferenceFacade _lLiveryFacadeReference;
    private readonly LReflexFacade _lLiveryFacadeReflex;
    private readonly LScriptFacade _lLiveryFacadeScript;
    private readonly LSettingsFacade _lLiveryFacadeSettings;
    private readonly LVocabularyFacade _lLiveryFacadeVocabulary;

    public LLiveryFacade(
        LEngineHearth hearth,
        LCardFacade card,
        LCatalogFacade catalog,
        LEntryFacade entry,
        LFanqieFacade fanqie,
        LLanguageFacade language,
        LReferenceFacade reference,
        LReflexFacade reflex,
        LScriptFacade script,
        LSettingsFacade settings,
        LVocabularyFacade vocabulary)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(fanqie);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(reflex);
        ArgumentNullException.ThrowIfNull(script);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(vocabulary);
        _lLiveryFacadeHearth = hearth;
        _lLiveryFacadeCard = card;
        _lLiveryFacadeCatalog = catalog;
        _lLiveryFacadeEntry = entry;
        _lLiveryFacadeFanqie = fanqie;
        _lLiveryFacadeLanguage = language;
        _lLiveryFacadeReference = reference;
        _lLiveryFacadeReflex = reflex;
        _lLiveryFacadeScript = script;
        _lLiveryFacadeSettings = settings;
        _lLiveryFacadeVocabulary = vocabulary;
    }

    private LEngineStaff LLiveryFacadeStaff => _lLiveryFacadeHearth.LEngineStaffHeld;

    public LLiveryPage? LEngineLiveryRead(long entryId)
    {
        lock (_lLiveryFacadeHearth.LEngineGate)
        {
            if (_lLiveryFacadeEntry.LEngineEntryLoad(entryId) is not LEntryDraft draft)
            {
                return null;
            }

            LEngineStaff staff = LLiveryFacadeStaff;
            string language = draft.LEntryDraftLanguage;
            IReadOnlyList<string> reflexes =
                [.. draft.LEntryDraftReflexes.Select(static row => row.LReflexDraftLanguage)];
            (_, string created, string updated) = _lLiveryFacadeEntry.LEngineStampRead(entryId);
            LAccentSheet accent = _lLiveryFacadeLanguage.LEngineAccentRead(draft);
            return LLiveryClerk.LLiveryClerkBuild(
                draft,
                _lLiveryFacadeCatalog.LEngineFavoriteCheck(entryId),
                _lLiveryFacadeEntry.LEngineGraspRead(entryId),
                created,
                updated,
                accent,
                _lLiveryFacadeReflex.LEngineGuiseRead(language, reflexes),
                staff.LEngineStaffLanguage.LLanguageStaffReflex.LReflexFoldedRead(language),
                _lLiveryFacadeLanguage.LEngineTranscriptionRead(draft),
                _lLiveryFacadeLanguage.LEngineGlyphRead(language),
                _lLiveryFacadeLanguage.LEngineGlyphDivide(draft),
                staff.LEngineStaffLanguage.LLanguageStaffFrequency.LFrequencyClerkRead(entryId, fetch: false),
                _lLiveryFacadeVocabulary.LEngineParadigmScan(entryId),
                _lLiveryFacadeFanqie.LEngineFanqieDivide(entryId),
                _lLiveryFacadeScript.LEngineScriptDivide(entryId),
                _lLiveryFacadeCard.LEngineTranslationRead(draft),
                _lLiveryFacadeReference.LEngineCitationRead(draft),
                _lLiveryFacadeCard.LEngineIncomingRead(entryId),
                _lLiveryFacadeCard.LEngineEtymonRead(draft),
                staff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageFlagFind,
                staff.LEngineStaffLanguage.LLanguageStaffLanguage.LVarietyFlagScan(accent));
        }
    }

    public LLiveryLanguage LEngineLiveryRead(string language, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(localize);

        lock (_lLiveryFacadeHearth.LEngineGate)
        {
            LEngineStaff staff = LLiveryFacadeStaff;
            return LLiveryClerk.LLiveryClerkBuild(
                language,
                staff.LEngineStaffLanguage.LLanguageStaffPronunciation.LPronunciationClerkFind(
                    string.Empty, LCatalogOrder.LCatalogOrderName),
                staff.LEngineStaffLanguage.LLanguageStaffShengfu.LShengfuRuleRead(language) is null
                    ? null
                    : staff.LEngineStaffLanguage.LLanguageStaffStem,
                _lLiveryFacadeFanqie.LEngineBookCheck(language) ? staff.LEngineStaffLanguage.LLanguageStaffDiwei : null,
                _lLiveryFacadeSettings.LEngineRespellingCheck(language),
                _lLiveryFacadeHearth.LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }
}
