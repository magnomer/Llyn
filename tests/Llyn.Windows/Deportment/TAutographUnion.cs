using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TAutographUnion
{
    [Fact]
    public void UnionRead_TypedName_ExcludesSelf()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        guild.TGuildRowSelect(ada.LAuthorId);
        guild.TGuildScribeSet(true);

        Assert.True(guild.LGuildAutographShown);
        Assert.True(guild.LGuildUnionShown);
        Assert.Equal(["Adam"], guild.TGuildUnionRead("Ad").Select(row => row.LCatalogAuthorName));
        Assert.Empty(guild.TGuildUnionRead(" "));
    }

    [Fact]
    public void UnionSelect_Confirmed_AbsorbsAndShowsKept()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        guild.TGuildRowSelect(ada.LAuthorId);
        guild.TGuildScribeSet(true);

        guild.TGuildUnionSelect(adam.LAuthorId);

        Assert.Null(engine.TEngineAuthorRead(ada.LAuthorId));
        Assert.False(guild.LGuildAutographShown);
        Assert.True(guild.LGuildVitaShown);
        Assert.Equal("Adam", guild.TGuildVitaRead().LVitaName);
        Assert.Equal("Guild.WorkOne", guild.TGuildVitaRead().LVitaWork);
        Assert.False(guild.LGuildAutograph.LDeskHeld);
    }

    [Fact]
    public void Store_FreshNamedAuthor_SelectsItAndStaysEditing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LGuild guild = TGuildPrepare(engine);
        string? refused = null;
        guild.LGuildRefused += key => refused = key;

        guild.TGuildFreshStart();

        Assert.True(guild.LGuildAutographShown);
        Assert.False(guild.LGuildUnionShown);
        Assert.False(guild.TGuildSave());
        Assert.Equal("Guild.NameBlank", refused);

        guild.LGuildAutograph.TDeskDefer(TInterface.TRequestAuthorCreate(guild.LGuildAutograph.LDeskId, "Ada"));

        Assert.True(guild.TGuildSave());
        Assert.True(guild.LGuildAutographShown);
        Assert.True(guild.LGuildUnionShown);
        Assert.True(guild.LGuildAutograph.LDeskHeld);
        Assert.Equal(
            ["Ada"],
            guild.TGuildRollRead().Where(row => row.LCatalogAuthorChosen).Select(row => row.LCatalogAuthorName));
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
