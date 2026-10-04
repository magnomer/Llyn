using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuildScribe
{
    [Fact]
    public void GuildAuthorSelect_UnsavedDraftKept_StaysOnTheAutograph()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorCreate();
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Bob"));

        guild.CGuildAuthorSelect(ada.LAuthorId);

        Assert.Equal(["Leave"], asked);
        Assert.True(guild.CGuildAutographShown);
        Assert.False(guild.CGuildVitaHeld);
    }

    [Fact]
    public void GuildAuthorSelect_UnsavedDraftKept_RecordsNoStation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
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
    public void GuildAuthorOpen_UnsavedDraft_OpensWithoutAsking()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorCreate();
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Bob"));

        guild.TGuildAuthorOpen(ada.LAuthorId);

        Assert.Empty(asked);
        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildBinEnabled);
        Assert.Equal("Ada", guild.CGuildRollRead().CGuildRollVita.CVitaName);
    }

    [Fact]
    public void GuildScribeToggle_NoAuthorChosen_ClosesThePanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        engine.TEngineDelaySet(0);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

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
            guild.CGuildRollRead().CGuildRollRows
                .Where(row => row.CCatalogAuthorChosen)
                .Select(row => row.CCatalogAuthorName));
    }
}
