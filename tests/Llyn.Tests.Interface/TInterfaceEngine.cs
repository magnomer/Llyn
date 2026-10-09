using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
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
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffClaim.LClaimStaffClaim.LDraftScan();

    internal static LDraft TEngineDraftStart(this LEngine engine, string origin, long? entryId) =>
        engine.LEngineDraft.LEngineDraftStart(origin, entryId);

    internal static void TEngineDraftSave(this LEngine engine, LDraft draft)
    {
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffClaim.LClaimStaffClaim.LDraftSave(draft);
    }

    internal static LDraft TEngineAuthorStart(this LEngine engine, string origin, long? authorId) =>
        engine.LEngineAuthor.LEngineAuthorStart(origin, authorId);

    internal static LRevision TEngineEntryDelete(this LEngine engine, long id) =>
        engine.LEngineEntry.LEngineEntryDelete(id);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LTag tag) =>
        engine.LEngineEntry.LEngineEntryFind(LSubject.LSubjectTag, tag.LTagId);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LRegister register) =>
        engine.LEngineEntry.LEngineEntryFind(LSubject.LSubjectRegister, register.LRegisterId);

    internal static LRegister TEngineRegisterCreate(this LEngine engine, string name) =>
        engine.LEngineCatalog.LEngineRegisterCreate(name);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, string query) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffEntry.LEntryStaffQuery.LEntryFind(query);

    internal static LEntryDraft? TEngineEntryLoad(this LEngine engine, long id) =>
        engine.LEngineEntry.LEngineEntryLoad(id);

    internal static LEntry? TEngineEntryRead(this LEngine engine, long id) =>
        engine.LEngineEntry.LEngineEntryRead(id);

    internal static (bool, string, string) TEngineStampRead(this LEngine engine, long id) =>
        engine.LEngineEntry.LEngineStampRead(id);

    internal static string TEngineStampFormat(string? utc) => LEntryFacade.LEngineStampFormat(utc);

    internal static (string, IReadOnlyList<LMentionPiece>, string) TEngineLineRead(
        LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations) =>
        LExampleClerk.LExampleLineRead(sentence, order, mark, citations);

    internal static LEntry TEngineEntrySave(this LEngine engine, LEntryDraft draft) =>
        engine.TEngineEntryCommit(null, draft);

    internal static LEntry TEngineEntryUpdate(this LEngine engine, long id, LEntryDraft draft) =>
        engine.TEngineEntryCommit(id, draft);

    private static LEntry TEngineEntryCommit(this LEngine engine, long? id, LEntryDraft draft)
    {
        LDraft started = engine.LEngineDraft.LEngineDraftStart("test", id);
        LEngineStaff staff = engine.LEngineHearth.LEngineStaffHeld;
        staff.LEngineStaffClaim.LClaimStaffClaim.LDraftSave(
            started with { LDraftContent = staff.LEngineStaffClaim.LClaimStaffDraft.LDraftClerkNormalize(draft) });
        return engine.LEngineDraft.LEngineDraftCommit(started.LDraftId).LOutcomeEntry;
    }

    internal static bool TEngineFavoriteCheck(this LEngine engine, long entryId) =>
        engine.LEngineCatalog.LEngineFavoriteCheck(entryId);

    internal static void TEngineFavoriteDelete(this LEngine engine, long entryId)
    {
        engine.LEngineCatalog.LEngineFavoriteDelete(entryId);
    }

    internal static void TEngineFavoriteSave(this LEngine engine, long entryId)
    {
        engine.LEngineCatalog.LEngineFavoriteSave(entryId);
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
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffCatalog.LCatalogStaffMention
            .LMentionClerkFind(text, language, offset, mentions);

    internal static IReadOnlyList<LDraft> TEngineLeftoverRead(this LEngine engine)
    {
        LClaimClerk claims = engine.LEngineHearth.LEngineStaffHeld.LEngineStaffClaim.LClaimStaffClaim;
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

    internal static LDraft TEngineRequestApply(this LEngine engine, LRequest request) =>
        engine.LEngineRequest.LEngineRequestApply(request);

    internal static LEngine TEngineCreate(LRig rig) => new(rig, _ => rig, _ => { });
}
