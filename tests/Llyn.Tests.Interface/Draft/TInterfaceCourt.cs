using System.Text.Json;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static bool TClaimArchiveCheck(string root, long draftId) =>
        TDraftVaultCreate(root).LDraftClaimCheck(draftId);

    internal static string? TClaimArchiveRead(string root, long draftId)
    {
        string path = Path.Combine(LWorkspaceRoot.LWorkspaceClaimRead(root), draftId + ".json");
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    internal static void TClaimArchiveSave(string root, long draftId, int process, DateTimeOffset moment)
    {
        string path = Path.Combine(LWorkspaceRoot.LWorkspaceClaimRead(root), draftId + ".json");
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(new { LClaimDraft = draftId, LClaimProcess = process, LClaimMoment = moment }));
    }

    internal static IReadOnlyList<string> TClaimArchiveScan(string root) =>
        Directory.GetFiles(LWorkspaceRoot.LWorkspaceClaimRead(root), "*.json");

    internal static LCourt? TCourtArchiveRead(string root, long id) =>
        TDraftVaultCreate(root).LDraftCourtScan().FirstOrDefault(link => link.LCourtId == id);

    internal static void TCourtArchiveSave(string root, LCourt link)
    {
        TDraftVaultCreate(root).LDraftCourtSave(link);
    }

    internal static IReadOnlyList<LCourt> TCourtArchiveScan(string root) =>
        TDraftVaultCreate(root).LDraftCourtScan();

    internal static IReadOnlyList<long> TCourtArchiveSettle(string root, long draftId, long realId) =>
        TDraftVaultCreate(root).LDraftCourtSettle(draftId, realId);

    internal static LCourt TCourtCreate(long id, long ownerId, long targetId, string headword, string language) =>
        new(id, ownerId, targetId, headword, language);

    internal static LDraft TDraftArchiveStart(string root, LDraft draft) =>
        TDraftVaultCreate(root).LDraftStart(draft);

    internal static LDraftRelease TDraftArchiveCancel(string root, long id, IReadOnlySet<long> held) =>
        TDraftVaultCreate(root).LDraftCancel(id, held);

    internal static void TDraftArchiveDelete(string root, long id)
    {
        TDraftVaultCreate(root).LDraftFinish(id);
    }

    internal static LDraft? TDraftArchiveRead(string root, long id) =>
        TDraftVaultCreate(root).LDraftRead(id);

    internal static void TDraftArchiveSave(string root, LDraft draft)
    {
        TDraftVaultCreate(root).LDraftSave(draft);
    }

    internal static IReadOnlyList<LDraft> TDraftArchiveScan(string root) =>
        TDraftVaultCreate(root).LDraftScan();
}
