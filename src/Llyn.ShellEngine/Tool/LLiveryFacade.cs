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

    public LLiveryPage? LEngineLiveryRead(long entryId, bool pending)
    {
        lock (_lLiveryFacadeHearth.LEngineGate)
        {
            if (_lLiveryFacadeEntry.LEngineEntryLoad(entryId) is not LEntryDraft draft)
            {
                return null;
            }

            LEngineStaff staff = LLiveryFacadeStaff;
            string language = draft.LEntryDraftLanguage;
            (_, string created, string updated) = _lLiveryFacadeEntry.LEngineStampRead(entryId);
            LAccentSheet accent = _lLiveryFacadeLanguage.LEngineAccentRead(draft);
            bool marked = pending && _lLiveryFacadeVocabulary.LEngineInflectionCheck(entryId);
            bool enabled = _lLiveryFacadeSettings.LEngineMorphologyCheck();
            Dictionary<LFontRole, LFont> font = new Dictionary<LFontRole, LFont>
            {
                [LFontRole.LFontRoleHeadword] = _lLiveryFacadeLanguage.LEngineFontRead(
                    language, LFontRole.LFontRoleHeadword),
                [LFontRole.LFontRoleGlyph] = _lLiveryFacadeLanguage.LEngineFontRead(
                    language, LFontRole.LFontRoleGlyph),
            };
            HashSet<LFoldBox> opened = Enum.GetValues<LFoldBox>()
                .Where(box => _lLiveryFacadeReflex.LEngineBoxCheck(entryId, box))
                .ToHashSet();
            return LLiveryClerk.LLiveryClerkBuild(
                draft,
                _lLiveryFacadeCatalog.LEngineFavoriteCheck(entryId),
                _lLiveryFacadeEntry.LEngineGraspRead(entryId),
                created,
                updated,
                accent,
                _lLiveryFacadeReflex.LEngineGuiseRead(language, draft.LEntryDraftReflexes),
                staff.LEngineStaffLanguage.LLanguageStaffReflex.LReflexFoldedRead(language),
                staff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkRead(entryId),
                _lLiveryFacadeLanguage.LEngineTranscriptionRead(draft),
                _lLiveryFacadeLanguage.LEngineGlyphRead(language),
                _lLiveryFacadeLanguage.LEngineGlyphDivide(draft),
                staff.LEngineStaffLanguage.LLanguageStaffFrequency.LFrequencyClerkRead(entryId, fetch: false),
                _lLiveryFacadeVocabulary.LEngineParadigmScan(entryId),
                marked,
                enabled,
                _lLiveryFacadeVocabulary.LEngineInflectionRead(entryId, marked, enabled, false),
                _lLiveryFacadeFanqie.LEngineFanqieDivide(entryId),
                _lLiveryFacadeScript.LEngineScriptDivide(entryId),
                _lLiveryFacadeCard.LEngineTranslationRead(draft),
                _lLiveryFacadeReference.LEngineCitationRead(draft),
                _lLiveryFacadeCard.LEngineIncomingRead(entryId),
                staff.LEngineStaffEntry.LEntryStaffLineage.LLineageClerkRead(entryId, draft.LEntryDraftAncestors),
                staff.LEngineStaffEntry.LEntryStaffAncestor.LAncestorRelationRead(),
                staff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageFlagFind,
                staff.LEngineStaffLanguage.LLanguageStaffLanguage.LVarietyFlagScan(accent),
                font,
                opened,
                staff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkFormat(
                    entryId, draft.LEntryDraftHeadword));
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
                staff.LEngineStaffLanguage.LLanguageStaffShengfu.LShengfuRuleRead(language) is null
                    ? null
                    : staff.LEngineStaffLanguage.LLanguageStaffStem,
                _lLiveryFacadeFanqie.LEngineBookCheck(language) ? staff.LEngineStaffLanguage.LLanguageStaffDiwei : null,
                _lLiveryFacadeSettings.LEngineRespellingCheck(language),
                _lLiveryFacadeHearth.LEngineSettingsHeld.LSettingsRespelled,
                _lLiveryFacadeHearth.LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }
}
