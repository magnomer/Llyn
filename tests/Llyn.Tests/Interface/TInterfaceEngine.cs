using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LRevisionChange> TEngineChangeRead(
        this LEngine engine,
        string revisionId) =>
        engine.LEngineChangeRead(revisionId);

    internal static LCourtLink? TEngineCourtFind(this LEngine engine, string ownerId, string targetId) =>
        engine.LEngineCourtFind(ownerId, targetId);

    internal static LCourtLink TEngineCourtStart(
        this LEngine engine,
        string ownerId,
        string origin,
        string headword,
        string language) =>
        engine.LEngineCourtStart(ownerId, origin, headword, language);

    internal static string TEngineCardCreate(this LEngine engine) =>
        engine.LEngineCardCreate();

    internal static IReadOnlyList<LCardDraft> TEngineDraftNormalize(
        this LEngine engine,
        string id,
        bool collocation) =>
        engine.LEngineDraftNormalize(id, collocation);

    internal static LCourtLink TEngineCourtSave(
        this LEngine engine,
        string ownerId,
        string targetId,
        string headword,
        string language) =>
        engine.LEngineCourtSave(ownerId, targetId, headword, language);

    internal static void TEngineDraftCancel(this LEngine engine, string id)
    {
        engine.LEngineDraftCancel(id);
    }

    internal static bool TEngineDraftCheck(this LEngine engine, string id) =>
        engine.LEngineDraftCheck(id);

    internal static LEntry TEngineDraftCommit(this LEngine engine, string id) =>
        engine.LEngineDraftCommit(id);

    internal static void TEngineDraftDelete(this LEngine engine, string id)
    {
        engine.LEngineDraftDelete(id);
    }

    internal static IReadOnlyList<LCardDraft> TEngineDraftMove(
        this LEngine engine,
        string id,
        bool collocation,
        int from,
        int target) =>
        engine.LEngineDraftMove(id, collocation, from, target);

    internal static LDraft? TEngineDraftRead(this LEngine engine, string id) =>
        engine.LEngineDraftRead(id);

    internal static LEntryDraft TEngineDraftSave(this LEngine engine, LDraft draft) =>
        engine.LEngineDraftSave(draft);

    internal static IReadOnlyList<LDraft> TEngineDraftScan(this LEngine engine) =>
        engine.LEngineDraftScan();

    internal static LDraft TEngineDraftStart(this LEngine engine, string origin, string? entryId) =>
        engine.LEngineDraftStart(origin, entryId);

    internal static LRevision TEngineEntryDelete(this LEngine engine, string id) =>
        engine.LEngineEntryDelete(id);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LTag tag) =>
        engine.LEngineEntryFind(tag);

    internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, string query) =>
        engine.LEngineEntryFind(query);

    internal static LEntryDraft? TEngineEntryLoad(this LEngine engine, string id) =>
        engine.LEngineEntryLoad(id);

    internal static LEntry? TEngineEntryRead(this LEngine engine, string id) =>
        engine.LEngineEntryRead(id);

    internal static LEntry TEngineEntrySave(this LEngine engine, LEntryDraft draft) =>
        engine.LEngineEntrySave(draft);

    internal static LEntry TEngineEntryUpdate(this LEngine engine, string id, LEntryDraft draft) =>
        engine.LEngineEntryUpdate(id, draft);

    internal static bool TEngineFavoriteCheck(this LEngine engine, string entryId) =>
        engine.LEngineFavoriteCheck(entryId);

    internal static void TEngineFavoriteDelete(this LEngine engine, string entryId)
    {
        engine.LEngineFavoriteDelete(entryId);
    }

    internal static IReadOnlyList<LFavorite> TEngineFavoriteFind(this LEngine engine, string query) =>
        engine.LEngineFavoriteFind(query);

    internal static void TEngineFavoriteSave(this LEngine engine, string entryId)
    {
        engine.LEngineFavoriteSave(entryId);
    }

    internal static IReadOnlyList<LDraft> TEngineLeftoverRead(this LEngine engine) =>
        engine.LEngineLeftoverRead();

    internal static void TEngineLeftoverSweep(this LEngine engine)
    {
        engine.LEngineLeftoverSweep();
    }

    internal static IReadOnlyList<LEntry> TEngineMarkupImport(this LEngine engine, string text) =>
        engine.LEngineMarkupImport(text);

    internal static LRevision? TEngineRevisionRead(this LEngine engine) =>
        engine.LEngineRevisionRead();

    internal static LWorkspaceState TEngineStateRead(this LEngine engine) =>
        engine.LEngineStateRead();
}
