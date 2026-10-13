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
    public void MeaningRead_ReadyRows_WalksSubSensesUnderTheirParentInReadingOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LCardDraft firstSub =
            TInterface.TCardDraftCreate(string.Empty, string.Empty, "first sub-sense", [], [], [], [], [], 1);
        LCardDraft secondSub =
            TInterface.TCardDraftCreate("second sub-title", string.Empty, string.Empty, [], [], [], [], [], 2);
        LCardDraft first =
            TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)
            with { LCardDraftChild = [firstSub, secondSub] };
        LCardDraft second =
            TInterface.TCardDraftCreate(string.Empty, string.Empty, "another meaning", [], [], [], [], [], 2);
        LEntry entry = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("word", "English", string.Empty, string.Empty, [first, second], []));

        IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows =
            engine.TEngineMeaningRead(entry.LEntryId, "Display.Unknown");

        Assert.Equal(
            [("a meaning", 0), ("first sub-sense", 1), ("second sub-title", 1), ("another meaning", 0)],
            rows.Select(static row => (row.LMeaningName, row.LMeaningDepth)).ToList());
    }

    [Fact]
    public void MeaningClerkSort_NestedMeanings_ReadsDepthFirstByPosition()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;

        IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows = TInterface.TMeaningClerkSort(
            [
                TInterface.TMeaningCreate(3, 7, null, 2, TInterfaceState.TStateValueCreate("second"), blank),
                TInterface.TMeaningCreate(1, 7, null, 1, TInterfaceState.TStateValueCreate("first"), blank),
                TInterface.TMeaningCreate(4, 7, 1, 2, TInterfaceState.TStateValueCreate("first-b"), blank),
                TInterface.TMeaningCreate(2, 7, 1, 1, TInterfaceState.TStateValueCreate("first-a"), blank),
            ],
            "unknown");

        Assert.Equal([(1L, "first", 0), (2L, "first-a", 1), (4L, "first-b", 1), (3L, "second", 0)], rows);
    }

    [Fact]
    public void MeaningClerkSort_BlankName_TakesTheUnknownText()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;

        IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows =
            TInterface.TMeaningClerkSort([TInterface.TMeaningCreate(1, 7, null, 1, blank, blank)], "unknown");

        Assert.Equal("unknown", Assert.Single(rows).LMeaningName);
    }

    [Fact]
    public void MeaningName_TitleOrDefinition_PrefersTheTitle()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;

        LStateValue first = TInterfaceState.TStateValueCreate("first");
        LStateValue second = TInterfaceState.TStateValueCreate("second");

        Assert.Equal("first", TInterface.TMeaningCreate(2, 9, 1, 0, first, blank).LMeaningName);
        Assert.Equal("second", TInterface.TMeaningCreate(3, 9, 1, 1, blank, second).LMeaningName);
        Assert.Equal(string.Empty, TInterface.TMeaningCreate(4, 9, 1, 2, blank, blank).LMeaningName);
    }
}
