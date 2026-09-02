using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TImage
{
    [Fact]
    public void EveryImageACardReferencesLoadsBackInTheOrderItWasWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                [],
                string.Empty,
                [],
                [@"D:\pictures\word.png", "https://example.com/word.png"])],
            [new LCardDraft(
                string.Empty,
                "in a word",
                "briefly",
                [],
                [],
                string.Empty,
                [],
                ["https://example.com/phrase.png"])]));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            [@"D:\pictures\word.png", "https://example.com/word.png"],
            Assert.Single(loaded.LEntryDraftSenses).LCardDraftImage.Select(image => image.LStateValueShow()));
        Assert.Equal(
            "https://example.com/phrase.png",
            Assert.Single(Assert.Single(loaded.LEntryDraftCollocations).LCardDraftImage).LStateValueShow());
    }

    [Fact]
    public void AnImageDroppedFromACardIsDetachedAndANewOneTakesItsPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                string.Empty, string.Empty, "a meaning", [], [], string.Empty, [], ["first.png", "second.png"])],
            []));

        LEntryDraft? loaded = engine.LEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);

        engine.LEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftSenses =
            [
                loaded.LEntryDraftSenses[0] with { LCardDraftImage = [LStateValue.LStateValueCreate("third.png")] },
            ],
        });

        LEntryDraft? reloaded = engine.LEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(reloaded);
        Assert.Equal(
            "third.png",
            Assert.Single(Assert.Single(reloaded.LEntryDraftSenses).LCardDraftImage).LStateValueShow());
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_image WHERE 1 = 0;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_image;"));
    }

    [Fact]
    public void AnImageLeftEmptyIsNeitherWrittenNorLoadedBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                string.Empty, string.Empty, "a meaning", [], [], string.Empty, [], ["   "])],
            []));

        LEntryDraft? loaded = engine.LEngineEntryLoad(entry.LEntryId);

        Assert.NotNull(loaded);
        Assert.Empty(Assert.Single(loaded.LEntryDraftSenses).LCardDraftImage);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
    }
}
