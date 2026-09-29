using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuild
{
    [Fact]
    public void GuildRollRead_UncreditedSource_LeadsWithOrphanRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);

        IReadOnlyList<CCatalogAuthor> rows = guild.CGuildRollRead();

        Assert.Equal([0L, ada.LAuthorId], rows.Select(row => row.CCatalogAuthorId));
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.Uncredited"), rows[0].CCatalogAuthorName);
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.WorkOne"), rows[1].CCatalogAuthorWork);
        Assert.False(guild.CGuildEmpty);
    }

    [Fact]
    public void GuildQuerySet_Muster_NarrowsAndDropsOrphanRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Orphan");
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));

        guild.CGuildQuerySet("Bo");

        Assert.Equal(["Bob"], guild.CGuildRollRead().Select(row => row.CCatalogAuthorName));
    }

    [Fact]
    public void GuildOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        guild.CGuildOrderSet(CCatalogOrder.CCatalogOrderReverse);
        guild.CGuildOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, guild.CGuildPanel.CPanelOrder);
    }

    [Fact]
    public void GuildFilterSet_HiddenKind_MarksTheRollFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        guild.CGuildFilterSet(new CCatalogFilter(["Book"]));

        Assert.True(guild.CGuildFiltered);
        Assert.Equal(["Book"], guild.CGuildPanel.CPanelFilter.CCatalogFilterHidden);
    }

    [Fact]
    public void GuildVitaRead_NothingChosen_ReadsNobody()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));

        CVita vita = guild.CGuildVitaRead();

        Assert.False(vita.CVitaNamed);
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.Unnamed"), vita.CVitaName);
        Assert.Empty(vita.CVitaFellows);
        Assert.False(guild.CGuildVitaHeld);
    }

    [Fact]
    public void GuildAuthorSelect_Author_ShowsVitaWithTally()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        engine.TRequestCreditApply(book.LReferenceId, bob.LAuthorId, 1);

        guild.CGuildAuthorSelect(ada.LAuthorId);
        CVita vita = guild.CGuildVitaRead();

        Assert.True(guild.CGuildVitaShown);
        Assert.True(guild.CGuildVitaHeld);
        Assert.True(guild.CGuildModeEnabled);
        Assert.True(guild.CGuildBinEnabled);
        Assert.Equal("Ada", vita.CVitaName);
        Assert.True(vita.CVitaNamed);
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.WorkOne"), vita.CVitaWork);
        Assert.Equal(["Bob"], vita.CVitaFellows.Select(fellow => fellow.CFellowName));
        Assert.Equal(["Book"], guild.CGuildOeuvre.COeuvreRowsRead().Select(row => row.CCatalogReferenceName));
    }

    [Fact]
    public void GuildAuthorSelect_OrphanRow_ListsUncreditedWithoutVita()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Orphan");

        guild.CGuildAuthorSelect(0);

        Assert.False(guild.CGuildVitaHeld);
        Assert.False(guild.CGuildModeEnabled);
        Assert.False(guild.CGuildBinEnabled);
        Assert.Contains("Orphan", guild.CGuildOeuvre.COeuvreRowsRead().Select(row => row.CCatalogReferenceName));
    }

    [Fact]
    public void GuildAuthorSelect_UnsavedDraftKept_StaysOnTheAutograph()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorCreate();
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Bob"));

        guild.CGuildAuthorSelect(ada.LAuthorId);

        Assert.Equal(["Leave"], asked);
        Assert.True(guild.CGuildAutographShown);
        Assert.False(guild.CGuildVitaHeld);
    }

    [Fact]
    public void GuildAuthorOpen_UnsavedDraft_OpensWithoutAsking()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorCreate();
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Bob"));

        guild.TGuildAuthorOpen(ada.LAuthorId);

        Assert.Empty(asked);
        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildBinEnabled);
        Assert.Equal("Ada", guild.CGuildVitaRead().CVitaName);
    }

    [Fact]
    public void GuildAuthorSelect_Click_RecordsTheStationItLeaves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        atelier.CAtelierNavigation.CNavigationTabSelect("Guild");
        guild.CGuildAuthorSelect(ada.LAuthorId);
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;

        guild.CGuildAuthorSelect(bob.LAuthorId);

        Assert.Equal(new CVoyageState(true, false), Assert.Single(states).CNavigationStateVoyage);
        Assert.Equal("Bob", guild.CGuildVitaRead().CVitaName);
    }

    [Fact]
    public void GuildAuthorSelect_UnsavedDraftKept_RecordsNoStation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        atelier.CAtelierNavigation.CNavigationTabSelect("Guild");
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Zed"));
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;

        guild.CGuildAuthorSelect(bob.LAuthorId);

        Assert.Equal(["Leave"], asked);
        Assert.Empty(states);
        Assert.Equal(ada.LAuthorId, guild.CGuildPanel.TPanelChosenRead());
    }

    [Fact]
    public void GuildSourceSelect_Oeuvre_ShowsColophonAndDisablesMode()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        CColophon? shown = null;
        guild.CGuildOeuvre.COeuvreColophonChanged += colophon => shown = colophon;
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildSourceSelect(book.LReferenceId);

        Assert.True(guild.CGuildColophonShown);
        Assert.False(guild.CGuildVitaShown);
        Assert.False(guild.CGuildModeEnabled);
        Assert.True(guild.CGuildPressAllowed);
        Assert.Equal("Book", shown?.CColophonTitle);
    }

    [Fact]
    public void GuildScribeToggle_NoAuthorChosen_ClosesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Orphan");
        guild.CGuildAuthorSelect(0);

        guild.CGuildScribeToggle(true);

        Assert.False(guild.CGuildAutographShown);
        Assert.False(guild.CGuildPanel.CPanelBinEnabled);
        Assert.False(guild.CGuildAutograph.CDeskHeld);
    }

    [Fact]
    public void GuildScribeRestore_StoredAuthor_OpensTheAutographOverIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));

        guild.TGuildScribeRestore(true);

        Assert.False(guild.CGuildAutographShown);

        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.TGuildScribeRestore(true);

        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildUnionShown);
    }

    [Fact]
    public void GuildSessionSave_FreshAuthor_RefusesBlankThenStoresNamed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));

        guild.CGuildAuthorCreate();

        Assert.True(guild.CGuildAutographShown);
        Assert.False(guild.CGuildUnionShown);
        Assert.False(guild.CGuildStoreEnabled);
        Assert.False(guild.CGuildSession.CSessionSave());
        Assert.Equal(["Guild.NameBlank"], asked);

        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Ada"));

        Assert.True(guild.CGuildStoreEnabled);
        Assert.True(guild.CGuildSession.CSessionSave());
        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildUnionShown);
        Assert.True(guild.CGuildAutograph.CDeskHeld);
        Assert.Equal(
            ["Ada"],
            guild.CGuildRollRead().Where(row => row.CCatalogAuthorChosen).Select(row => row.CCatalogAuthorName));
    }

    [Fact]
    public void GuildUnionRead_TypedName_ExcludesSelf()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildUnionShown);
        Assert.Equal(["Adam"], guild.CGuildUnionRead("Ad").Select(row => row.CCatalogAuthorName));
        Assert.Empty(guild.CGuildUnionRead(" "));
    }

    [Fact]
    public void GuildUnionSelect_Confirmed_AbsorbsAndShowsKept()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        guild.CGuildUnionSelect(adam.LAuthorId);

        Assert.Equal(["Guild.MergeConfirm>Ada>Adam"], asked);
        Assert.Null(engine.TEngineAuthorRead(ada.LAuthorId));
        Assert.False(guild.CGuildAutographShown);
        Assert.True(guild.CGuildVitaShown);
        Assert.Equal("Adam", guild.CGuildVitaRead().CVitaName);
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.WorkOne"), guild.CGuildVitaRead().CVitaWork);
        Assert.False(guild.CGuildAutograph.CDeskHeld);
    }

    [Fact]
    public void GuildUnionSelect_PaddedNames_AsksWithBothTrimmed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, " Adam  "));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "  Ada B "));

        guild.CGuildUnionSelect(adam.LAuthorId);

        Assert.Equal(["Guild.MergeConfirm>Ada B>Adam"], asked);
    }

    [Fact]
    public void GuildUnionSelect_Declined_KeepsBothAuthors()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        guild.CGuildUnionSelect(adam.LAuthorId);

        Assert.NotNull(engine.TEngineAuthorRead(ada.LAuthorId));
        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildAutograph.CDeskHeld);
    }

    [Fact]
    public void GuildRollRead_ChosenAuthorGone_ClosesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        engine.TEngineAuthorDelete(ada.LAuthorId, false);

        IReadOnlyList<CCatalogAuthor> rows = guild.CGuildRollRead();

        Assert.DoesNotContain(rows, row => row.CCatalogAuthorId == ada.LAuthorId);
        Assert.False(guild.CGuildBinEnabled);
    }

    [Fact]
    public void GuildAuthorDelete_Confirmed_RemovesTheAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildAuthorDelete();

        Assert.Equal(["Guild.DeleteConfirm"], asked);
        Assert.Null(engine.TEngineAuthorRead(ada.LAuthorId));
        Assert.False(guild.CGuildBinEnabled);
    }

    [Fact]
    public void GuildAuthorClose_ChosenAuthor_EmptiesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildAuthorClose();

        Assert.False(guild.CGuildBinEnabled);
        Assert.False(guild.CGuildVitaHeld);
    }

    [Fact]
    public void GuildCatalogResonate_Notice_RefreshesTheRollAndTheVerdicts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        int refreshed = 0;
        int changed = 0;
        guild.CGuildPanel.CPanelRowsChanged += () => refreshed++;
        guild.CGuildChanged += () => changed++;

        guild.CGuildCatalogResonate();

        Assert.Equal(1, refreshed);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void GuildPortraitPrint_AuthorSide_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        Task printed = guild.CGuildPortraitPrint();

        Assert.Same(Task.CompletedTask, printed);
        Assert.False(guild.CGuildPressAllowed);
    }

    internal static CGuild TGuildPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CGuild guild = CGuild.CGuildCreate(atelier, static () => true, envoy);
        guild.CGuildVistaRestore();
        return guild;
    }
}
