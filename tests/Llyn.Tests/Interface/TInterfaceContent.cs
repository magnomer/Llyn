using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LPronunciationDraft> TEntryPronunciationRead(this LEngine engine, long entryId) =>
        engine.LEngineEntry.LEngineEntryLoad(entryId)!.LEntryDraftPronunciations;

    internal static IReadOnlyList<LInflection> TEntryInflectionRead(this LEngine engine, long entryId) =>
        engine.LEngineEntry.LEngineEntryLoad(entryId)!.LEntryDraftInflections;

    internal static LEntry TEntryInflectionSave(
        this LEngine engine, long entryId, IReadOnlyList<LInflection> inflections)
    {
        LEntryDraft loaded = engine.LEngineEntry.LEngineEntryLoad(entryId)!;
        return engine.TEngineEntryUpdate(entryId, loaded with { LEntryDraftInflections = inflections });
    }

    internal static LSpeechValue? TSpeechValueFind(this LEngine engine, string language, string name) =>
        engine.LEngineVocabulary.LEngineSpeechRead(language)
            .SingleOrDefault(row => string.Equals(row.LSpeechValueName, name, StringComparison.Ordinal));

    internal static LMorphology TParadigmMorphologyRead(this LEngine engine, long entryId, string name) =>
        engine.LEngineVocabulary.LEngineParadigmShow(entryId)
            .First(slot => string.Equals(slot.LParadigmSlotMorphology.LMorphologyName, name, StringComparison.Ordinal))
            .LParadigmSlotMorphology;

    internal static IReadOnlyList<LReflexDraft> TEntryReflexRead(this LEngine engine, long entryId) =>
        engine.LEngineEntry.LEngineEntryLoad(entryId)!.LEntryDraftReflexes;

    internal static LEntry TEntryAnchorApply(
        this LEngine engine, long entryId, IReadOnlyList<long> anchors, int? position = null)
    {
        LDraft held = engine.LEngineDraft.LEngineDraftStart("Input", entryId);
        IReadOnlyList<LReflexDraft> rows = held.LDraftContent.LEntryDraftReflexes;
        for (int index = 0; index < rows.Count; index++)
        {
            if (position is int only && only != index)
            {
                continue;
            }

            long rowId = rows[index].LReflexDraftId;
            foreach (long dropped in rows[index].LReflexDraftAnchors.Except(anchors))
            {
                engine.LEngineRequest.LEngineRequestApply(
                    new LRequestReflexAnchor(held.LDraftId, rowId, dropped, false));
            }

            foreach (long added in anchors.Except(rows[index].LReflexDraftAnchors))
            {
                engine.LEngineRequest.LEngineRequestApply(new LRequestReflexAnchor(held.LDraftId, rowId, added, true));
            }
        }

        return engine.LEngineDraft.LEngineDraftCommit(held.LDraftId).LOutcomeEntry;
    }

    internal static LExample TEngineExampleCommit(this LEngine engine, long id) =>
        engine.LEngineExample.LEngineExampleCommit(id);

    internal static LDraft TEngineExampleStart(this LEngine engine, string origin, long? exampleId) =>
        engine.LEngineExample.LEngineExampleStart(origin, exampleId);

    internal static LExample TEngineExampleCreate(this LEngine engine, LExample example) =>
        engine.LEngineStaffHeld.LEngineStaffExample.LExampleClerkCreate(example);

    internal static void TEngineExampleDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineExample.LEngineExampleDelete(id, detach);
    }

    internal static LExample? TEngineExampleRead(this LEngine engine, long id) =>
        engine.LEngineExample.LEngineExampleRead(id);

    internal static void TEngineExampleUpdate(this LEngine engine, LExample example)
    {
        engine.LEngineStaffHeld.LEngineStaffExample.LExampleClerkUpdate(example);
    }

    internal static IReadOnlyList<LUsage> TEngineIncomingRead(this LEngine engine, long entryId) =>
        engine.LEngineCard.LEngineIncomingRead(entryId);

    internal static IReadOnlyList<LParadigmSlot> TEngineParadigmShow(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineParadigmShow(entryId);

    internal static bool TEngineParadigmMatch(LParadigm paradigm, string headword, string form) =>
        LParadigmClerk.LParadigmClerkMatch(paradigm, headword, form);

    internal static IReadOnlyList<string> TEngineSchemeRead(this LEngine engine, string language) =>
        engine.LEnginePronunciation.LEngineSchemeRead(language);

    internal static LGlyph? TEngineGlyphRead(this LEngine engine, string language) =>
        engine.LEngineEntry.LEngineGlyphRead(language);

    internal static LEntry TEngineGlyphResolve(this LEngine engine, string character, string language) =>
        engine.LEngineEntry.LEngineGlyphResolve(character, language);

    internal static IReadOnlyList<LMentionLabel> TEngineMentionResolve(
        this LEngine engine, string text, IReadOnlyList<LMentionDraft> mentions) =>
        engine.LEngineMention.LEngineMentionResolve(text, mentions);

    internal static IReadOnlyList<LMeaning> TEngineMeaningRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineCard.LEngineMeaningRead(ownerId, owner);

    internal static LSituation TEngineSituationCommit(this LEngine engine, long id) =>
        engine.LEngineSituation.LEngineSituationCommit(id);

    internal static LSituation TEngineSituationCreate(this LEngine engine, LSituation situation) =>
        engine.LEngineStaffHeld.LEngineStaffSituation.LSituationClerkCreate(situation);

    internal static LDraft TEngineSituationStart(this LEngine engine, string origin, long? situationId) =>
        engine.LEngineSituation.LEngineSituationStart(origin, situationId);

    internal static void TEngineSituationDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineSituation.LEngineSituationDelete(id, detach);
    }

    internal static LSituation? TEngineSituationRead(this LEngine engine, long id) =>
        engine.LEngineSituation.LEngineSituationRead(id);

    internal static void TEngineSituationUpdate(this LEngine engine, LSituation situation)
    {
        engine.LEngineStaffHeld.LEngineStaffSituation.LSituationClerkUpdate(situation);
    }

    internal static LSpeechValue? TEngineSpeechAdd(this LEngine engine, string language, string name) =>
        engine.LEngineVocabulary.LEngineSpeechAdd(language, name);

    internal static IReadOnlyList<LSpeechValue> TEngineSpeechRead(this LEngine engine, string language) =>
        engine.LEngineVocabulary.LEngineSpeechRead(language);

    internal static LSpeechValue? TEngineSpeechRead(this LEngine engine, string language, long id) =>
        engine.LEngineVocabulary.LEngineSpeechRead(language).SingleOrDefault(row => row.LSpeechValueId == id);

    internal static LTag TEngineTagCreate(this LEngine engine, string text) =>
        engine.LEngineCard.LEngineTagCreate(text);

    internal static IReadOnlyList<LTag> TEngineTagRead(this LEngine engine) =>
        engine.LEngineCard.LEngineTagRead();

    internal static LEntry TEngineTranslationCreate(this LEngine engine, string headword, string language) =>
        engine.LEngineCard.LEngineTranslationCreate(headword, language);

    internal static IReadOnlyList<LEntry> TEngineTranslationFind(
        this LEngine engine,
        string query,
        long? entryId) =>
        engine.LEngineStaffHeld.LEngineStaffTranslation.LTranslationClerkFind(query, entryId);

    internal static LEntry? TEngineTranslationResolve(this LEngine engine, string word, long? entryId) =>
        engine.LEngineCard.LEngineTranslationResolve(word, entryId);

    internal static IReadOnlyDictionary<long, int> TEngineUsageRead(this LEngine engine, LOwner owner) =>
        engine.LEngineEntry.LEngineUsageRead(owner);

    internal static IReadOnlyList<LUsage> TEngineUsageRead(this LEngine engine, long id, LOwner owner) =>
        engine.LEngineEntry.LEngineUsageRead(id, owner);
}
