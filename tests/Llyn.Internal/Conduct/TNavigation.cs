using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TNavigation
{
    private static readonly string[] TNavigationTabs = ["Input", "Library", "Corpus"];

    [Fact]
    public void NavigationTabRead_NothingStored_ReadsFirstTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);

        Assert.Equal("Input", navigation.CNavigationTabRead());
    }

    [Fact]
    public void NavigationTabOpen_NothingStored_AnswersNothing()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);

        Assert.Null(navigation.CNavigationTabOpen(_ => true));
    }

    [Fact]
    public void NavigationTabOpen_StoredTab_AnswersItAgain()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);
        navigation.CNavigationTabSelect("Corpus", false, _ => true);

        Assert.Equal("Corpus", navigation.CNavigationTabOpen(_ => true));
        Assert.Equal("Corpus", navigation.CNavigationTabRead());
    }

    [Fact]
    public void NavigationTabOpen_StoredTabNotAllowed_FallsBackToFirstTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);
        navigation.CNavigationTabSelect("Corpus", false, _ => true);

        Assert.Equal("Input", navigation.CNavigationTabOpen(tab => tab != "Corpus"));
        Assert.Equal("Input", navigation.CNavigationTabRead());
    }

    [Fact]
    public void NavigationTabSelect_TabClick_AsksOnlyTheOpenTab()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);
        navigation.CNavigationTabSelect("Library", false, _ => true);

        bool selected = navigation.CNavigationTabSelect("Corpus", false, tab =>
        {
            asked.Add(tab);
            return true;
        });

        Assert.True(selected);
        Assert.Equal(["Library"], asked);
        Assert.Equal("Corpus", navigation.CNavigationTabRead());
    }

    [Fact]
    public void NavigationTabSelect_Arriving_AsksTheTargetFirst()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);
        navigation.CNavigationTabSelect("Library", false, _ => true);

        bool selected = navigation.CNavigationTabSelect("Corpus", true, tab =>
        {
            asked.Add(tab);
            return true;
        });

        Assert.True(selected);
        Assert.Equal(["Corpus", "Library"], asked);
    }

    [Fact]
    public void NavigationTabSelect_OpenTabRefuses_KeepsTheOpenTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);
        navigation.CNavigationTabSelect("Library", false, _ => true);

        bool selected = navigation.CNavigationTabSelect("Corpus", false, tab => tab != "Library");

        Assert.False(selected);
        Assert.Equal("Library", navigation.CNavigationTabRead());
    }

    [Fact]
    public void NavigationTabSelect_TargetRefusesArrival_KeepsTheOpenTab()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CNavigation navigation = CNavigation.CNavigationCreate(atelier, TNavigationTabs);
        navigation.CNavigationTabSelect("Library", false, _ => true);

        bool selected = navigation.CNavigationTabSelect("Corpus", true, tab => tab != "Corpus");

        Assert.False(selected);
        Assert.Equal("Library", navigation.CNavigationTabRead());
    }
}
