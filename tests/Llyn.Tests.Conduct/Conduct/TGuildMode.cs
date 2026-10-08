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
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
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
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        guild.CGuildAuthorSelect(bob.LAuthorId);

        Assert.True(guild.CGuildDiptych.CDiptychParentEditing);
        Assert.True(guild.CGuildDiptych.CDiptychScribeChecked);
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
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        guild.CGuildAuthorSelect(0);

        Assert.False(guild.CGuildDiptych.CDiptychParentEditing);
        Assert.False(guild.CGuildDiptych.CDiptychScribeChecked);
        Assert.False(guild.CGuildAutograph.CDeskHeld);
    }

    [Fact]
    public void GuildAuthorSelect_UnsavedDraftStored_StoresItBeforeOpening()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildDiptych.CDiptychEntryCreate();
        guild.CGuildAutograph.TDeskDefer(TInterface.TAuthorNameCreate(guild.CGuildAutograph.CDeskId, "Bob"));

        guild.CGuildAuthorSelect(ada.LAuthorId);

        IReadOnlyList<CCatalogAuthor> rows = guild.CGuildRollRead().CGuildRollRows;
        Assert.Equal(["Leave"], asked);
        Assert.Contains("Bob", rows.Select(row => row.CCatalogAuthorName));
        Assert.Equal(["Ada"], rows.Where(row => row.CCatalogAuthorChosen).Select(row => row.CCatalogAuthorName));
        Assert.True(guild.CGuildDiptych.CDiptychParentEditing);
    }

    [Fact]
    public void GuildSourceSelect_NoRow_KeepsTheAuthorSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);

        guild.CGuildSourceSelect(null);

        Assert.Empty(asked);
        Assert.False(guild.CGuildDiptych.CDiptychChildSide);
        Assert.True(guild.CGuildDiptych.CDiptychParentShown);
    }

    [Fact]
    public void GuildScribeToggle_SourceSide_KeepsTheColophon()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildSourceSelect(book.LReferenceId);

        guild.CGuildScribeToggle(true);

        Assert.True(guild.CGuildDiptych.CDiptychChildSide);
        Assert.False(guild.CGuildDiptych.CDiptychParentEditing);
        Assert.False(guild.CGuildDiptych.CDiptychScribeChecked);
    }

    [Fact]
    public void GuildScribeToggle_BackToReading_DropsTheHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        Assert.True(guild.CGuildAutograph.CDeskHeld);

        guild.CGuildScribeToggle(false);

        Assert.False(guild.CGuildAutograph.CDeskHeld);
        Assert.True(guild.CGuildDiptych.CDiptychParentShown);
        Assert.True(guild.CGuildVitaHeld);
    }

    [Fact]
    public void GuildAuthorDelete_SourceSide_DeletesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildSourceSelect(book.LReferenceId);

        guild.CGuildDiptych.CDiptychEntryDelete();

        Assert.Empty(asked);
        Assert.NotNull(engine.TEngineAuthorRead(ada.LAuthorId));
    }
}
