using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuildRoll
{
    [Fact]
    public void RollRead_UncreditedSource_LeadsWithOrphanRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);

        IReadOnlyList<LCatalogAuthor> rows = guild.TGuildRollRead();

        Assert.Equal([0L, ada.LAuthorId], rows.Select(row => row.LCatalogAuthorStored.LAuthorId));
        Assert.Equal(1, rows[1].LCatalogAuthorWork);
        Assert.False(guild.LGuildEmpty);
    }

    [Fact]
    public void QuerySet_Muster_NarrowsAndDropsOrphanRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        engine.TEngineCitationCreate("Orphan");
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));

        guild.TGuildQuerySet("Bo");

        Assert.Equal(["Bob"], guild.TGuildRollRead().Select(row => row.LCatalogAuthorName));
    }

    [Fact]
    public void RowSelect_Author_ShowsVitaWithTally()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        engine.TRequestCreditApply(book.LReferenceId, bob.LAuthorId, 1);

        guild.TGuildRowSelect(ada.LAuthorId);
        LVita vita = guild.TGuildVitaRead();

        Assert.True(guild.LGuildVitaShown);
        Assert.True(guild.LGuildVitaHeld);
        Assert.True(guild.LGuildModeEnabled);
        Assert.True(guild.LGuildBinEnabled);
        Assert.Equal("Ada", vita.LVitaName);
        Assert.True(vita.LVitaNamed);
        Assert.Equal("Guild.WorkOne", vita.LVitaWork);
        Assert.Equal(["Bob"], vita.LVitaFellows.Select(fellow => fellow.LFellowName));
        Assert.Equal(["Book"], guild.LGuildOeuvre.TOeuvreRowsRead().Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void RowSelect_OrphanRow_ListsUncreditedWithoutVita()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        engine.TEngineCitationCreate("Orphan");

        guild.TGuildRowSelect(0);

        Assert.False(guild.LGuildVitaHeld);
        Assert.False(guild.LGuildModeEnabled);
        Assert.False(guild.LGuildBinEnabled);
        Assert.Contains("Orphan", guild.LGuildOeuvre.TOeuvreRowsRead().Select(row => row.LCatalogReferenceName));
    }

    [Fact]
    public void SourceSelect_Oeuvre_ShowsColophonAndDisablesMode()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        LColophon? shown = null;
        guild.LGuildOeuvre.LOeuvrePanel.LPanelDraftChanged +=
            draft => shown = guild.LGuildOeuvre.TOeuvreColophonRead(draft);
        guild.TGuildRowSelect(ada.LAuthorId);

        guild.TGuildSourceSelect(book.LReferenceId);

        Assert.True(guild.LGuildColophonShown);
        Assert.False(guild.LGuildVitaShown);
        Assert.False(guild.LGuildModeEnabled);
        Assert.True(guild.LGuildPressAllowed);
        Assert.Equal("Book", shown?.LColophonTitle);
    }

    private static LGuild TGuildPrepare(LEngine engine)
    {
        LGuild guild = TInterfaceDeportment.TGuildCreate(engine, () => true, _ => true, (_, _) => true);
        guild.TGuildVistaRestore(
            engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName),
            engine.TEngineVistaStart("oeuvre", LCatalogOrder.LCatalogOrderName));
        return guild;
    }
}
