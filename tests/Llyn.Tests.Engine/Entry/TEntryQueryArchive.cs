using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryQueryArchive
{
    [Fact]
    public void EntryFind_AccentedOtherCase_FindsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LEntryQueryArchive queries = TInterface.TEntryQueryCreate(workspace.TWorkspaceDatabase);

        entries.TEntryCreate(TInterface.TEntryCreate(0, "Äpfel", "de", 0, null, null), [], []);
        entries.TEntryCreate(TInterface.TEntryCreate(0, "straße", "de", 0, null, null), [], []);

        Assert.Equal("Äpfel", Assert.Single(queries.TEntryFind("äpfel")).LEntryHeadword);
        Assert.Equal("Äpfel", Assert.Single(queries.TEntryFind("ÄPF")).LEntryHeadword);
        Assert.Equal("straße", Assert.Single(queries.TEntryFind("STRAßE")).LEntryHeadword);

        Assert.Equal(2, queries.TEntryFind("   ").Count);
        Assert.Equal(2, queries.TEntryFind(string.Empty).Count);
        Assert.Single(queries.TEntryFind("  Äpfel "));
    }

    [Fact]
    public void EntryHeadwordFind_LanguagesStoredOutOfOrder_ListsByLanguageThenId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LEntryQueryArchive queries = TInterface.TEntryQueryCreate(workspace.TWorkspaceDatabase);

        LEntry lower = entries.TEntryCreate(TInterface.TEntryCreate(0, "rosa", "latin", 0, null, null), [], []);
        LEntry first = entries.TEntryCreate(TInterface.TEntryCreate(0, "rosa", "Latin", 0, null, null), [], []);
        LEntry second = entries.TEntryCreate(TInterface.TEntryCreate(0, "rosa", "Latin", 0, null, null), [], []);

        Assert.Equal(
            [first.LEntryId, second.LEntryId, lower.LEntryId],
            queries.TEntryHeadwordFind("LATIN", "rosa").Select(entry => entry.LEntryId));
    }
}
