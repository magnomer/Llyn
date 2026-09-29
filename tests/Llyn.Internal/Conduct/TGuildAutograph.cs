using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuildAutograph
{
    [Fact]
    public void GuildNameSet_FreshAuthor_StoresTheTypedName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        guild.CGuildAuthorCreate();

        guild.CGuildNameSet("Ada");

        Assert.True(guild.CGuildStoreEnabled);
        Assert.True(guild.CGuildSession.CSessionSave());
        Assert.Equal(
            ["Ada"],
            guild.CGuildRollRead().Where(row => row.CCatalogAuthorChosen).Select(row => row.CCatalogAuthorName));
    }

    [Fact]
    public void GuildNameSet_NoAuthorHeld_ChangesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        guild.CGuildNameSet("Ada");

        Assert.False(guild.CGuildAutograph.CDeskHeld);
        Assert.True(guild.CGuildEmpty);
    }

    [Fact]
    public void GuildUnionCleared_AuthorStarted_TellsTheDriverOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        int cleared = 0;
        guild.CGuildUnionCleared += () => cleared++;

        guild.CGuildAuthorSelect(ada.LAuthorId);

        Assert.Equal(0, cleared);

        guild.CGuildScribeToggle(true);

        Assert.Equal(1, cleared);
    }

    [Fact]
    public void GuildCreate_DraftNotice_ShowsTheDraftThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CGuild guild = CGuild.CGuildCreate(
            atelier,
            static () => true,
            TInterfaceConduct.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        guild.CGuildVistaRestore();
        guild.CGuildAuthorCreate();
        guild.CGuildNameSet("Ada");
        guild.CGuildAutograph.CDeskPersist();
        List<string> shown = [];
        guild.CGuildAutograph.CDeskDraftChanged += draft => shown.Add(draft.CDraftAuthorName);
        marshalled = 0;

        engine.TEngineBulletinRaise(LSubject.LSubjectDraft, guild.CGuildAutograph.CDeskId);

        Assert.Equal(1, marshalled);
        Assert.Equal(["Ada"], shown);
    }
}
