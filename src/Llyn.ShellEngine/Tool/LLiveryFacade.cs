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
                staff.LEngineStaffReflex.LReflexFoldedRead(language),
                engine.LEngineLanguage.LEngineTranscriptionRead(draft),
                engine.LEngineLanguage.LEngineGlyphRead(language),
                engine.LEngineLanguage.LEngineGlyphDivide(draft),
                staff.LEngineStaffFrequency.LFrequencyClerkRead(entryId, fetch: false),
                engine.LEngineVocabulary.LEngineParadigmScan(entryId),
                engine.LEngineFanqie.LEngineFanqieDivide(entryId),
                engine.LEngineLanguage.LEngineScriptDivide(entryId),
                engine.LEngineCard.LEngineTranslationRead(draft),
                engine.LEngineReference.LEngineCitationRead(draft),
                engine.LEngineCard.LEngineIncomingRead(entryId),
                engine.LEngineCard.LEngineEtymonRead(draft),
                staff.LEngineStaffLanguage.LLanguageFlagFind,
                staff.LEngineStaffLanguage.LVarietyFlagScan(accent));
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
                staff.LEngineStaffPronunciation.LPronunciationClerkFind(
                    string.Empty, LCatalogOrder.LCatalogOrderName),
                staff.LEngineStaffShengfu.LShengfuRuleRead(language) is null ? null : staff.LEngineStaffStem,
                engine.LEngineFanqie.LEngineBookCheck(language) ? staff.LEngineStaffDiwei : null,
                engine.LEngineSettings.LEngineRespellingCheck(language),
                engine.LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }
}
