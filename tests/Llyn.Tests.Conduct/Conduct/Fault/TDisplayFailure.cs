using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayFailure
{
    [Theory]
    [InlineData("Display.StampFailed")]
    [InlineData("Sound.StartFailed")]
    public void DisplayEntryOpen_PortFailure_ShowsFailureNotice(string notice)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        TDisplayWingOpen(engine, atelier, asked);

        Assert.Contains(notice, asked);
    }

    [Theory]
    [InlineData("Display.OrderFailed")]
    [InlineData("Display.CitationFailed")]
    public void DisplayCardRead_PortFailure_ShowsFailureNotice(string notice)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        Assert.Contains(notice, asked);
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

    [Theory]
    [InlineData("Display.LanguageFailed")]
    [InlineData("Sound.PendingFailed")]
    public void DisplayParadigmRead_PortFailure_ShowsFailureNotice(string notice)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();

        Assert.Contains(notice, asked);
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
    public void DisplayFanqieRead_AnchorPortFails_ShowsTheAnchorFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        CLecternFanqie fanqie = wing.CWingDisplay.CDisplaySound.CDisplayFanqieRead();
        wing.CWingDisplay.CDisplaySound.CDisplayReflexRead();

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

    [Theory]
    [InlineData("Display.OrderFailed")]
    [InlineData("Fold.ReadFailed")]
    public void DisplayCardRead_RepeatedPortFailure_ShowsOneNotice(string notice)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TDisplayAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingOpen(engine, atelier, asked);
        asked.Clear();

        wing.CWingDisplay.CDisplayCard.CDisplayCardRead();
        wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        Assert.Single(asked, key => key == notice);
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

        wing.CWingDisplay.CDisplayGrasp.CDisplayFavoriteToggle();
        wing.CWingDisplay.CDisplayGrasp.CDisplayFavoriteToggle();

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
            ["LEngineCollocationRead"] = args => engine.TEngineCollocationRead(
                (LEntryDraft)args![0]!,
                (IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>>)args[1]!),
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
