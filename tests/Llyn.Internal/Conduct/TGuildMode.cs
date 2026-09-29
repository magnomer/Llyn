using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuildMode
{
    [Fact]
    public void GuildAuthorSelect_NoRow_ChangesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildAuthorSelect(null);

        Assert.Empty(asked);
        Assert.True(guild.CGuildBinEnabled);
        Assert.Equal("Ada", guild.CGuildRollRead().CGuildRollVita.CVitaName);
    }

    [Fact]
    public void GuildAuthorSelect_WritingThenAuthor_KeepsTheAutograph()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        guild.CGuildAuthorSelect(bob.LAuthorId);

        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildScribeChecked);
        Assert.True(guild.CGuildAutograph.CDeskHeld);
        IReadOnlyList<CCatalogAuthor> rows = guild.CGuildRollRead().CGuildRollRows;
        Assert.Equal(["Bob"], rows.Where(row => row.CCatalogAuthorChosen).Select(row => row.CCatalogAuthorName));
    }

    [Fact]
    public void GuildAuthorSelect_WritingThenOrphanRow_DropsTheAutograph()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        guild.CGuildAuthorSelect(0);

        Assert.False(guild.CGuildAutographShown);
        Assert.False(guild.CGuildScribeChecked);
        Assert.False(guild.CGuildAutograph.CDeskHeld);
    }

    [Fact]
    public void GuildAuthorSelect_UnsavedDraftStored_StoresItBeforeOpening()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorCreate();
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Bob"));

        guild.CGuildAuthorSelect(ada.LAuthorId);

        IReadOnlyList<CCatalogAuthor> rows = guild.CGuildRollRead().CGuildRollRows;
        Assert.Equal(["Leave"], asked);
        Assert.Contains("Bob", rows.Select(row => row.CCatalogAuthorName));
        Assert.Equal(["Ada"], rows.Where(row => row.CCatalogAuthorChosen).Select(row => row.CCatalogAuthorName));
        Assert.True(guild.CGuildAutographShown);
    }

    [Fact]
    public void GuildSourceSelect_NoRow_KeepsTheAuthorSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildSourceSelect(null);

        Assert.Empty(asked);
        Assert.False(guild.CGuildColophonShown);
        Assert.True(guild.CGuildVitaShown);
    }

    [Fact]
    public void GuildScribeToggle_SourceSide_KeepsTheColophon()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildSourceSelect(book.LReferenceId);

        guild.CGuildScribeToggle(true);

        Assert.True(guild.CGuildColophonShown);
        Assert.False(guild.CGuildAutographShown);
        Assert.False(guild.CGuildScribeChecked);
    }

    [Fact]
    public void GuildScribeToggle_BackToReading_DropsTheHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        Assert.True(guild.CGuildAutograph.CDeskHeld);

        guild.CGuildScribeToggle(false);

        Assert.False(guild.CGuildAutograph.CDeskHeld);
        Assert.True(guild.CGuildVitaShown);
        Assert.True(guild.CGuildVitaHeld);
    }

    [Fact]
    public void GuildAuthorDelete_SourceSide_DeletesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildSourceSelect(book.LReferenceId);

        guild.CGuildAuthorDelete();

        Assert.Empty(asked);
        Assert.NotNull(engine.TEngineAuthorRead(ada.LAuthorId));
    }
}
