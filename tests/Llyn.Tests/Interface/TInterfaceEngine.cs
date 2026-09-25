using System;
using System.Net;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LCourt? TEngineCourtFind(this LEngine engine, long ownerId, long targetId) =>
        engine.LEngineRequest.LEngineCourtFind(ownerId, targetId);

    internal static LCourt TEngineCourtStart(
        this LEngine engine,
        long ownerId,
        string origin,
        string headword,
        string language) =>
        engine.LEngineRequest.LEngineCourtStart(ownerId, origin, headword, language);

    internal static LDraft? TEngineChronicleUndo(this LEngine engine, long id) =>
        engine.LEngineRequest.LEngineChronicleUndo(id);

    internal static LDraft? TEngineChronicleRedo(this LEngine engine, long id) =>
        engine.LEngineRequest.LEngineChronicleRedo(id);

    internal static bool TEngineUndoCheck(this LEngine engine, long id) =>
        engine.LEngineRequest.LEngineUndoCheck(id);

    internal static bool TEngineRedoCheck(this LEngine engine, long id) =>
        engine.LEngineRequest.LEngineRedoCheck(id);

    internal static LCourt TEngineCourtSave(
        this LEngine engine,
        long ownerId,
        long targetId,
        string headword,
        string language) =>
        engine.LEngineRequest.LEngineCourtSave(ownerId, targetId, headword, language);

    internal static void TEngineDraftCancel(this LEngine engine, long id)
    {
        engine.LEngineDraft.LEngineDraftCancel(id);
    }

    internal static bool TEngineDraftCheck(this LEngine engine, long id) =>
        engine.LEngineDraft.LEngineDraftCheck(id);

    internal static bool TEngineDraftCheck(this LEngine engine, long id, out string? refusal) =>
        engine.LEngineDraft.LEngineDraftCheck(id, out refusal);

    internal static LEntry TEngineDraftCommit(this LEngine engine, long id) =>
        engine.LEngineDraft.LEngineDraftCommit(id).LOutcomeEntry;

    internal static LOutcome TEngineOutcomeCommit(this LEngine engine, long id) =>
        engine.LEngineDraft.LEngineDraftCommit(id);

    internal static void TEngineDraftSweep(this LEngine engine, long id)
    {
        engine.LEngineDraft.LEngineDraftSweep(id);
    }

    internal static void TEngineDraftDelete(this LEngine engine, long id)
    {
        engine.LEngineDraft.LEngineDraftDelete(id);
    }

    internal static LDraft? TEngineDraftRead(this LEngine engine, long id) =>
        engine.LEngineDraft.LEngineDraftRead(id);

    internal static IReadOnlyList<LDraft> TEngineDraftScan(this LEngine engine) =>
        engine.LEngineStaffHeld.LEngineStaffClaim.LDraftScan();

    internal static LDraft TEngineDraftStart(this LEngine engine, string origin, long? entryId) =>
        engine.LEngineDraft.LEngineDraftStart(origin, entryId);

    internal static LDraft TEngineAuthorStart(this LEngine engine, string origin, long? authorId) =>
        engine.LEngineAuthor.LEngineAuthorStart(origin, authorId);

    internal static LRevision TEngineEntryDelete(this LEngine engine, long id) =>
        engine.LEngineEntry.LEngineEntryDelete(id);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LTag tag) =>
        engine.LEngineEntry.LEngineEntryFind(tag);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LRegister register) =>
        engine.LEngineEntry.LEngineEntryFind(register);

    internal static LRegister TEngineRegisterCreate(this LEngine engine, string name) =>
        engine.LEngineCard.LEngineRegisterCreate(name);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, string query) =>
        engine.LEngineStaffHeld.LEngineStaffEntry.LEntryClerkFind(query);

    internal static LEntryDraft? TEngineEntryLoad(this LEngine engine, long id) =>
        engine.LEngineEntry.LEngineEntryLoad(id);

    internal static LEntry? TEngineEntryRead(this LEngine engine, long id) =>
        engine.LEngineEntry.LEngineEntryRead(id);

    internal static LEntry TEngineEntrySave(this LEngine engine, LEntryDraft draft) =>
        engine.TEngineEntryCommit(null, draft);

    internal static LEntry TEngineEntryUpdate(this LEngine engine, long id, LEntryDraft draft) =>
        engine.TEngineEntryCommit(id, draft);

    private static LEntry TEngineEntryCommit(this LEngine engine, long? id, LEntryDraft draft)
    {
        LDraft started = engine.LEngineDraft.LEngineDraftStart("test", id);
        LEngineStaff staff = engine.LEngineStaffHeld;
        staff.LEngineStaffClaim.LDraftSave(
            started with { LDraftContent = staff.LEngineStaffDraft.LDraftClerkNormalize(draft) });
        return engine.LEngineDraft.LEngineDraftCommit(started.LDraftId).LOutcomeEntry;
    }

    internal static bool TEngineFavoriteCheck(this LEngine engine, long entryId) =>
        engine.LEngineVista.LEngineFavoriteCheck(entryId);

    internal static void TEngineFavoriteDelete(this LEngine engine, long entryId)
    {
        engine.LEngineVista.LEngineFavoriteDelete(entryId);
    }

    internal static void TEngineFavoriteSave(this LEngine engine, long entryId)
    {
        engine.LEngineVista.LEngineFavoriteSave(entryId);
    }

    internal static int TEngineGraspRead(this LEngine engine, long entryId) =>
        engine.LEngineEntry.LEngineGraspRead(entryId);

    internal static void TEngineGraspSave(this LEngine engine, long entryId, int grasp)
    {
        engine.LEngineEntry.LEngineGraspSave(entryId, grasp);
    }

    internal static LMentionResult TEngineMentionFind(this LEngine engine, long exampleId, int offset) =>
        engine.LEngineMention.LEngineMentionFind(exampleId, offset);

    internal static LMentionResult TEngineMentionFind(
        this LEngine engine,
        string text,
        string language,
        int offset,
        IReadOnlyList<LMention> mentions) =>
        engine.LEngineMention.LEngineMentionFind(text, language, offset, mentions);

    internal static void TEngineMarkupExport(this LEngine engine, IReadOnlyList<long> ids, string path)
    {
        engine.LEngineStaffHeld.LEngineStaffMarkup.LMarkupClerkExport(ids, path);
    }

    internal static Task TEnginePortraitExport(
        this LEngine engine,
        long entryId,
        string path,
        LPortraitFormat format,
        LPortraitLabel label) =>
        engine.LEnginePortrait.LEnginePortraitExport(entryId, path, format, label);

    internal static LPortraitPage TEnginePortraitRead(this LEngine engine, long entryId, LPortraitLabel label) =>
        engine.LEnginePortrait.LEnginePortraitRead(entryId, label);

    internal static LPortraitPage TEnginePortraitRead(
        this LEngine engine, long id, LOwner owner, LPortraitLegend legend) =>
        engine.LEnginePortrait.LEnginePortraitRead(id, owner, legend);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, long entryId, LPortraitLabel label, LPressTicket ticket) =>
        engine.LEnginePortrait.LEnginePortraitPrint(entryId, label, ticket);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket) =>
        engine.LEnginePortrait.LEnginePortraitPrint(id, owner, legend, ticket);

    internal static IReadOnlyList<LDraft> TEngineLeftoverRead(this LEngine engine)
    {
        LClaimClerk claims = engine.LEngineStaffHeld.LEngineStaffClaim;
        return
        [
            .. claims.LDraftScan().Where(draft =>
                !claims.LClaimClerkHeld.Contains(draft.LDraftId)
                && engine.LEngineDraft.LEngineDraftCheck(draft.LDraftId)
                && !claims.LClaimForeignCheck(draft.LDraftId)),
        ];
    }

    internal static void TEngineLeftoverSweep(this LEngine engine)
    {
        engine.LEngineDraft.LEngineLeftoverSweep();
    }

    internal static void TEngineRecordingSweep(this LEngine engine)
    {
        engine.LEnginePronunciation.LEngineRecordingSweep();
    }

    internal static string TEngineWorkspaceRead(this LEngine engine) =>
        engine.LEngineWorkspaceRead();

    internal static void TEngineWorkspaceOpen(this LEngine engine, string path)
    {
        engine.LEngineRigApply(LRigFactory.LRigFactoryBuild(
            path,
            TPronunciationHelper.TSourceClientCreate(string.Empty, HttpStatusCode.NotFound),
            new LUsherFile(),
            new TPress(),
            new TPhonographFake())
            with { LRigClock = new TClockFake() });
    }

    internal static void TEngineRigApply(this LEngine engine, LRig rig)
    {
        engine.LEngineRigApply(rig);
    }

    internal static LDoctorRescue TEngineRescueRead(this LEngine engine) =>
        engine.LEngineRescueRead();

    internal static LEstablishment TEngineEstablishmentRead(this LEngine engine) =>
        engine.LEngineEntry.LEngineEstablishmentRead();

    internal static void TEngineObserverAttach(this LEngine engine, Action<LBulletin> observer)
    {
        engine.LEngineObserverAttach(observer);
    }

    internal static void TEngineObserverDetach(this LEngine engine, Action<LBulletin> observer)
    {
        engine.LEngineObserverDetach(observer);
    }

    internal static LDraft TEngineRequestApply(this LEngine engine, LRequest request) =>
        engine.LEngineRequest.LEngineRequestApply(request);

    internal static LMarkupCargo TEngineMarkupRead(this LEngine engine, string path) =>
        engine.LEngineMarkup.LEngineMarkupRead(path);

    internal static IReadOnlyList<LEntry> TEngineMarkupFind(this LEngine engine, LMarkupEntry entry) =>
        engine.LEngineMarkup.LEngineMarkupFind(entry);

    internal static TMarkupOutcome TEngineMarkupImport(
        this LEngine engine, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        List<HashSet<long>> held = [];
        foreach (LMarkupEntry entry in cargo.LMarkupCargoEntry)
        {
            held.Add([.. engine.TEngineMarkupFind(entry).Select(static found => found.LEntryId)]);
        }

        LMarkupOutcome outcome = engine.LEngineMarkup.LEngineMarkupImport(cargo, intakes);

        HashSet<long> taken = [];
        List<LEntry> stored = [];
        for (int index = 0; index < cargo.LMarkupCargoEntry.Count; index++)
        {
            long target = intakes.Single(intake => intake.LMarkupIntakeIndex == index).LMarkupIntakeTarget;
            long id = target > 0
                ? target
                : engine.TEngineMarkupFind(cargo.LMarkupCargoEntry[index])
                    .Select(static found => found.LEntryId)
                    .Where(found => !held[index].Contains(found) && !taken.Contains(found))
                    .Min();
            taken.Add(id);
            stored.Add(engine.TEngineEntryRead(id)!);
        }

        return new TMarkupOutcome(stored, outcome.LMarkupOutcomeOmission);
    }

    internal static Uri? TEngineLocationResolve(this LEngine engine, string location) =>
        engine.LEngineStaffHeld.LEngineStaffTrail.LTrailClerkResolve(location);

    internal static Uri? TEngineLocationRead(this LEngine engine, string location) =>
        engine.LEngineWorkspace.LEngineLocationRead(location);

    internal static async Task<IReadOnlyList<LEnsignRow>> TEngineEnsignLoad(this LEngine engine)
    {
        List<LEnsignRow> kept = [];
        await engine.LEngineLanguage.LEngineEnsignLoad((rows, _) => () => kept.AddRange(rows));
        return kept;
    }

    internal static async Task<IReadOnlyList<LEnsignRow>> TEngineEnsignLoad(
        this LEngine engine, string language, IEnumerable<string> varieties)
    {
        List<LEnsignRow> kept = [];
        await engine.LEngineLanguage.LEngineEnsignLoad(language, varieties, (rows, _) => () => kept.AddRange(rows));
        return kept;
    }

    internal static string TEngineWorkspaceFormat(this LEngine engine) =>
        engine.LEngineWorkspaceFormat();

    internal static Task<LMarkupCargo> TEngineMarkupStart(this LEngine engine, string path) =>
        engine.LEngineMarkup.LEngineMarkupStart(path);

    internal static Task<LMarkupOutcome> TEngineMarkupStart(
        this LEngine engine, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes) =>
        engine.LEngineMarkup.LEngineMarkupStart(cargo, intakes);

    internal static long? TEngineRevisionRead(this LEngine engine) =>
        engine.LEngineWorkspace.LEngineStateRead().LWorkspaceStateRevision;

    internal static LWorkspaceState TEngineStateRead(this LEngine engine) =>
        engine.LEngineWorkspace.LEngineStateRead();

    internal static LSettings TEngineSettingsRead(this LEngine engine) =>
        engine.LEngineSettings.LEngineSettingsRead();

    internal static string TEngineLocalizationRead(this LEngine engine)
    {
        return engine.LEngineSettings.LEngineLocalizationRead();
    }

    internal static void TEngineLocalizationSave(this LEngine engine, string language)
    {
        engine.LEngineSettings.LEngineLocalizationSave(language);
    }

    internal static void TEngineEpithetSave(this LEngine engine, bool epithet) =>
        engine.LEngineSettings.LEngineEpithetSave(epithet);

    internal static string TEngineEpithetRead(this LEngine engine, long entryId) =>
        engine.LEngineEntry.LEngineEpithetRead(entryId);

    internal static void TEngineRespellingSave(this LEngine engine, bool respelled)
    {
        engine.LEngineSettings.LEngineRespellingSave(respelled);
    }

    internal static void TEngineTallySave(this LEngine engine, bool respelled)
    {
        engine.LEngineSettings.LEngineTallySave(respelled);
    }

    internal static bool TEngineRespellingCheck(this LEngine engine, string language) =>
        engine.LEngineSettings.LEngineRespellingCheck(language);

    internal static bool TEnginePhonemicCheck(this LEngine engine, string language) =>
        engine.LEngineSettings.LEnginePhonemicCheck(language);

    internal static void TEngineFrequencySave(this LEngine engine, bool frequency)
    {
        engine.LEngineSettings.LEngineFrequencySave(frequency);
    }

    internal static void TEngineMorphologySave(this LEngine engine, bool morphology)
    {
        engine.LEngineSettings.LEngineMorphologySave(morphology);
    }

    internal static IReadOnlyList<LFrequency> TEngineFrequencyRead(this LEngine engine, long entryId) =>
        engine.LEnginePronunciation.LEngineFrequencyRead(entryId);

    internal static void TEngineFrequencyStart(this LEngine engine, long entryId)
    {
        engine.LEnginePronunciation.LEngineFrequencyStart(entryId);
    }

    internal static void TEngineInflectionStart(this LEngine engine, long entryId)
    {
        engine.LEngineVocabulary.LEngineInflectionStart(entryId);
    }

    internal static bool TEngineInflectionCheck(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineInflectionCheck(entryId);

    internal static IReadOnlyList<LScriptImage> TEngineScriptRead(this LEngine engine, long entryId) =>
        engine.LEngineStaffHeld.LEngineStaffScript.LScriptClerkRead(entryId);

    internal static bool TEngineScriptCheck(this LEngine engine, long entryId) =>
        engine.LEngineLanguage.LEngineScriptCheck(entryId);

    internal static void TEngineScriptStart(this LEngine engine, long entryId) =>
        engine.LEngineLanguage.LEngineScriptStart(entryId);

    internal static void TEngineScriptRebuild(this LEngine engine, long entryId) =>
        engine.LEngineLanguage.LEngineScriptRebuild(entryId);

    internal static IReadOnlyList<LReflexRule> TEngineReflexRead(this LEngine engine, string language) =>
        engine.LEngineReflex.LEngineReflexRead(language);

    internal static bool TEngineReflexCheck(this LEngine engine, long entryId) =>
        engine.LEngineReflex.LEngineReflexCheck(entryId);

    internal static void TEngineReflexStart(this LEngine engine, long entryId) =>
        engine.LEngineReflex.LEngineReflexStart(entryId);

    internal static void TEngineReflexRebuild(this LEngine engine, long entryId) =>
        engine.LEngineReflex.LEngineReflexRebuild(entryId);

    internal static IReadOnlyList<LFanqieRow> TEngineFanqieRead(this LEngine engine, long entryId) =>
        engine.LEngineStaffHeld.LEngineStaffFanqie.LFanqieClerkRead(entryId);

    internal static IReadOnlyList<LFanqieRow> TEngineFanqieRead(this LEngine engine, LDiwei diwei) =>
        engine.LEngineStaffHeld.LEngineStaffDiwei.LDiweiFanqieRead(diwei);

    internal static LDiwei? TEngineDiweiFind(this LEngine engine, string language, string kind, string key) =>
        engine.LEngineFanqie.LEngineDiweiFind(language, kind, key);

    internal static LStem? TEngineStemFind(this LEngine engine, string language, string key) =>
        engine.LEngineStem.LEngineStemFind(language, key);

    internal static string? TEngineStemFind(this LEngine engine) =>
        engine.LEngineStem.LEngineStemFind();

    internal static LStemPage TEngineStemResolve(this LEngine engine, long? id) =>
        engine.LEngineStem.LEngineStemResolve(id);

    internal static IReadOnlyList<LVistaRow> TEngineKindredFind(
        this LEngine engine, string language, IReadOnlyList<long> stemIds, string query) =>
        engine.LEngineStem.LEngineKindredFind(language, stemIds, query);

    internal static IReadOnlyList<LTally> TEngineTallyRead(this LEngine engine, LDiwei diwei) =>
        engine.LEngineStaffHeld.LEngineStaffDiwei.LTallyRead(diwei);

    internal static bool TEngineFanqieCheck(this LEngine engine, long entryId) =>
        engine.LEngineFanqie.LEngineFanqieCheck(entryId);

    internal static void TEngineFanqieStart(this LEngine engine, long entryId) =>
        engine.LEngineFanqie.LEngineFanqieStart(entryId);

    internal static IReadOnlyList<LFanqieGroup> TEngineFanqieDivide(this LEngine engine, long entryId) =>
        engine.LEngineFanqie.LEngineFanqieDivide(entryId);
}
