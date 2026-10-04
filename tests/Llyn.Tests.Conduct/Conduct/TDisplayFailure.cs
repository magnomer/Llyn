using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayFailure
{
    [Fact]
    public void DisplayEntryOpen_StampPortFails_ShowsTheStampFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        TDisplayWingOpen(engine, atelier, asked);

        Assert.Contains("Display.StampFailed", asked);
    }

    [Fact]
    public void DisplayCardRead_OrderPortFails_ShowsTheOrderFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplayArea.CDisplayCardRead();

        Assert.Contains("Display.OrderFailed", asked);
    }

    [Fact]
    public void DisplayCardRead_CitationPortFails_ShowsTheCitationFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplayArea.CDisplayCardRead();

        Assert.Contains("Display.CitationFailed", asked);
    }

    [Fact]
    public void DisplayReflexRead_ReflexPortFails_ShowsTheReflexFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        CLecternReflex reflex = wing.CWingDisplay.CDisplaySound.CDisplayReflexRead();

        Assert.Empty(reflex.CLecternReflexRows);
        Assert.Contains("Display.ReflexFailed", asked);
    }

    [Fact]
    public void DisplayFanqieRead_ReadingPortFails_ShowsTheReadingFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplaySound.CDisplayFanqieRead();

        Assert.Contains("Display.ReadingFailed", asked);
    }

    [Fact]
    public void DisplayParadigmRead_LanguagePortFails_ShowsTheLanguageFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();

        Assert.Contains("Display.LanguageFailed", asked);
    }

    [Fact]
    public void DisplayEntryOpen_SoundStartFails_ShowsTheStartFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        TDisplayWingOpen(engine, atelier, asked);

        Assert.Contains("Sound.StartFailed", asked);
    }

    [Fact]
    public void DisplayParadigmRead_ScanPortFails_ShowsTheParadigmFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        CLecternParadigm paradigm = wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();

        Assert.Empty(paradigm.CLecternParadigmSlots);
        Assert.Contains("Display.ParadigmReadFailed", asked);
    }

    [Fact]
    public void DisplayParadigmRead_CheckPortFails_ShowsThePendingFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();

        Assert.Contains("Sound.PendingFailed", asked);
    }

    [Fact]
    public void DisplayFanqieRead_AnchorPortFails_ShowsTheAnchorFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        CLecternFanqie fanqie = wing.CWingDisplay.CDisplaySound.CDisplayFanqieRead();

        Assert.Empty(fanqie.CLecternFanqieGroups);
        Assert.Contains("Display.AnchorFailed", asked);
    }

    [Fact]
    public void DisplayFavoriteRead_PortFails_ShowsTheReadFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);

        Assert.False(wing.CWingDisplay.TDisplayFavoriteRead(1));
        Assert.Equal(["Favorite.ReadFailed"], asked);
    }

    [Fact]
    public void DisplayFavoriteRead_PortFailsTwice_ShowsOneNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);

        wing.CWingDisplay.TDisplayFavoriteRead(1);
        wing.CWingDisplay.TDisplayFavoriteRead(1);

        Assert.Equal(["Favorite.ReadFailed"], asked);
    }

    [Fact]
    public void DisplayCardRead_OrderPortFailsTwice_ShowsOneNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplayArea.CDisplayCardRead();
        wing.CWingDisplay.CDisplayArea.CDisplayCardRead();

        Assert.Single(asked, key => key == "Display.OrderFailed");
    }

    [Fact]
    public void DisplayFavoriteToggle_PortFailsTwice_ShowsTwoNotices()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplayArea.CDisplayFavoriteToggle(true);
        wing.CWingDisplay.CDisplayArea.CDisplayFavoriteToggle(true);

        Assert.Equal(2, asked.FindAll(key => key == "Favorite.MarkFailed").Count);
    }

    [Fact]
    public void DisplayGraspRead_PortFails_ShowsTheReadFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);

        Assert.Equal(0, wing.CWingDisplay.TDisplayGraspRead(1));
        Assert.Equal(["Grasp.ReadFailed"], asked);
    }

    [Fact]
    public void DisplayFrequencyRead_PortFails_ShowsTheReadFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);

        Assert.Null(wing.CWingDisplay.TDisplayFrequencyRead(1, "once in {0} words"));
        Assert.Equal(["Frequency.ReadFailed"], asked);
    }

    private static CAtelier TDisplayAtelierCreate(LEngine engine) =>
        TInterfaceConduct.TAtelierCreate(engine, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineEntryLoad"] = args => engine.TEngineEntryLoad((long)args![0]!),
        });

    private static CWing TDisplayWingOpen(LEngine engine, CAtelier atelier, List<string> asked)
    {
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []));
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        wing.CWingEntryOpen(water.LEntryId);
        return wing;
    }
}
