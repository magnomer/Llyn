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
                [],
                [
                    TInterface.TImageDraftCreate(@"D:\pictures\word.png"),
                    TInterface.TImageDraftCreate("https://example.com/word.png"),
                ],
                1)],
            [TInterface.TCardDraftCreate(
                string.Empty,
                "in a word",
                "briefly",
                [],
                [],
                [],
                [],
                [TInterface.TImageDraftCreate("https://example.com/phrase.png")],
                1)]));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            [@"D:\pictures\word.png", "https://example.com/word.png"],
            Assert.Single(loaded.LEntryDraftMeanings).LCardDraftImage.Select(
                image => image.LImageDraftLocation.TStateValueShow()));
        Assert.Equal(
            "https://example.com/phrase.png",
            Assert.Single(Assert.Single(loaded.LEntryDraftCollocations).LCardDraftImage)
                .LImageDraftLocation.TStateValueShow());
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
                string.Empty, string.Empty, "a meaning", [], [], [], [],
                [TInterface.TImageDraftCreate("first.png"), TInterface.TImageDraftCreate("second.png")],
                1)],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                loaded.LEntryDraftMeanings[0] with { LCardDraftImage = [TInterface.TImageDraftCreate("third.png")] },
            ],
        });

        LEntryDraft? reloaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(reloaded);
        Assert.Equal(
            "third.png",
            Assert.Single(Assert.Single(reloaded.LEntryDraftMeanings).LCardDraftImage)
                .LImageDraftLocation.TStateValueShow());
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
                string.Empty, string.Empty, "a meaning", [], [], [], [], [TInterface.TImageDraftCreate("   ")], 1)],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);

        Assert.NotNull(loaded);
        Assert.Empty(Assert.Single(loaded.LEntryDraftMeanings).LCardDraftImage);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
    }
}
