using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LLiveryFacade
{
    private readonly LEngine _lLiveryFacadeEngine;

    public LLiveryFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lLiveryFacadeEngine = engine;
    }

    private LEngineStaff LLiveryFacadeStaff => _lLiveryFacadeEngine.LEngineStaffHeld;

    public LLiveryPage? LEngineLiveryRead(long entryId)
    {
        LEngine engine = _lLiveryFacadeEngine;
        lock (engine.LEngineGate)
        {
            if (engine.LEngineEntry.LEngineEntryLoad(entryId) is not LEntryDraft draft)
            {
                return null;
            }

            LEngineStaff staff = LLiveryFacadeStaff;
            string language = draft.LEntryDraftLanguage;
            IReadOnlyList<string> reflexes =
                [.. draft.LEntryDraftReflexes.Select(static row => row.LReflexDraftLanguage)];
            (_, string created, string updated) = engine.LEngineEntry.LEngineStampRead(entryId);
            LAccentSheet accent = engine.LEngineLanguage.LEngineAccentRead(draft);
            return LLiveryClerk.LLiveryClerkBuild(
                draft,
                engine.LEngineVista.LEngineFavoriteCheck(entryId),
                engine.LEngineEntry.LEngineGraspRead(entryId),
                created,
                updated,
                accent,
                engine.LEngineReflex.LEngineGuiseRead(language, reflexes),
                staff.LEngineStaffLanguage.LLanguageStaffReflex.LReflexFoldedRead(language),
                engine.LEngineLanguage.LEngineTranscriptionRead(draft),
                engine.LEngineLanguage.LEngineGlyphRead(language),
                engine.LEngineLanguage.LEngineGlyphDivide(draft),
                staff.LEngineStaffLanguage.LLanguageStaffFrequency.LFrequencyClerkRead(entryId, fetch: false),
                engine.LEngineVocabulary.LEngineParadigmScan(entryId),
                engine.LEngineFanqie.LEngineFanqieDivide(entryId),
                engine.LEngineLanguage.LEngineScriptDivide(entryId),
                engine.LEngineCard.LEngineTranslationRead(draft),
                engine.LEngineReference.LEngineCitationRead(draft),
                engine.LEngineCard.LEngineIncomingRead(entryId),
                engine.LEngineCard.LEngineEtymonRead(draft),
                staff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageFlagFind,
                staff.LEngineStaffLanguage.LLanguageStaffLanguage.LVarietyFlagScan(accent));
        }
    }

    public LLiveryLanguage LEngineLiveryRead(string language, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(localize);

        LEngine engine = _lLiveryFacadeEngine;
        lock (engine.LEngineGate)
        {
            LEngineStaff staff = LLiveryFacadeStaff;
            return LLiveryClerk.LLiveryClerkBuild(
                language,
                staff.LEngineStaffLanguage.LLanguageStaffPronunciation.LPronunciationClerkFind(
                    string.Empty, LCatalogOrder.LCatalogOrderName),
                staff.LEngineStaffLanguage.LLanguageStaffShengfu.LShengfuRuleRead(language) is null
                    ? null
                    : staff.LEngineStaffLanguage.LLanguageStaffStem,
                engine.LEngineFanqie.LEngineBookCheck(language) ? staff.LEngineStaffLanguage.LLanguageStaffDiwei : null,
                engine.LEngineSettings.LEngineRespellingCheck(language),
                engine.LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }
}
