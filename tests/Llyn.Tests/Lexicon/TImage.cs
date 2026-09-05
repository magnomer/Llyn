using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TImage
{
    [Fact]
    public void EntryLoad_CardImages_ReturnsWrittenOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                [],
                [],
                string.Empty,
                [],
                [@"D:\pictures\word.png", "https://example.com/word.png"],
                1)],
            [TInterface.TCardDraftCreate(
                string.Empty,
                "in a word",
                "briefly",
                [],
                [],
                [],
                string.Empty,
                [],
                ["https://example.com/phrase.png"],
                1)]));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            [@"D:\pictures\word.png", "https://example.com/word.png"],
            Assert.Single(loaded.LEntryDraftMeanings).LCardDraftImage.Select(image => image.TStateValueShow()));
        Assert.Equal(
            "https://example.com/phrase.png",
            Assert.Single(Assert.Single(loaded.LEntryDraftCollocations).LCardDraftImage).TStateValueShow());
    }

    [Fact]
    public void EntryUpdate_ImageReplaced_DetachesOldAttachesNew()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], ["first.png", "second.png"], 1)],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                loaded.LEntryDraftMeanings[0] with { LCardDraftImage = [TInterface.TStateValueCreate("third.png")] },
            ],
        });

        LEntryDraft? reloaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(reloaded);
        Assert.Equal(
            "third.png",
            Assert.Single(Assert.Single(reloaded.LEntryDraftMeanings).LCardDraftImage).TStateValueShow());
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_image WHERE 1 = 0;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_image;"));
    }

    [Fact]
    public void EntrySave_EmptyImage_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], ["   "], 1)],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);

        Assert.NotNull(loaded);
        Assert.Empty(Assert.Single(loaded.LEntryDraftMeanings).LCardDraftImage);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
    }
}
