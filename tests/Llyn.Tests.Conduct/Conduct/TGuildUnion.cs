using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuildUnion
{
    [Fact]
    public void GuildUnionRead_TypedName_ExcludesSelf()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
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
        CVita vita = guild.CGuildRollRead().CGuildRollVita;
        Assert.Equal("Adam", vita.CVitaName);
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.WorkOne"), vita.CVitaWork);
        Assert.False(guild.CGuildAutograph.CDeskHeld);
    }

    [Fact]
    public void GuildUnionSelect_PaddedNames_AsksWithBothTrimmed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
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
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        guild.CGuildScribeToggle(true);

        guild.CGuildUnionSelect(adam.LAuthorId);

        Assert.NotNull(engine.TEngineAuthorRead(ada.LAuthorId));
        Assert.True(guild.CGuildAutographShown);
        Assert.True(guild.CGuildAutograph.CDeskHeld);
    }
}
