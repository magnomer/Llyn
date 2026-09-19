using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LEntryVault TEntryVaultCreate(LDatabase database) =>
        new LEntryArchive(database);

    internal static LDraftVault TDraftVaultCreate(string root) =>
        new LDraftArchive(root);

    internal static void TDraftSave(this LDraftVault draftVault, LDraft draft)
    {
        draftVault.LDraftSave(draft);
    }

    internal static LDraft? TDraftRead(this LDraftVault draftVault, long id) =>
        draftVault.LDraftRead(id);

    internal static IReadOnlyList<LDraft> TDraftScan(this LDraftVault draftVault) =>
        draftVault.LDraftScan();

    internal static void TDraftDelete(this LDraftVault draftVault, long id)
    {
        draftVault.LDraftDelete(id);
    }
}
