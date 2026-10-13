using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFoldBox
{
    [Theory]
    [InlineData(LFoldBox.LFoldBoxFanqie)]
    [InlineData(LFoldBox.LFoldBoxScript)]
    public void BoxSpread_OpenedEntry_AnswersOpened(LFoldBox box)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "word", out long entryId);

        Assert.False(engine.TEngineBoxCheck(entryId, box));

        engine.TEngineBoxSpread(entryId, box, true);

        Assert.True(engine.TEngineBoxCheck(entryId, box));
    }

    [Theory]
    [InlineData(LFoldBox.LFoldBoxFanqie, "fanqie_fold")]
    [InlineData(LFoldBox.LFoldBoxScript, "script_fold")]
    public void BoxSpread_ClosedAfterOpened_AnswersClosedAndKeepsNoRow(LFoldBox box, string table)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "word", out long entryId);
        engine.TEngineBoxSpread(entryId, box, true);

        engine.TEngineBoxSpread(entryId, box, false);

        Assert.False(engine.TEngineBoxCheck(entryId, box));
        Assert.Equal(0, workspace.TWorkspaceCountRead($"SELECT COUNT(*) FROM {table};"));
    }

    [Theory]
    [InlineData(LFoldBox.LFoldBoxFanqie, LFoldBox.LFoldBoxScript)]
    [InlineData(LFoldBox.LFoldBoxScript, LFoldBox.LFoldBoxFanqie)]
    public void BoxSpread_OneBoxOpened_LeavesTheOtherBoxAndTheReflexFoldClosed(LFoldBox box, LFoldBox other)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "word", out long entryId);

        engine.TEngineBoxSpread(entryId, box, true);

        Assert.True(engine.TEngineBoxCheck(entryId, box));
        Assert.False(engine.TEngineBoxCheck(entryId, other));
        Assert.False(engine.TEngineSpreadCheck(entryId));
    }

    [Fact]
    public void ReflexSpread_OpenedEntry_LeavesBothBoxesClosed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "word", out long entryId);

        engine.TEngineReflexSpread(entryId, true);

        Assert.False(engine.TEngineBoxCheck(entryId, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(entryId, LFoldBox.LFoldBoxScript));
    }

    [Theory]
    [InlineData(LFoldBox.LFoldBoxFanqie)]
    [InlineData(LFoldBox.LFoldBoxScript)]
    public void BoxSpread_TwoEntries_KeepsEachEntrysOwnState(LFoldBox box)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "water", out long firstId);
        TFold.TFoldEntryCreate(engine, "stone", out long secondId);

        engine.TEngineBoxSpread(firstId, box, true);

        Assert.True(engine.TEngineBoxCheck(firstId, box));
        Assert.False(engine.TEngineBoxCheck(secondId, box));

        engine.TEngineBoxSpread(secondId, box, true);
        engine.TEngineBoxSpread(firstId, box, false);

        Assert.False(engine.TEngineBoxCheck(firstId, box));
        Assert.True(engine.TEngineBoxCheck(secondId, box));
    }

    [Fact]
    public void EntryDelete_SpreadBoxes_DropsTheirRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "word", out long entryId);
        engine.TEngineBoxSpread(entryId, LFoldBox.LFoldBoxFanqie, true);
        engine.TEngineBoxSpread(entryId, LFoldBox.LFoldBoxScript, true);

        engine.TEngineEntryDelete(entryId);

        Assert.False(engine.TEngineBoxCheck(entryId, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(entryId, LFoldBox.LFoldBoxScript));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM fanqie_fold;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM script_fold;"));
    }

    [Theory]
    [InlineData(0, LFoldBox.LFoldBoxFanqie, "fanqie_fold")]
    [InlineData(-1, LFoldBox.LFoldBoxFanqie, "fanqie_fold")]
    [InlineData(0, LFoldBox.LFoldBoxScript, "script_fold")]
    [InlineData(-1, LFoldBox.LFoldBoxScript, "script_fold")]
    public void BoxSpread_IdAtOrBelowZero_WritesNothingAndThrowsNothing(long entryId, LFoldBox box, string table)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "word", out _);

        engine.TEngineBoxSpread(entryId, box, true);

        Assert.False(engine.TEngineBoxCheck(entryId, box));
        Assert.Equal(0, workspace.TWorkspaceCountRead($"SELECT COUNT(*) FROM {table};"));

        engine.TEngineBoxSpread(entryId, box, false);

        Assert.False(engine.TEngineBoxCheck(entryId, box));
    }

    [Theory]
    [InlineData(LFoldBox.LFoldBoxFanqie)]
    [InlineData(LFoldBox.LFoldBoxScript)]
    public void BoxSpread_StoredEntry_RaisesFoldBulletinOnItsEntry(LFoldBox box)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFold.TFoldEntryCreate(engine, "word", out long entryId);
        List<long> raised = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectFold)
            {
                raised.Add(bulletin.LBulletinId);
            }
        });

        engine.TEngineBoxSpread(entryId, box, true);
        engine.TEngineBoxSpread(entryId, box, false);

        Assert.Equal([entryId, entryId], raised);
    }
}
