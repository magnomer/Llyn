using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMeaning
{
    [Fact]
    public void MeaningRead_SubSensesUnderParent_CarryParentAndPosition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LCardDraft firstSub =
            TInterface.TCardDraftCreate(string.Empty, string.Empty, "first sub-sense", [], [], [], [], [], 1);
        LCardDraft secondSub =
            TInterface.TCardDraftCreate(string.Empty, string.Empty, "second sub-sense", [], [], [], [], [], 2);
        LCardDraft root =
            TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)
            with { LCardDraftChild = [firstSub, secondSub] };
        LEntry entry = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("word", "English", string.Empty, string.Empty, [root], []));

        IReadOnlyList<LMeaning> read = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry);

        Assert.Equal(3, read.Count);
        LMeaning parent = read.Single(row => row.LMeaningParentId is null);
        LMeaning[] children = read.Where(row => row.LMeaningParentId == parent.LMeaningId)
            .OrderBy(row => row.LMeaningPosition).ToArray();
        Assert.Equal(
            ["first sub-sense", "second sub-sense"],
            children.Select(row => row.LMeaningDefinition.TStateValueShow()));
        Assert.Equal([0, 1], children.Select(row => row.LMeaningPosition));
    }

    [Fact]
    public void MeaningRead_UnknownSide_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TMeaningEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.TEngineMeaningRead(meaningId, LOwner.LOwnerMeaning));
    }

    private static LEntry TMeaningEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }

    [Fact]
    public void MeaningName_TitleOrDefinition_PrefersTheTitle()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;

        LStateValue first = TInterface.TStateValueCreate("first");
        LStateValue second = TInterface.TStateValueCreate("second");

        Assert.Equal("first", TInterface.TMeaningCreate(2, 9, 1, 0, first, blank).LMeaningName);
        Assert.Equal("second", TInterface.TMeaningCreate(3, 9, 1, 1, blank, second).LMeaningName);
        Assert.Equal(string.Empty, TInterface.TMeaningCreate(4, 9, 1, 2, blank, blank).LMeaningName);
    }
}
