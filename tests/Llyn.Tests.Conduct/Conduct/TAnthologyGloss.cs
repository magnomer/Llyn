using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnthologyGloss
{
    [Fact]
    public void AnthologyGlossAdd_RowBelow_PlacesTheGlossInTheGlossLanguageAfterIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "French", 0));
        desk.CDeskPersist();
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "German", 1));
        desk.CDeskPersist();

        anthology.CAnthologyGlossAdd(0);

        Assert.Equal(
            ["French", engine.TEngineGlossRead(), "German"],
            TAnthologyGlossRead(anthology, desk).Select(static row => row.CGlossDraftLanguage));
    }

    [Fact]
    public void AnthologyGlossAdd_BelowTheLastRowAndBeforeTheFirst_PlacesEachAtItsPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "French", 0));
        desk.CDeskPersist();

        anthology.CAnthologyGlossAdd(0);
        anthology.CAnthologyGlossAdd(-1);

        Assert.Equal(
            [engine.TEngineGlossRead(), "French", engine.TEngineGlossRead()],
            TAnthologyGlossRead(anthology, desk).Select(static row => row.CGlossDraftLanguage));
    }

    [Fact]
    public void AnthologyGlossPrepare_EmptyThenHeld_AddsOnlyTheFirstGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        bool first = anthology.CAnthologyGlossPrepare();
        bool second = anthology.CAnthologyGlossPrepare();

        Assert.True(first);
        Assert.False(second);
        CGlossDraft held = Assert.Single(TAnthologyGlossRead(anthology, desk));
        Assert.Equal(engine.TEngineGlossRead(), held.CGlossDraftLanguage);
    }

    [Fact]
    public void AnthologyGlossPrepare_NoTranscriptHeld_AddsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out _);

        Assert.False(anthology.CAnthologyGlossPrepare());
    }

    [Fact]
    public void AnthologyGlossSetAndLanguageSetAndRemove_PickedRows_WriteTheTextRetagAndDropTheGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "French", 0));
        desk.CDeskPersist();
        desk.TDeskDefer(TInterface.TGlossAdditionCreate(desk.CDeskId, 0, 0, "German", 1));
        desk.CDeskPersist();
        IReadOnlyList<CGlossDraft> added = TAnthologyGlossRead(anthology, desk);

        anthology.CAnthologyGlossSet(added[1].CGlossDraftId, "die Katze");
        desk.CDeskPersist();
        anthology.CAnthologyLanguageSet(added[1].CGlossDraftId, "Dutch");
        anthology.CAnthologyGlossRemove(added[0].CGlossDraftId);

        CGlossDraft kept = Assert.Single(TAnthologyGlossRead(anthology, desk));
        Assert.Equal(added[1].CGlossDraftId, kept.CGlossDraftId);
        Assert.Equal("Dutch", kept.CGlossDraftLanguage);
        Assert.Equal("die Katze", kept.CGlossDraftText.CStateValueShown);
    }

    private static IReadOnlyList<CGlossDraft> TAnthologyGlossRead(CAnthology anthology, CDesk desk)
    {
        return anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleGloss ?? [];
    }
}
