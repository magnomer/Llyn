using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TEstablishment
{
    private static readonly CEstablishment TEstablishmentSaved =
        new(0, 12, false, "Establishment.Entry", "Establishment.Kilobyte", "3");

    private static readonly CCourierState TEstablishmentIdle = new(false, true, false, string.Empty);

    [Fact]
    public void EstablishmentDraw_NothingHeard_ShowsOpening()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();

        Assert.Equal(
            " Opening…".PadRight(80), TEstablishmentFixture.TEstablishmentDraw(establishment, TEstablishmentIdle));
    }

    [Fact]
    public void EstablishmentDraw_NoToken_ShowsConnectAndMutedTally()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved);

        IReadOnlyList<BCanvasText> texts =
            establishment.BEstablishmentDraw(
                TEstablishmentIdle, TEstablishmentFixture.TEstablishmentGlyphs, new BCanvasWidth(false));

        Assert.Equal(
            " Joplin: Connect".PadRight(62) + "12 entries · 3 KB ",
            TEstablishmentFixture.TEstablishmentRowBuild(texts));
        Assert.Equal("muted", texts.Single(static text => text.BCanvasTextValue.Contains('·')).BCanvasTextRole);
    }

    [Fact]
    public void EstablishmentDraw_TokenStored_ShowsConnected()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved);

        string row = TEstablishmentFixture.TEstablishmentDraw(
            establishment, TEstablishmentIdle with { CCourierStateAttached = true });

        Assert.StartsWith(" Joplin: Connected ", row, StringComparison.Ordinal);
    }

    [Fact]
    public void EstablishmentDraw_CourierLine_ShowsThenClears()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved);

        string shown = TEstablishmentFixture.TEstablishmentDraw(
            establishment, new CCourierState(true, false, false, "Sending entries to Joplin…"));
        string cleared = TEstablishmentFixture.TEstablishmentDraw(establishment, TEstablishmentIdle);

        Assert.StartsWith(" Joplin: Connect / Sending entries to Joplin… ", shown, StringComparison.Ordinal);
        Assert.DoesNotContain("/", cleared, StringComparison.Ordinal);
    }

    [Fact]
    public void EstablishmentDraw_LongCourierLine_CutsKeepingRightEnd()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(
            TEstablishmentSaved with { CEstablishmentUnsaved = 2, CEstablishmentPending = true });

        string row = TEstablishmentFixture.TEstablishmentDraw(
            establishment, TEstablishmentIdle with { CCourierStateLine = new string('x', 120) });

        string head = " Joplin: Connect / ";
        string mark = "  ● 2 unsaved";
        string tally = "12 entries · 3 KB ";
        int room = 80 - tally.Length - 2 - mark.Length - head.Length;
        Assert.Equal(head + new string('x', room - 1) + "…" + mark + "  " + tally, row);
    }

    [Fact]
    public void EstablishmentDraw_NoRoom_KeepsEllipsis()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        BEstablishment roomy = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved with { CEstablishmentAmount = new string('9', 41) });
        roomy.BEstablishmentObserve(TEstablishmentSaved with { CEstablishmentAmount = new string('9', 40) });

        CCourierState sending = TEstablishmentIdle with { CCourierStateLine = "Sending" };
        string row = TEstablishmentFixture.TEstablishmentDraw(establishment, sending);
        string fitted = TEstablishmentFixture.TEstablishmentDraw(roomy, sending);

        string tally = $"12 entries · {new string('9', 41)} KB ";
        string wider = $"12 entries · {new string('9', 40)} KB ";
        Assert.Equal(" Joplin: Connect / …".PadRight(80 - tally.Length) + tally, row);
        Assert.Equal(" Joplin: Connect / S…".PadRight(80 - wider.Length) + wider, fitted);
    }

    [Fact]
    public void EstablishmentDraw_HugeAmount_LeavesEllipsisVisible()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved with { CEstablishmentAmount = new string('9', 50) });

        string row = TEstablishmentFixture.TEstablishmentDraw(
            establishment, TEstablishmentIdle with { CCourierStateLine = "Sending" });

        string head = " Joplin: Connect / …";
        string tally = $"12 entries · {new string('9', 50)} KB";
        Assert.Equal(head + tally[(head.Length - (79 - tally.Length))..] + " ", row);
    }

    [Fact]
    public void EstablishmentDraw_WideFirstCluster_KeepsEllipsisInRoom()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved with { CEstablishmentAmount = new string('9', 40) });

        string row = TEstablishmentFixture.TEstablishmentDraw(
            establishment, TEstablishmentIdle with { CCourierStateLine = "전송" });

        string tally = $"12 entries · {new string('9', 40)} KB ";
        Assert.Equal(" Joplin: Connect / …".PadRight(80 - tally.Length) + tally, row);
    }

    [Fact]
    public void EstablishmentDraw_Unsaved_ShowsPendingMarkAndCount()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(
            TEstablishmentSaved with { CEstablishmentUnsaved = 4, CEstablishmentPending = true });

        IReadOnlyList<BCanvasText> texts =
            establishment.BEstablishmentDraw(
                TEstablishmentIdle, TEstablishmentFixture.TEstablishmentGlyphs, new BCanvasWidth(false));

        Assert.StartsWith(
            " Joplin: Connect  ● 4 unsaved ",
            TEstablishmentFixture.TEstablishmentRowBuild(texts),
            StringComparison.Ordinal);
        Assert.Equal("pending", texts.Single(static text => text.BCanvasTextValue == "●").BCanvasTextRole);
    }

    [Fact]
    public void EstablishmentDraw_NoUnsaved_HidesPending()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved);

        string row = TEstablishmentFixture.TEstablishmentDraw(establishment, TEstablishmentIdle);

        Assert.DoesNotContain("unsaved", row, StringComparison.Ordinal);
        Assert.DoesNotContain("●", row, StringComparison.Ordinal);
    }

    [Fact]
    public void EstablishmentDraw_NoNewRead_KeepsLastNumbers()
    {
        BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
        establishment.BEstablishmentObserve(TEstablishmentSaved);
        establishment.BEstablishmentObserve(
            TEstablishmentSaved with { CEstablishmentEntry = 1, CEstablishmentEntryKey = "Establishment.EntryOne" });

        string first = TEstablishmentFixture.TEstablishmentDraw(establishment, TEstablishmentIdle);
        string again = TEstablishmentFixture.TEstablishmentDraw(establishment, TEstablishmentIdle);

        Assert.EndsWith("1 entry · 3 KB ", first, StringComparison.Ordinal);
        Assert.Equal(first, again);
    }

    [Fact]
    public void HeadquarterBuild_NoToken_ShowsConnectAndSyncAboveRule()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        atelier.TAtelierStubOpen();

        BRoofMenu menu = TEstablishmentFixture.TEstablishmentMenuBuild(courier);

        Assert.Equal(
            ["Connect to Joplin", "Sync to Joplin", null, "About Llyn"],
            menu.BRoofMenuItems.Take(4).Select(static item => item?.BRoofItemLabel));
        Assert.True(menu.BRoofMenuItems[0]!.BRoofItemLive());
        Assert.True(menu.BRoofMenuItems[1]!.BRoofItemLive());
    }

    [Fact]
    public void HeadquarterBuild_TokenStored_HidesConnect()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"warrant\": \"fake:token\" }");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        atelier.TAtelierStubOpen();

        BRoofMenu menu = TEstablishmentFixture.TEstablishmentMenuBuild(courier);

        Assert.Equal(
            ["Sync to Joplin", null, "About Llyn"],
            menu.BRoofMenuItems.Take(3).Select(static item => item?.BRoofItemLabel));
    }

    [Fact]
    public async Task HeadquarterBuild_CourierBusy_DimsJoplinItems()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        atelier.TAtelierStubOpen();
        BRoofMenu menu = TEstablishmentFixture.TEstablishmentMenuBuild(courier);
        List<(bool, bool, bool, bool)> seen = [];
        courier.CCourierChanged += state => seen.Add((
            state.CCourierStateBusy,
            menu.BRoofMenuItems[0]!.BRoofItemLive(),
            menu.BRoofMenuItems[1]!.BRoofItemLive(),
            menu.BRoofMenuItems[3]!.BRoofItemLive()));

        await courier.CCourierSend(TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Contains((true, false, false, true), seen);
        Assert.Equal((false, true, true, true), seen[^1]);
    }

    [Fact]
    public void HeadquarterBuild_NoCourier_LeavesJoplinOut()
    {
        BRoofMenu menu = TEstablishmentFixture.TEstablishmentMenuBuild(null);

        Assert.Equal("About Llyn", menu.BRoofMenuItems[0]?.BRoofItemLabel);
    }

    [Fact]
    public void WindowOpen_Workspace_ShowsStatusLine()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.StartsWith(" Joplin: Connect ", console.TConsoleFakeShots[0][22], StringComparison.Ordinal);
        Assert.Matches(" [0-9]+ entries · [0-9]+ KB $", console.TConsoleFakeShots[0][22]);
    }

    [Fact]
    public void MirrorOpen_FailedWorkspace_SeedsCourierState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        SynchronizationContext? context = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new BLoop(static _ => { }));
        try
        {
            BMirror mirror = TEstablishmentFixture.TEstablishmentMirrorCreate(atelier, courier);
            BEstablishment establishment = TEstablishmentFixture.TEstablishmentCreate();
            establishment.BEstablishmentObserve(TEstablishmentSaved);
            mirror.BMirrorOpen(atelier, courier);

            mirror.BMirrorRoof.BRoofOpen(0);
            IReadOnlyList<BCanvasText> texts =
                mirror.BMirrorRoof.BRoofDraw(TEstablishmentFixture.TEstablishmentGlyphs, new BCanvasWidth(false));
            Assert.Equal(new CCourierState(false, true, false, string.Empty), mirror.BMirrorCourier);
            Assert.StartsWith(
                " Joplin: Connect ", TEstablishmentFixture.TEstablishmentDraw(establishment, mirror.BMirrorCourier!));
            Assert.Contains(
                texts, static text => text.BCanvasTextValue.Contains("Connect to Joplin", StringComparison.Ordinal));
            Assert.Contains(
                texts, static text => text.BCanvasTextValue.Contains("Sync to Joplin", StringComparison.Ordinal));
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }

    [Fact]
    public void MirrorCourierObserve_MenuOpen_KeepsMenuOpen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"warrant\": \"fake:token\" }");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);
        SynchronizationContext? context = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new BLoop(static _ => { }));
        try
        {
            BMirror mirror = TEstablishmentFixture.TEstablishmentMirrorCreate(atelier, courier);
            mirror.BMirrorOpen(atelier, courier);
            mirror.BMirrorRoof.BRoofOpen(0);
            _ = mirror.BMirrorRoof.BRoofKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
            _ = mirror.BMirrorRoof.BRoofKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

            atelier.TAtelierStubOpen();

            IReadOnlyList<BCanvasText> texts =
                mirror.BMirrorRoof.BRoofDraw(TEstablishmentFixture.TEstablishmentGlyphs, new BCanvasWidth(false));
            Assert.True(courier.CCourierRead().CCourierStateAttached);
            Assert.Equal(BKeymapFocus.BKeymapFocusRoof, mirror.BMirrorRoof.BRoofKeymapFocus);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "lit-other"
                && text.BCanvasTextValue.Contains("About Llyn", StringComparison.Ordinal));
            Assert.DoesNotContain(
                texts, static text => text.BCanvasTextValue.Contains("Connect to Joplin", StringComparison.Ordinal));
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
