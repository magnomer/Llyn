using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCharter
{
    [Fact]
    public void CharterRead_NoDraft_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CCharterState? state = deck.CDeckCharter.CCharterRead();

        Assert.Null(state);
    }

    [Fact]
    public void CharterRead_LeadSet_ReadsDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterLeadSet((int)LBearingLead.LBearingLeadAlternate - 1);

        CCharterState state = deck.CDeckCharter.CCharterRead()!;
        Assert.Equal((int)LBearingLead.LBearingLeadAlternate - 1, state.CCharterStateLead);
        Assert.Equal(
            ["Bearing.Forward", "Bearing.Backward"],
            state.CCharterStateBearings.Select(static bearing => bearing.CCharterBearingCaption));
        Assert.NotEqual(
            LBearingLead.LBearingLeadAlternate,
            port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckLead);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterLeadSet_Alternate_SavesLead()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterLeadSet((int)LBearingLead.LBearingLeadAlternate - 1);
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(
            LBearingLead.LBearingLeadAlternate,
            port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckLead);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterBasicSet_On_SavesBasic()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterBasicSet(true);
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.True(port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckBasic);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterOrderSet_Random_SavesOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterOrderSet((int)LAlmanacOrder.LAlmanacOrderRandom - 1);
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(
            LAlmanacOrder.LAlmanacOrderRandom,
            port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckSchedule.LDeckScheduleOrder);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterGoalSet_Digits_SavesGoal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterGoalSet("7");
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(7, port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckSchedule.LDeckScheduleGoal);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterSpacedSet_Digits_SavesSpaced()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterSpacedSet("30");
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(30, port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckSchedule.LDeckScheduleSpaced);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterPoolSet_Digits_SavesPool()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterPoolSet("5");
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(5, port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckSchedule.LDeckSchedulePool);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterStreakSet_Digits_SavesStreak()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterStreakSet("4");
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(4, port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckSchedule.LDeckScheduleStreak);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterTargetSet_Letters_KeepsTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterTargetSet("soon");
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(
            TInterface.TDeckScheduleCreate().LDeckScheduleTarget,
            port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckSchedule.LDeckScheduleTarget);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterQuorumSet_Digits_SavesQuorum()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterQuorumSet("3");
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(3, port.TEngineDeckRead(stored)!.Value.LDeckStored.LDeckSchedule.LDeckScheduleQuorum);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterFrontToggle_SideTwo_SavesFront()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterFrontToggle(1, 1);
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(
            3,
            port.TEngineDeckRead(stored)!.Value.LDeckBearings
                .Single(static bearing => bearing.LBearingDirection == 1)
                .LBearingFront);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterBackToggle_SideTwo_SavesBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterBackToggle(2, 1);
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(
            3,
            port.TEngineDeckRead(stored)!.Value.LDeckBearings
                .Single(static bearing => bearing.LBearingDirection == 2)
                .LBearingBack);
        Assert.Empty(asked);
    }

    [Fact]
    public void CharterStatisticsSet_Digits_SavesStatistics()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long stored = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        List<string> asked = [];
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        deck.CDeckSelect(stored);

        deck.CDeckCharter.CCharterStatisticsSet(2, "3");
        bool saved = deck.CDeckCharter.CCharterSave();

        Assert.True(saved);
        Assert.Equal(
            3,
            port.TEngineDeckRead(stored)!.Value.LDeckBearings
                .Single(static bearing => bearing.LBearingDirection == 2)
                .LBearingStatistics);
        Assert.Empty(asked);
    }
}
