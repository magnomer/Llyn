using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TQuillSituation
{
    [Fact]
    public void SituationChange_UnknownKindLeftEmpty_KeepsKindUnknown()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation stored = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "Hearth", null, LStateValue.LStateValueUnknown));
        LDesk desk = TInterfaceDeportment.TDeskCreate(engine, "Situation", static () => false);

        desk.TDeskSituationStart(stored.LSituationId);
        desk.TQuillSituationChange("Fire", string.Empty, string.Empty);

        LSituation held = desk.TDeskRead()!.LDraftSituation!;
        Assert.Equal("Fire", held.LSituationTitle.TStateValueShow());
        Assert.True(held.LSituationKind.LStateValueUncertain);
        Assert.True(held.LSituationDescription.LStateValueEmpty);
    }

    [Fact]
    public void SituationChange_KindTyped_WritesKindAsTyped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation stored = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "Hearth", null, LStateValue.LStateValueUnknown));
        LDesk desk = TInterfaceDeportment.TDeskCreate(engine, "Situation", static () => false);

        desk.TDeskSituationStart(stored.LSituationId);
        desk.TQuillSituationChange("Hearth", "By the fire", "Ritual");

        LSituation held = desk.TDeskRead()!.LDraftSituation!;
        Assert.Equal("Ritual", held.LSituationKind.TStateValueShow());
        Assert.False(held.LSituationKind.LStateValueUncertain);
        Assert.Equal("By the fire", held.LSituationDescription.TStateValueShow());
        Assert.True(desk.TDeskChangeCheck());
    }
}
