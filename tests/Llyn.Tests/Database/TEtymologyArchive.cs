using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEtymologyArchive
{
    [Fact]
    public void EtymologySave_ProseWithSpans_ReadsThemBackInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long other = TEtymologyEntryCreate(engine, "iernan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        LEtymologyArchive archive = TInterface.TEtymologyArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TEtymologySave(entryId, TInterface.TEtymologyCreate(
            entryId,
            "From rinnan, crossed with iernan.",
            [TInterface.TMentionCreate(0, 20, 6, other), TInterface.TMentionCreate(0, 5, 6, source)]));

        LEtymology stored = Assert.IsType<LEtymology>(archive.TEtymologyRead(entryId));
        Assert.Equal("From rinnan, crossed with iernan.", stored.LEtymologyText);
        Assert.Equal([5, 20], stored.LEtymologyMentions.Select(static mention => mention.LMentionOffset));
        Assert.Equal([source, other], stored.LEtymologyMentions.Select(static mention => mention.LMentionEntryId));
        Assert.True(stored.LEtymologyNarrated);
    }

    [Fact]
    public void EtymologySave_BlankText_ClearsTheRowAndItsSpans()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        LEtymologyArchive archive = TInterface.TEtymologyArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TEtymologySave(entryId, TInterface.TEtymologyCreate(
            entryId, "From rinnan.", [TInterface.TMentionCreate(0, 5, 6, source)]));

        Assert.Null(archive.TEtymologySave(entryId, null));

        Assert.Null(archive.TEtymologyRead(entryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM etymology_mention;"));
    }

    [Fact]
    public void EtymologySave_SpanPastTheTextOrNamingNoEntry_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        LEtymologyArchive archive = TInterface.TEtymologyArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Throws<InvalidOperationException>(() => archive.TEtymologySave(
            entryId,
            TInterface.TEtymologyCreate(
                entryId, "From rinnan.", [TInterface.TMentionCreate(0, 5, 40, source)])));
        Assert.Throws<InvalidOperationException>(() => archive.TEtymologySave(
            entryId,
            TInterface.TEtymologyCreate(entryId, "From rinnan.", [TInterface.TMentionCreate(0, 5, 6, 0)])));
        Assert.Throws<InvalidOperationException>(() => archive.TEtymologySave(
            entryId,
            TInterface.TEtymologyCreate(
                entryId,
                "From rinnan.",
                [TInterface.TMentionCreate(0, 0, 8, source), TInterface.TMentionCreate(0, 5, 6, source)])));

        Assert.Null(archive.TEtymologyRead(entryId));
    }

    [Fact]
    public void EtymonSet_RepeatsSelfAndNothing_StoresTheRestInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long first = TEtymologyEntryCreate(engine, "rinnan");
        long second = TEtymologyEntryCreate(engine, "iernan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        LEtymologyArchive archive = TInterface.TEtymologyArchiveCreate(workspace.TWorkspaceDatabase);

        IReadOnlyList<LEtymon> written = archive.TEtymonSet(
            entryId, [second, first, second, entryId, 0]);

        Assert.Equal([second, first], written.Select(static etymon => etymon.LEtymonTargetId));
        Assert.Equal([0, 1], written.Select(static etymon => etymon.LEtymonPosition));
        Assert.Equal([second, first], archive.TEtymonRead(entryId).Select(static row => row.LEtymonTargetId));
    }

    [Fact]
    public void EtymonSet_EmptyList_ClearsEveryLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        LEtymologyArchive archive = TInterface.TEtymologyArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TEtymonSet(entryId, [source]);

        Assert.Empty(archive.TEtymonSet(entryId, []));
        Assert.Empty(archive.TEtymonRead(entryId));
    }

    [Fact]
    public void EtymologySourceScan_NamedEntry_ListsBothShapesOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long linked = TEtymologyEntryCreate(engine, "run");
        long narrated = TEtymologyEntryCreate(engine, "runner");
        LEtymologyArchive archive = TInterface.TEtymologyArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TEtymonSet(linked, [source]);
        archive.TEtymologySave(narrated, TInterface.TEtymologyCreate(
            narrated, "From rinnan.", [TInterface.TMentionCreate(0, 5, 6, source)]));

        Assert.Equal(
            [linked, narrated],
            archive.TEtymologySourceScan(source).Select(static entry => entry.LEntryId).Order());
        Assert.Empty(archive.TEtymologySourceScan(linked));
    }

    [Fact]
    public void EntryDelete_NamedSource_TakesEveryLinkToItAway()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        LEtymologyArchive archive = TInterface.TEtymologyArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TEtymonSet(entryId, [source]);
        archive.TEtymologySave(entryId, TInterface.TEtymologyCreate(
            entryId, "From rinnan.", [TInterface.TMentionCreate(0, 5, 6, source)]));

        engine.TEngineEntryDelete(source);

        Assert.Empty(archive.TEtymonRead(entryId));
        Assert.Empty(Assert.IsType<LEtymology>(archive.TEtymologyRead(entryId)).LEtymologyMentions);
    }

    private static long TEtymologyEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [], [])).LEntryId;
    }
}
