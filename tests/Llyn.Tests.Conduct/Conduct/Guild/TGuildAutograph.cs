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
        engine.TEngineDelaySet(0);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        guild.CGuildDiptych.CDiptychEntryCreate();

        guild.CGuildNameSet("Ada");

        Assert.True(guild.CGuildStoreEnabled);
        Assert.True(guild.CGuildSession.CSessionSave());
        Assert.Equal(
            ["Ada"],
            guild.CGuildRollRead().CGuildRollRows
                .Where(row => row.CCatalogAuthorChosen)
                .Select(row => row.CCatalogAuthorName));
    }

    [Fact]
    public void GuildNameSet_NoAuthorHeld_ChangesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        guild.CGuildNameSet("Ada");

        Assert.False(guild.CGuildAutograph.CDeskHeld);
        Assert.All(guild.CGuildRollRead().CGuildRollRows, row => Assert.Equal(0L, row.CCatalogAuthorId));
    }

    [Fact]
    public void GuildUnionCleared_AuthorStarted_TellsTheDriverOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        int cleared = 0;
        guild.CGuildUnion.CGuildUnionCleared += () => cleared++;

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
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        guild.CGuildDiptych.CDiptychEntryCreate();
        guild.CGuildNameSet("Ada");
        guild.CGuildAutograph.CDeskDraft.CDeskDraftPersist();
        List<string> shown = [];
        guild.CGuildAutograph.CDeskDraft.CDeskDraftChanged += draft => shown.Add(draft.CDraftAuthorName);
        marshalled = 0;

        engine.TEngineBulletinRaise(LSubject.LSubjectDraft, guild.CGuildAutograph.CDeskId);

        Assert.Equal(1, marshalled);
        Assert.Equal(["Ada"], shown);
    }
}
