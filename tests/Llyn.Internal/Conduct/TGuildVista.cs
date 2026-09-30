using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuildVista
{
    [Fact]
    public void GuildVistaRestore_QueryHeld_CarriesItIntoTheFreshVista()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        guild.CGuildQuerySet("Bo");

        guild.TGuildVistaRestore();

        Assert.Equal(["Bob"], guild.CGuildRollRead().CGuildRollRows.Select(row => row.CCatalogAuthorName));
    }

    [Fact]
    public void GuildVistaRestore_OeuvreQueryHeld_CarriesItIntoTheFreshVista()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineCitationCreate("Grammar");
        engine.TEngineCitationCreate("Atlas");
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        guild.CGuildOeuvre.COeuvreQuerySet("gram");

        guild.TGuildVistaRestore();

        Assert.Equal(["Grammar"], guild.CGuildOeuvre.COeuvreRowsRead().Select(row => row.CCatalogReferenceName));
    }

    [Fact]
    public void GuildVistaRestore_AuthorNoticeAfterASecondRestore_RaisesTheRollOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CGuild guild = CGuild.CGuildCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        guild.TGuildVistaRestore();
        int rows = 0;
        guild.CGuildPanel.CPanelRowsChanged += () => rows++;
        marshalled = 0;

        engine.TEngineBulletinRaise(LSubject.LSubjectAuthor, 0);

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void GuildCreate_AuthorNoticeWithoutARestore_RaisesTheRollOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CGuild guild = CGuild.CGuildCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int rows = 0;
        guild.CGuildPanel.CPanelRowsChanged += () => rows++;
        marshalled = 0;

        engine.TEngineBulletinRaise(LSubject.LSubjectAuthor, 0);

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void GuildVistaRestore_OeuvreVistaNotice_RaisesTheOeuvreRowsThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CGuild guild = CGuild.CGuildCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int rows = 0;
        guild.CGuildOeuvre.COeuvrePanel.CPanelRowsChanged += () => rows++;
        marshalled = 0;

        guild.CGuildOeuvre.COeuvreQuerySet("gram");

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void GuildOrderRead_Menu_OffersTheFourOrderings()
    {
        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderWork,
                CCatalogOrder.CCatalogOrderUsage,
            ],
            CGuild.CGuildOrderRead());
    }
}
