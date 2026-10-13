using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TNavigation
{
    [Fact]
    public void NavigationTabOpen_NothingStored_PaintsNoTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        navigation.TNavigationTabOpen();

        CNavigationState state = Assert.Single(states);
        Assert.Null(state.CNavigationStateTab);
        Assert.Empty(state.CNavigationStateHidden);
    }

    [Fact]
    public void NavigationTabOpen_StoredTab_OpensItAgainAndRestoresItsEditor()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<bool> scribed = [];
        navigation.TNavigationTabAdd("Corpus", static () => true, static () => 0, scribed.Add, static _ => { });
        navigation.CNavigationTabSelect("Corpus");
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        navigation.TNavigationTabOpen();

        Assert.Equal("Corpus", Assert.Single(states).CNavigationStateTab);
        Assert.Equal([TInterfaceConduct.TAtelierSplitRead(atelier)], scribed);
    }

    [Fact]
    public void NavigationTabOpen_StoredTabNotAllowed_FallsBackToFirstTabAndHidesIt()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        navigation.TNavigationTabAdd(
            "Corpus", static () => true, static () => 0, static _ => { }, static _ => { }, static () => false);
        navigation.CNavigationTabSelect("Corpus");
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        navigation.TNavigationTabOpen();
        navigation.TNavigationTabOpen();

        Assert.All(states, state => Assert.Equal("Input", state.CNavigationStateTab));
        Assert.All(states, state => Assert.Equal(["Corpus"], state.CNavigationStateHidden));
    }

    [Fact]
    public void NavigationTabSelect_TabClick_AsksOnlyTheOpenTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        List<string> asked = [];
        TNavigationTabAdd(navigation, "Library", asked, true, 0, arrived);
        TNavigationTabAdd(navigation, "Corpus", asked, true, 0, arrived);
        navigation.CNavigationTabSelect("Library");
        asked.Clear();
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool selected = navigation.CNavigationTabSelect("Corpus");

        Assert.True(selected);
        Assert.Equal(["Library"], asked);
        Assert.Equal("Corpus", Assert.Single(states).CNavigationStateTab);
    }

    [Fact]
    public void NavigationTabSelect_OpenTabRefuses_KeepsTheOpenTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        TNavigationTabAdd(navigation, "Library", [], false, 0, arrived);
        navigation.CNavigationTabSelect("Library");
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool selected = navigation.CNavigationTabSelect("Corpus");
        navigation.TNavigationTabOpen();

        Assert.False(selected);
        Assert.Equal("Library", Assert.Single(states).CNavigationStateTab);
    }

    [Fact]
    public void NavigationEntryOpen_Jump_AsksTheTargetFirstRecordsTheStationAndArrives()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        List<string> asked = [];
        TNavigationTabAdd(navigation, "Library", asked, true, 4, arrived);
        TNavigationTabAdd(navigation, "Corpus", asked, true, 9, arrived);
        navigation.CNavigationTabSelect("Corpus");
        asked.Clear();
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool opened = navigation.CNavigationEntryOpen(5);

        Assert.True(opened);
        Assert.Equal(["Library", "Corpus"], asked);
        Assert.Equal([("Library", 5L)], arrived);
        CNavigationState state = Assert.Single(states);
        Assert.Equal("Library", state.CNavigationStateTab);
        Assert.Equal(new CVoyageState(true, false), state.CNavigationStateVoyage);
    }

    [Fact]
    public void NavigationEntryOpen_TargetRefusesOrHasNoPanel_KeepsTheOpenTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        TNavigationTabAdd(navigation, "Library", [], false, 4, arrived);
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool refused = navigation.CNavigationEntryOpen(5);
        bool unregistered = navigation.CNavigationUsageOpen(TNavigationUsageCreate(true));

        Assert.False(refused);
        Assert.False(unregistered);
        Assert.Empty(states);
        Assert.Empty(arrived);
    }

    [Theory]
    [InlineData(false, true, 1)]
    [InlineData(true, false, 2)]
    public void NavigationEntryOpen_LeaveDeclined_AsksFirstAndRecordsNothing(bool target, bool standing, int questions)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        List<string> asked = [];
        TNavigationTabAdd(navigation, "Library", asked, target, 4, arrived);
        TNavigationTabAdd(navigation, "Corpus", asked, standing, 9, arrived);
        navigation.CNavigationTabSelect("Corpus");
        asked.Clear();
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool opened = navigation.CNavigationEntryOpen(5);
        bool back = navigation.CNavigationStationUndo();

        Assert.False(opened);
        Assert.False(back);
        Assert.Equal(new[] { "Library", "Corpus" }.Take(questions), asked);
        Assert.Empty(states);
        Assert.Empty(arrived);
    }

    [Fact]
    public void NavigationStationUndo_RecordedStation_ArrivesThereAndParksTheStandingOne()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        TNavigationTabAdd(navigation, "Library", [], true, 4, arrived);
        TNavigationTabAdd(navigation, "Corpus", [], true, 9, arrived);
        navigation.CNavigationTabSelect("Corpus");
        navigation.CNavigationEntryOpen(4);
        arrived.Clear();
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool back = navigation.CNavigationStationUndo();
        bool forward = navigation.CNavigationStationRedo();
        bool beyond = navigation.CNavigationStationRedo();

        Assert.True(back);
        Assert.True(forward);
        Assert.False(beyond);
        Assert.Equal([("Corpus", 9L), ("Library", 4L)], arrived);
        Assert.Equal(["Corpus", "Library"], states.Select(static state => state.CNavigationStateTab));
        Assert.Equal(new CVoyageState(false, true), states[0].CNavigationStateVoyage);
        Assert.Equal(new CVoyageState(true, false), states[1].CNavigationStateVoyage);
    }

    [Fact]
    public void NavigationStationUndo_LandingTabDeclines_StaysAndKeepsTheTrail()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        List<string> asked = [];
        bool leaves = true;
        TNavigationTabAdd(navigation, "Library", asked, true, 4, arrived);
        navigation.TNavigationTabAdd(
            "Corpus",
            () =>
            {
                asked.Add("Corpus");
                return leaves;
            },
            static () => 9,
            static _ => { },
            id => arrived.Add(("Corpus", id)));
        navigation.CNavigationTabSelect("Corpus");
        navigation.CNavigationEntryOpen(4);
        arrived.Clear();
        asked.Clear();
        leaves = false;
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool refused = navigation.CNavigationStationUndo();

        Assert.False(refused);
        Assert.Equal(["Corpus"], asked);
        Assert.Empty(arrived);
        Assert.Empty(states);

        leaves = true;
        bool back = navigation.CNavigationStationUndo();

        Assert.True(back);
        Assert.Equal(["Corpus", "Corpus", "Library"], asked);
        Assert.Equal([("Corpus", 9L)], arrived);
        CNavigationState state = Assert.Single(states);
        Assert.Equal("Corpus", state.CNavigationStateTab);
        Assert.Equal(new CVoyageState(false, true), state.CNavigationStateVoyage);
    }

    [Fact]
    public void NavigationStationAdd_OpenTab_RecordsItsStationAndPaintsNoTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        TNavigationTabAdd(navigation, "Library", [], true, 4, arrived);
        navigation.CNavigationTabSelect("Library");
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        navigation.TNavigationStationAdd();

        CNavigationState state = Assert.Single(states);
        Assert.Null(state.CNavigationStateTab);
        Assert.Equal(new CVoyageState(true, false), state.CNavigationStateVoyage);
    }

    [Fact]
    public void NavigationUsageOpen_QuotedOrNamedPlace_OpensTheExampleOrTheEntry()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        TNavigationTabAdd(navigation, "Library", [], true, 0, arrived);
        TNavigationTabAdd(navigation, "Corpus", [], true, 0, arrived);

        navigation.CNavigationUsageOpen(TNavigationUsageCreate(true));
        navigation.CNavigationUsageOpen(TNavigationUsageCreate(false));

        Assert.Equal([("Corpus", 3L), ("Library", 9L)], arrived);
    }

    [Fact]
    public void NavigationDiweiOpen_Jump_OpensTheYunjingTabAndHandsTheCellToItsArea()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        List<string> asked = [];
        TNavigationTabAdd(navigation, "Yunjing", asked, true, 0, arrived);
        List<CNavigationState> states = TNavigationStateAttach(navigation);
        List<string> cells = [];
        navigation.TNavigationDiweiAttach((language, kind, key) => cells.Add(language + " " + kind + " " + key));

        bool opened = navigation.TNavigationDiweiOpen("Korean", "initial", "k");

        Assert.True(opened);
        Assert.Equal(["Yunjing"], asked);
        Assert.Equal(["Korean initial k"], cells);
        Assert.Equal("Yunjing", Assert.Single(states).CNavigationStateTab);
    }

    [Fact]
    public void NavigationStemOpen_Jump_OpensTheXieshengTabAndHandsTheSeriesToItsArea()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = atelier.CAtelierNavigation;
        List<(string, long)> arrived = [];
        List<string> asked = [];
        TNavigationTabAdd(navigation, "Xiesheng", asked, true, 0, arrived);
        List<CNavigationState> states = TNavigationStateAttach(navigation);

        bool early = navigation.TNavigationStemOpen("Korean", "k");
        List<string> series = [];
        navigation.TNavigationStemAttach((language, key) => series.Add(language + " " + (key ?? "-")));
        bool opened = navigation.TNavigationStemOpen("Korean", null);

        Assert.False(early);
        Assert.True(opened);
        Assert.Equal(["Xiesheng"], asked);
        Assert.Equal(["Korean -"], series);
        Assert.Equal("Xiesheng", Assert.Single(states).CNavigationStateTab);
        Assert.Empty(arrived);
    }

    private static void TNavigationTabAdd(
        CNavigation navigation, string tab, List<string> asked, bool leave, long station, List<(string, long)> arrived)
    {
        navigation.TNavigationTabAdd(
            tab,
            () =>
            {
                asked.Add(tab);
                return leave;
            },
            () => station,
            static _ => { },
            id => arrived.Add((tab, id)));
    }

    private static List<CNavigationState> TNavigationStateAttach(CNavigation navigation)
    {
        List<CNavigationState> states = [];
        navigation.CNavigationChanged += states.Add;
        return states;
    }

    private static CUsage TNavigationUsageCreate(bool quoted)
    {
        return new CUsage(3, 9, "water", string.Empty, "English", CStateValue.CStateValueEmpty, quoted, false);
    }
}
