using System;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LRevisionChange> TEngineChangeRead(
        this LEngine engine,
        long revisionId) =>
        engine.LEngineChangeRead(revisionId);

    internal static LCourt? TEngineCourtFind(this LEngine engine, long ownerId, long targetId) =>
        engine.LEngineCourtFind(ownerId, targetId);

    internal static LCourt TEngineCourtStart(
        this LEngine engine,
        long ownerId,
        string origin,
        string headword,
        string language) =>
        engine.LEngineCourtStart(ownerId, origin, headword, language);

    internal static long TEngineCardCreate(this LEngine engine) =>
        engine.LEngineCardCreate();

    internal static IReadOnlyList<LCardDraft> TEngineDraftNormalize(
        this LEngine engine,
        long id,
        bool collocation) =>
        engine.LEngineDraftNormalize(id, collocation);

    internal static LCourt TEngineCourtSave(
        this LEngine engine,
        long ownerId,
        long targetId,
        string headword,
        string language) =>
        engine.LEngineCourtSave(ownerId, targetId, headword, language);

    internal static void TEngineDraftCancel(this LEngine engine, long id)
    {
        engine.LEngineDraftCancel(id);
    }

    internal static bool TEngineDraftCheck(this LEngine engine, long id) =>
        engine.LEngineDraftCheck(id);

    internal static bool TEngineDraftCheck(this LEngine engine, long id, out string? refusal) =>
        engine.LEngineDraftCheck(id, out refusal);

    internal static LEntry TEngineDraftCommit(this LEngine engine, long id) =>
        engine.LEngineDraftCommit(id).LOutcomeEntry;

    internal static LOutcome TEngineOutcomeCommit(this LEngine engine, long id) =>
        engine.LEngineDraftCommit(id);

    internal static void TEngineDraftSweep(this LEngine engine, long id)
    {
        engine.LEngineDraftSweep(id);
    }

    internal static void TEngineDraftDelete(this LEngine engine, long id)
    {
        engine.LEngineDraftDelete(id);
    }

    internal static IReadOnlyList<LCardDraft> TEngineDraftMove(
        this LEngine engine,
        long id,
        bool collocation,
        int from,
        int target) =>
        engine.LEngineDraftMove(id, collocation, from, target);

    internal static LDraft? TEngineDraftRead(this LEngine engine, long id) =>
        engine.LEngineDraftRead(id);

    internal static IReadOnlyList<LDraft> TEngineDraftScan(this LEngine engine) =>
        engine.LEngineDraftScan();

    internal static LDraft TEngineDraftStart(this LEngine engine, string origin, long? entryId) =>
        engine.LEngineDraftStart(origin, entryId);

    internal static LRevision TEngineEntryDelete(this LEngine engine, long id) =>
        engine.LEngineEntryDelete(id);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LTag tag) =>
        engine.LEngineEntryFind(tag);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LRegister register) =>
        engine.LEngineEntryFind(register);

    internal static LRegister TEngineRegisterCreate(this LEngine engine, string name) =>
        engine.LEngineRegisterCreate(name);

    internal static void TEngineRegisterChange(this LEngine engine, long id, string renamed) =>
        engine.LEngineRegisterChange(id, renamed);

    internal static void TEngineRegisterDelete(this LEngine engine, long id) =>
        engine.LEngineRegisterDelete(id);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, string query) =>
        engine.LEngineEntryFind(query);

    internal static LEntryDraft? TEngineEntryLoad(this LEngine engine, long id) =>
        engine.LEngineEntryLoad(id);

    internal static LEntry? TEngineEntryRead(this LEngine engine, long id) =>
        engine.LEngineEntryRead(id);

    internal static LEntry TEngineEntrySave(this LEngine engine, LEntryDraft draft) =>
        engine.LEngineEntrySave(draft);

    internal static LEntry TEngineEntryUpdate(this LEngine engine, long id, LEntryDraft draft) =>
        engine.LEngineEntryUpdate(id, draft);

    internal static bool TEngineFavoriteCheck(this LEngine engine, long entryId) =>
        engine.LEngineFavoriteCheck(entryId);

    internal static void TEngineFavoriteDelete(this LEngine engine, long entryId)
    {
        engine.LEngineFavoriteDelete(entryId);
    }

    internal static IReadOnlyList<LFavorite> TEngineFavoriteFind(this LEngine engine, string query) =>
        engine.LEngineFavoriteFind(query);

    internal static void TEngineFavoriteSave(this LEngine engine, long entryId)
    {
        engine.LEngineFavoriteSave(entryId);
    }

    internal static int TEngineGraspRead(this LEngine engine, long entryId) =>
        engine.LEngineGraspRead(entryId);

    internal static void TEngineGraspSave(this LEngine engine, long entryId, int grasp)
    {
        engine.LEngineGraspSave(entryId, grasp);
    }

    internal static LMentionResult TEngineMentionFind(this LEngine engine, long exampleId, int offset) =>
        engine.LEngineMentionFind(exampleId, offset);

    internal static LMentionResult TEngineMentionFind(
        this LEngine engine,
        string text,
        string language,
        int offset,
        IReadOnlyList<LMention> mentions) =>
        engine.LEngineMentionFind(text, language, offset, mentions);

    internal static void TEngineMarkupExport(this LEngine engine, IReadOnlyList<long> ids, string path)
    {
        engine.LEngineMarkupExport(ids, path);
    }

    internal static Task TEnginePortraitExport(
        this LEngine engine,
        long entryId,
        string path,
        LPortraitFormat format,
        LPortraitLabel label) =>
        engine.LEnginePortraitExport(entryId, path, format, label);

    internal static LPortraitPage TEnginePortraitRead(
        this LEngine engine, long id, LOwner owner, LPortraitLegend legend) =>
        engine.LEnginePortraitRead(id, owner, legend);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, long entryId, LPortraitLabel label, LPressTicket ticket) =>
        engine.LEnginePortraitPrint(entryId, label, ticket);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket) =>
        engine.LEnginePortraitPrint(id, owner, legend, ticket);

    internal static void TEnginePressApply(this LEngine engine, LPress press)
    {
        engine.LEnginePressApply(press);
    }

    internal static IReadOnlyList<LDraft> TEngineLeftoverRead(this LEngine engine) =>
        engine.LEngineLeftoverRead();

    internal static void TEngineLeftoverSweep(this LEngine engine)
    {
        engine.LEngineLeftoverSweep();
    }

    internal static string TEngineWorkspaceRead(this LEngine engine) =>
        engine.LEngineWorkspaceRead();

    internal static void TEngineWorkspaceOpen(this LEngine engine, string path)
    {
        engine.LEngineWorkspaceOpen(path);
    }

    internal static void TEngineObserverAttach(this LEngine engine, LObserver observer)
    {
        engine.LEngineObserverAttach(observer);
    }

    internal static LDraft TEngineRequestApply(this LEngine engine, LRequest request) =>
        engine.LEngineRequestApply(request);

    internal static LMarkupCargo TEngineMarkupRead(this LEngine engine, string path) =>
        engine.LEngineMarkupRead(path);

    internal static IReadOnlyList<LEntry> TEngineMarkupFind(this LEngine engine, LMarkupEntry entry) =>
        engine.LEngineMarkupFind(entry);

    internal static LMarkupOutcome TEngineMarkupImport(
        this LEngine engine, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes) =>
        engine.LEngineMarkupImport(cargo, intakes);

    internal static Uri? TEngineLocationResolve(this LEngine engine, string location) =>
        engine.LEngineLocationResolve(location);

    internal static LRevision? TEngineRevisionRead(this LEngine engine) =>
        engine.LEngineRevisionRead();

    internal static LWorkspaceState TEngineStateRead(this LEngine engine) =>
        engine.LEngineStateRead();

    internal static LSettings TEngineSettingsRead(this LEngine engine) =>
        engine.LEngineSettingsRead();

    internal static void TEngineLocalizationSave(this LEngine engine, string language)
    {
        engine.LEngineLocalizationSave(language);
    }

    internal static void TEngineWindowSave(this LEngine engine, LWindowState window)
    {
        engine.LEngineWindowSave(window);
    }

    internal static void TEngineEpithetSave(this LEngine engine, bool epithet) =>
        engine.LEngineEpithetSave(epithet);

    internal static string TEngineEpithetRead(this LEngine engine, long entryId) =>
        engine.LEngineEpithetRead(entryId);

    internal static void TEngineRespellingSave(this LEngine engine, bool respelled)
    {
        engine.LEngineRespellingSave(respelled);
    }

    internal static bool TEngineRespellingCheck(this LEngine engine, string language) =>
        engine.LEngineRespellingCheck(language);

    internal static bool TEnginePhonemicCheck(this LEngine engine, string language) =>
        engine.LEnginePhonemicCheck(language);

    internal static void TEngineFrequencySave(this LEngine engine, bool frequency)
    {
        engine.LEngineFrequencySave(frequency);
    }

    internal static void TEngineMorphologySave(this LEngine engine, bool morphology)
    {
        engine.LEngineMorphologySave(morphology);
    }

    internal static void TEngineLayoutSave(this LEngine engine, params LLayout[] layout)
    {
        engine.LEngineLayoutSave(layout);
    }

    internal static void TEngineLayoutReset(this LEngine engine)
    {
        engine.LEngineLayoutReset();
    }

    internal static void TEngineLinkedSave(this LEngine engine, bool linked)
    {
        engine.LEngineLinkedSave(linked);
    }

    internal static Task<LFrequency?> TEngineFrequencyFind(
        this LEngine engine,
        string word,
        string language,
        CancellationToken cancellation) =>
        engine.LEngineFrequencyFind(word, language, cancellation);

    internal static string? TEngineBandResolve(this LEngine engine, string language, string raw) =>
        engine.LEngineBandResolve(language, raw);

    internal static LFrequency? TEngineFrequencyRead(this LEngine engine, long entryId) =>
        engine.LEngineFrequencyRead(entryId);

    internal static void TEngineFrequencyStart(this LEngine engine, long entryId)
    {
        engine.LEngineFrequencyStart(entryId);
    }

    internal static void TEngineInflectionStart(this LEngine engine, long entryId)
    {
        engine.LEngineInflectionStart(entryId);
    }

    internal static bool TEngineInflectionCheck(this LEngine engine, long entryId) =>
        engine.LEngineInflectionCheck(entryId);

    internal static IReadOnlyList<LScriptImage> TEngineScriptRead(this LEngine engine, long entryId) =>
        engine.LEngineScriptRead(entryId);

    internal static bool TEngineScriptCheck(this LEngine engine, long entryId) =>
        engine.LEngineScriptCheck(entryId);

    internal static void TEngineScriptStart(this LEngine engine, long entryId) =>
        engine.LEngineScriptStart(entryId);

    internal static Task<IReadOnlyList<LScriptImage>> TEngineScriptFind(
        this LEngine engine,
        string character,
        string language,
        CancellationToken cancellation) =>
        engine.LEngineScriptFind(character, language, cancellation);

    internal static IReadOnlyList<LReflex> TEngineReflexRead(this LEngine engine, long entryId) =>
        engine.LEngineReflexRead(entryId);

    internal static IReadOnlyList<LReflexRule> TEngineReflexRead(this LEngine engine, string language) =>
        engine.LEngineReflexRead(language);

    internal static IReadOnlyList<LReflex> TEngineReflexSet(
        this LEngine engine, long entryId, IReadOnlyList<LReflex> reflexes) =>
        engine.LEngineReflexSet(entryId, reflexes);

    internal static bool TEngineReflexCheck(this LEngine engine, long entryId) =>
        engine.LEngineReflexCheck(entryId);

    internal static void TEngineReflexStart(this LEngine engine, long entryId) =>
        engine.LEngineReflexStart(entryId);

    internal static void TEngineReflexRebuild(this LEngine engine, long entryId) =>
        engine.LEngineReflexRebuild(entryId);

    internal static Task<IReadOnlyList<LReflexDraft>> TEngineReflexFind(
        this LEngine engine,
        string headword,
        string language,
        CancellationToken cancellation) =>
        engine.LEngineReflexFind(headword, language, cancellation);

    internal static IReadOnlyList<LFanqieRow> TEngineFanqieRead(this LEngine engine, long entryId) =>
        engine.LEngineFanqieRead(entryId);

    internal static LDiwei? TEngineDiweiFind(this LEngine engine, string language, string kind, string key) =>
        engine.LEngineDiweiFind(language, kind, key);

    internal static IReadOnlyList<LTally> TEngineTallyRead(this LEngine engine, LDiwei diwei) =>
        engine.LEngineTallyRead(diwei);

    internal static bool TEngineFanqieCheck(this LEngine engine, long entryId) =>
        engine.LEngineFanqieCheck(entryId);

    internal static void TEngineFanqieStart(this LEngine engine, long entryId) =>
        engine.LEngineFanqieStart(entryId);

    internal static Task<IReadOnlyList<LFanqieRow>> TEngineFanqieFind(
        this LEngine engine,
        string character,
        string language,
        CancellationToken cancellation) =>
        engine.LEngineFanqieFind(character, language, cancellation);
}
