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
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);

        CGuildRoll roll = guild.CGuildRollRead();
        IReadOnlyList<CCatalogAuthor> rows = roll.CGuildRollRows;

        Assert.Equal([0L, ada.LAuthorId], rows.Select(row => row.CCatalogAuthorId));
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.Uncredited"), rows[0].CCatalogAuthorName);
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.WorkOne"), rows[1].CCatalogAuthorWork);
        Assert.False(roll.CGuildRollEmpty);
    }

    [Fact]
    public void GuildQuerySet_Muster_NarrowsAndDropsOrphanRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Orphan");
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));

        guild.CGuildPanel.CPanelAperture.CApertureQuerySet("Bo");

        Assert.Equal(["Bob"], guild.CGuildRollRead().CGuildRollRows.Select(row => row.CCatalogAuthorName));
    }

    [Fact]
    public void GuildOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        guild.CGuildPanel.CPanelAperture.CApertureOrderSet(CCatalogOrder.CCatalogOrderReverse);
        guild.CGuildPanel.CPanelAperture.CApertureOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, guild.CGuildPanel.CPanelAperture.CApertureOrder);
    }

    [Fact]
    public void GuildFilterSet_HiddenKind_MarksTheRollFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        guild.CGuildPanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter(["Book"]));

        Assert.True(guild.CGuildPanel.CPanelAperture.CApertureFiltered);
        Assert.Equal(["Book"], guild.CGuildPanel.CPanelAperture.CApertureFilter.CCatalogFilterHidden);
    }

    [Fact]
    public void GuildRollRead_NothingChosen_ReadsTheVitaOfNobody()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));

        CVita vita = guild.CGuildRollRead().CGuildRollVita;

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
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        engine.TRequestCreditApply(book.LReferenceId, bob.LAuthorId, 1);

        guild.CGuildAuthorSelect(ada.LAuthorId);
        CVita vita = guild.CGuildRollRead().CGuildRollVita;

        Assert.True(guild.CGuildDiptych.CDiptychParentShown);
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
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Orphan");

        guild.CGuildAuthorSelect(0);

        Assert.False(guild.CGuildVitaHeld);
        Assert.False(guild.CGuildModeEnabled);
        Assert.False(guild.CGuildBinEnabled);
        Assert.Contains("Orphan", guild.CGuildOeuvre.COeuvreRowsRead().Select(row => row.CCatalogReferenceName));
    }

    [Fact]
    public void GuildAuthorSelect_Click_RecordsTheStationItLeaves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        atelier.CAtelierNavigation.CNavigationTabSelect("Guild");
        guild.CGuildAuthorSelect(ada.LAuthorId);
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;

        guild.CGuildAuthorSelect(bob.LAuthorId);

        Assert.Equal(new CVoyageState(true, false), Assert.Single(states).CNavigationStateVoyage);
        Assert.Equal("Bob", guild.CGuildRollRead().CGuildRollVita.CVitaName);
    }

    [Fact]
    public void GuildSourceSelect_Oeuvre_ShowsColophonAndDisablesMode()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        CColophon? shown = null;
        guild.CGuildOeuvre.COeuvreColophonChanged += colophon => shown = colophon;
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildSourceSelect(book.LReferenceId);

        Assert.True(guild.CGuildDiptych.CDiptychChildSide);
        Assert.False(guild.CGuildDiptych.CDiptychParentShown);
        Assert.False(guild.CGuildModeEnabled);
        Assert.Equal("Book", shown?.CColophonTitle);
    }

    [Fact]
    public void GuildRollRead_ChosenAuthorGone_ClosesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        engine.TEngineAuthorDelete(ada.LAuthorId, false);

        IReadOnlyList<CCatalogAuthor> rows = guild.CGuildRollRead().CGuildRollRows;

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
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildDiptych.CDiptychEntryDelete();

        Assert.Equal(["Guild.DeleteConfirm"], asked);
        Assert.Null(engine.TEngineAuthorRead(ada.LAuthorId));
        Assert.False(guild.CGuildBinEnabled);
    }

    [Fact]
    public void GuildWorkspaceNotice_ChosenAuthor_EmptiesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.False(guild.CGuildBinEnabled);
        Assert.False(guild.CGuildVitaHeld);
    }

    [Fact]
    public void GuildAuthorNotice_Catalog_RefreshesTheRollAndTheVerdicts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int refreshed = 0;
        int changed = 0;
        guild.CGuildPanel.CPanelAperture.CApertureRowsChanged += () => refreshed++;
        guild.CGuildChanged += () => changed++;

        engine.TEngineBulletinRaise(LSubject.LSubjectAuthor, 0);

        Assert.Equal(1, refreshed);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void GuildPortraitPrint_AuthorSide_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        Task printed = guild.CGuildPortraitPrint();

        Assert.Same(Task.CompletedTask, printed);
        Assert.False(guild.CGuildDiptych.CDiptychChildSide);
    }

    internal static CGuild TGuildPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CGuild guild = CGuild.CGuildCreate(atelier, static () => true, envoy, static run => run());
        return guild;
    }
}
