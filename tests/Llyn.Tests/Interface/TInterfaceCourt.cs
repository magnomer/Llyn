using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static bool TClaimArchiveCheck(string root, long draftId) =>
        new LClaimArchive(root).LClaimCheck(draftId);

    internal static void TClaimArchiveDelete(string root, long draftId)
    {
        new LClaimArchive(root).LClaimDelete(draftId);
    }

    internal static LClaim? TClaimArchiveRead(string root, long draftId) =>
        new LClaimArchive(root).LClaimRead(draftId);

    internal static void TClaimArchiveSave(string root, LClaim claim)
    {
        new LClaimArchive(root).LClaimSave(claim);
    }

    internal static IReadOnlyList<LClaim> TClaimArchiveScan(string root) =>
        new LClaimArchive(root).LClaimScan();

    internal static LClaim TClaimCreate(long draftId, int process, DateTimeOffset moment) =>
        new(draftId, process, moment);

    internal static LCourt? TCourtArchiveRead(string root, long id) =>
        new LCourtArchive(root).LCourtRead(id);

    internal static void TCourtArchiveSave(string root, LCourt link)
    {
        new LCourtArchive(root).LCourtSave(link);
    }

    internal static IReadOnlyList<LCourt> TCourtArchiveScan(string root) =>
        new LCourtArchive(root).LCourtScan();

    internal static IReadOnlyList<LCourt> TCourtArchiveSettle(string root, long draftId) =>
        new LCourtArchive(root).LCourtSettle(draftId);

    internal static void TCourtArchiveSweep(string root)
    {
        new LCourtArchive(root).LCourtSweep();
    }

    internal static void TDraftArchiveDelete(string root, long id)
    {
        new LDraftArchive(root).LDraftDelete(id);
    }

    internal static LDraft? TDraftArchiveRead(string root, long id) =>
        new LDraftArchive(root).LDraftRead(id);

    internal static void TDraftArchiveSave(string root, LDraft draft)
    {
        new LDraftArchive(root).LDraftSave(draft);
    }

    internal static IReadOnlyList<LDraft> TDraftArchiveScan(string root) =>
        new LDraftArchive(root).LDraftScan();

    internal static void TDraftArchiveSweep(string root)
    {
        new LDraftArchive(root).LDraftSweep();
    }
}
