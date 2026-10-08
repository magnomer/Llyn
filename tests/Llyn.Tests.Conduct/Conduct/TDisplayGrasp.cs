using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayGrasp
{
    [Theory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(7, 7)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void DisplayGraspStep_HostileLimit_ReadsNoneBelowZero(int limit, int read)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = TInterfaceConductSound.TDisplayChosenCreate(
            engine, TInterfaceConduct.TEntryBundleCreate(engine, TGraspPortCreate(limit, 0)), 1);

        Assert.Equal(read, display.CDisplayGrasp.CDisplayGraspStep);
    }

    [Theory]
    [InlineData(5, int.MinValue, 0)]
    [InlineData(5, -1, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(5, 1, 1)]
    [InlineData(5, 3, 3)]
    [InlineData(5, 5, 5)]
    [InlineData(5, 6, 5)]
    [InlineData(5, int.MaxValue, 5)]
    [InlineData(0, 4, 0)]
    [InlineData(-3, 2, 0)]
    [InlineData(int.MinValue, int.MaxValue, 0)]
    public void DisplayGraspRead_HostileStep_ClampsBetweenZeroAndLimit(int limit, int stored, int read)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = TInterfaceConductSound.TDisplayChosenCreate(
            engine, TInterfaceConduct.TEntryBundleCreate(engine, TGraspPortCreate(limit, stored)), 1);

        CGrasp grasp = display.CDisplayGrasp.CDisplayGraspRead();

        Assert.Equal(new CGrasp(read, read.ToString(CultureInfo.InvariantCulture)), grasp);
        Assert.InRange(grasp.CGraspStep, 0, display.CDisplayGrasp.CDisplayGraspStep);
    }

    [Fact]
    public void DisplayGraspSet_StandingStepPressedAgain_ClearsTheGrasp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayArea.TDisplayAtelierCreate(engine);
        LEntry water = TDisplayArea.TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayArea.TDisplayWingPrepare(atelier);
        CDisplayGrasp area = wing.CWingDisplay.CDisplayGrasp;
        wing.CWingEntryOpen(water.LEntryId);

        CGrasp six = area.CDisplayGraspSet(6);
        Assert.Equal(6, six.CGraspStep);
        Assert.Equal(area.CDisplayGraspRead(6), six.CGraspLabel);
        Assert.Equal(6, wing.CWingDisplay.TDisplayGraspRead(water.LEntryId));

        area.CDisplayGraspSet(3);
        Assert.Equal(3, engine.TEngineGraspRead(water.LEntryId));

        Assert.Equal(0, area.CDisplayGraspSet(3).CGraspStep);
        Assert.Equal(0, engine.TEngineGraspRead(water.LEntryId));
    }

    [Fact]
    public void DisplayGraspSet_NoEntryChosen_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayArea.TDisplayAtelierCreate(engine);
        LEntry water = TDisplayArea.TDisplayEntrySave(engine, "water");
        CWing wing = TDisplayArea.TDisplayWingPrepare(atelier);
        CDisplayGrasp area = wing.CWingDisplay.CDisplayGrasp;

        CGrasp grasp = area.CDisplayGraspSet(4);

        Assert.Equal(new CGrasp(0, string.Empty), grasp);
        Assert.Empty(area.CDisplayGraspRead(4));
        Assert.Equal(0, engine.TEngineGraspRead(water.LEntryId));
    }

    internal static LGraspPort TGraspPortCreate(int limit, int stored) =>
        TEngineFake.TEngineCreate<LGraspPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["get_LEngineGraspStep"] = _ => limit,
            ["LEngineGraspRead"] = _ => stored,
            ["LEngineGraspFormat"] = static args => ((int)args![0]!).ToString(CultureInfo.InvariantCulture),
        });
}
