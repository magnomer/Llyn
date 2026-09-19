using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TVaultEntry
{
    [Fact]
    public void EntryRead_AfterCreate_ReturnsStoredEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryVault entries = TInterface.TEntryVaultCreate(workspace.TWorkspaceDatabase);

        LEntry stored = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "kindle", "en", 0, null, null),
            [TInterface.TFormCreate(0, 0, "kindle", null, "headword")],
            [TInterface.TSpeechCreate(0, 0, null, "verb")]);
        LEntry? read = entries.TEntryRead(stored.LEntryId);

        Assert.NotNull(read);
        Assert.Equal(stored.LEntryId, read.LEntryId);
        Assert.Equal("kindle", read.LEntryHeadword);
        Assert.Equal("en", read.LEntryLanguage);
    }

    [Fact]
    public void EntryLoad_AfterCreate_MatchesRead()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryVault entries = TInterface.TEntryVaultCreate(workspace.TWorkspaceDatabase);

        LEntry stored = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "kindle", "en", 0, null, null),
            [TInterface.TFormCreate(0, 0, "kindle", null, "headword"),
             TInterface.TFormCreate(0, 0, "kindled", null, "past")],
            [TInterface.TSpeechCreate(0, 0, null, "verb")]);
        LEntry? read = entries.TEntryRead(stored.LEntryId);
        LEntryDraft? loaded = entries.TEntryLoad(stored.LEntryId);

        Assert.NotNull(read);
        Assert.NotNull(loaded);
        Assert.Equal(read.LEntryHeadword, loaded.LEntryDraftHeadword);
        Assert.Equal(read.LEntryLanguage, loaded.LEntryDraftLanguage);
        Assert.Equal(2, loaded.LEntryDraftForms.Count);
        Assert.Single(loaded.LEntryDraftSpeeches);
    }

    [Fact]
    public void EntryFind_TwoEntries_ReturnsOnlyMatching()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryVault entries = TInterface.TEntryVaultCreate(workspace.TWorkspaceDatabase);

        entries.TEntryCreate(TInterface.TEntryCreate(0, "kindle", "en", 0, null, null), [], []);
        entries.TEntryCreate(TInterface.TEntryCreate(0, "ember", "en", 0, null, null), [], []);

        Assert.Equal("kindle", Assert.Single(entries.TEntryFind("kind")).LEntryHeadword);
        Assert.Equal(2, entries.TEntryFind(string.Empty).Count);
        Assert.Null(entries.TEntryRead(9999));
    }
}
