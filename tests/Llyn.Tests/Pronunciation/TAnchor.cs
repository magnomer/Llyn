using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnchor
{
    private const string TAnchorLanguage = "Classical Chinese";

    [Fact]
    public void FanqieSave_Refetch_KeepsRowIdsAndAnchors()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry wan = engine.TEngineEntrySave(TAnchorDraftCreate("完", "환"));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(TAnchorLanguage, "完", [TAnchorRowCreate("匣", 0), TAnchorRowCreate("溪", 1)]);
        IReadOnlyList<LFanqieRow> first = fanqie.TFanqieRead(TAnchorLanguage, "完");
        TAnchorApply(engine, wan.LEntryId, [first[0].LFanqieRowId]);

        fanqie.TFanqieSave(
            TAnchorLanguage, "完", [TAnchorRowCreate("匣", 0, "寒A"), TAnchorRowCreate("溪", 1)]);

        IReadOnlyList<LFanqieRow> second = fanqie.TFanqieRead(TAnchorLanguage, "完");
        Assert.Equal(first.Select(row => row.LFanqieRowId), second.Select(row => row.LFanqieRowId));
        Assert.Equal("寒A", second[0].LFanqieRowRime);
        Assert.Equal(
            [first[0].LFanqieRowId], Assert.Single(engine.TEngineReflexRead(wan.LEntryId)).LReflexAnchors);

        fanqie.TFanqieSave(TAnchorLanguage, "完", [TAnchorRowCreate("溪", 1)]);

        Assert.Equal([first[1].LFanqieRowId], fanqie.TFanqieRead(TAnchorLanguage, "完").Select(row => row.LFanqieRowId));
        Assert.Empty(Assert.Single(engine.TEngineReflexRead(wan.LEntryId)).LReflexAnchors);
    }

    [Fact]
    public void ReflexSet_Anchors_RoundTripSortedAndDropUnknownRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry wan = engine.TEngineEntrySave(TAnchorDraftCreate("完", "환"));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(TAnchorLanguage, "完", [TAnchorRowCreate("匣", 0), TAnchorRowCreate("溪", 1)]);
        IReadOnlyList<long> ids = fanqie.TFanqieRead(TAnchorLanguage, "完").Select(row => row.LFanqieRowId).ToList();

        TAnchorApply(engine, wan.LEntryId, [ids[1], ids[0], 9999]);

        Assert.Equal([ids[0], ids[1]], Assert.Single(engine.TEngineReflexRead(wan.LEntryId)).LReflexAnchors);
        LEntryDraft? loaded = engine.TEngineEntryLoad(wan.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal([ids[0], ids[1]], Assert.Single(loaded.LEntryDraftReflexes).LReflexDraftAnchors);

        TAnchorApply(engine, wan.LEntryId, []);

        Assert.Empty(Assert.Single(engine.TEngineReflexRead(wan.LEntryId)).LReflexAnchors);
    }

    [Fact]
    public void RequestApply_ReflexAnchor_TogglesOneRowAndCommits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry wan = engine.TEngineEntrySave(TAnchorDraftCreate("完", "환", "관"));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(TAnchorLanguage, "完", [TAnchorRowCreate("匣", 0), TAnchorRowCreate("溪", 1)]);
        IReadOnlyList<long> ids = fanqie.TFanqieRead(TAnchorLanguage, "完").Select(row => row.LFanqieRowId).ToList();
        LDraft started = engine.TEngineDraftStart("Input", wan.LEntryId);
        long gwan = started.LDraftContent.LEntryDraftReflexes[1].LReflexDraftId;

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TReflexAnchorCreate(started.LDraftId, gwan, ids[1], true));
        Assert.Equal([ids[1]], answered.LDraftContent.LEntryDraftReflexes[1].LReflexDraftAnchors);
        Assert.Empty(answered.LDraftContent.LEntryDraftReflexes[0].LReflexDraftAnchors);
        Assert.True(engine.TEngineDraftCheck(started.LDraftId));

        answered = engine.TEngineRequestApply(TInterface.TReflexAnchorCreate(started.LDraftId, gwan, ids[0], true));
        answered = engine.TEngineRequestApply(TInterface.TReflexAnchorCreate(started.LDraftId, gwan, ids[1], false));
        Assert.Equal([ids[0]], answered.LDraftContent.LEntryDraftReflexes[1].LReflexDraftAnchors);

        engine.TEngineDraftCommit(started.LDraftId);

        IReadOnlyList<LReflex> stored = engine.TEngineReflexRead(wan.LEntryId);
        Assert.Equal([[], [ids[0]]], stored.Select(reflex => reflex.LReflexAnchors.ToArray()));
    }

    private static void TAnchorApply(LEngine engine, long entryId, IReadOnlyList<long> anchors)
    {
        engine.TEngineReflexSet(
            entryId,
            engine.TEngineReflexRead(entryId).Select(reflex => reflex with { LReflexAnchors = anchors }).ToList());
    }

    [Fact]
    public void AnchorRowScan_UnstoredRow_IsLeftOut()
    {
        LFanqieRow loose = TAnchorRowCreate("疑", 1);
        LFanqieRow held = TAnchorRowCreate("匣", 0) with { LFanqieRowClass = "A", LFanqieRowId = 5 };

        LAnchorRow row = Assert.Single(TInterface.TAnchorRowScan([held, loose], [5], ["A"]));

        Assert.True(row.LAnchorRowHeld);
        Assert.True(row.LAnchorRowEstimated);
    }

    [Fact]
    public void AnchorTextFormat_WordHeadword_IsEmpty()
    {
        LFanqieRow held = TAnchorRowCreate("匣", 0) with { LFanqieRowId = 5, LFanqieRowSummary = "胡官切" };

        Assert.Equal("胡官切", TInterface.TAnchorTextFormat([held], [5], "完"));
        Assert.Empty(TInterface.TAnchorTextFormat([held], [5], "完全"));
    }

    private static LFanqieRow TAnchorRowCreate(string initial, int position, string rime = "寒")
    {
        return TInterface.TFanqieRowCreate("完", position, initial, rime, "一", "平");
    }

    private static LEntryDraft TAnchorDraftCreate(string headword, params string[] readings)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            TAnchorLanguage,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: readings.Select(reading => TInterface.TReflexDraftCreate("Korean", "", reading)).ToList());
    }
}
