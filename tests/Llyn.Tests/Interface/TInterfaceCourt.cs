using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static bool TClaimArchiveCheck(string root, long draftId) =>
        LClaimArchive.LClaimArchiveCheck(root, draftId);

    internal static void TClaimArchiveDelete(string root, long draftId)
    {
        LClaimArchive.LClaimArchiveDelete(root, draftId);
    }

    internal static LClaim? TClaimArchiveRead(string root, long draftId) =>
        LClaimArchive.LClaimArchiveRead(root, draftId);

    internal static void TClaimArchiveSave(string root, LClaim claim)
    {
        LClaimArchive.LClaimArchiveSave(root, claim);
    }

    internal static IReadOnlyList<LClaim> TClaimArchiveScan(string root) =>
        LClaimArchive.LClaimArchiveScan(root);

    internal static LClaim TClaimCreate(long draftId, int process, DateTimeOffset moment) =>
        new(draftId, process, moment);

    internal static LCourt? TCourtArchiveRead(string root, long id) =>
        LCourtArchive.LCourtArchiveRead(root, id);

    internal static void TCourtArchiveSave(string root, LCourt link)
    {
        LCourtArchive.LCourtArchiveSave(root, link);
    }

    internal static IReadOnlyList<LCourt> TCourtArchiveScan(string root) =>
        LCourtArchive.LCourtArchiveScan(root);

    internal static IReadOnlyList<LCourt> TCourtArchiveSettle(string root, long draftId) =>
        LCourtArchive.LCourtArchiveSettle(root, draftId);

    internal static void TCourtArchiveSweep(string root)
    {
        LCourtArchive.LCourtArchiveSweep(root);
    }

    internal static void TDraftArchiveDelete(string root, long id)
    {
        LDraftArchive.LDraftArchiveDelete(root, id);
    }

    internal static LDraft? TDraftArchiveRead(string root, long id) =>
        LDraftArchive.LDraftArchiveRead(root, id);

    internal static void TDraftArchiveSave(string root, LDraft draft)
    {
        LDraftArchive.LDraftArchiveSave(root, draft);
    }

    internal static IReadOnlyList<LDraft> TDraftArchiveScan(string root) =>
        LDraftArchive.LDraftArchiveScan(root);

    internal static void TDraftArchiveSweep(string root)
    {
        LDraftArchive.LDraftArchiveSweep(root);
    }
}
