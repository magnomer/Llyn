using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGuildRoll
{
    [Fact]
    public void GuildRollRead_ChosenAuthor_CarriesItsVitaWithTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LAuthor bob = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Bob"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        engine.TRequestCreditApply(book.LReferenceId, bob.LAuthorId, 1);
        guild.CGuildAuthorSelect(ada.LAuthorId);

        CGuildRoll roll = guild.CGuildRollRead();

        Assert.False(roll.CGuildRollEmpty);
        Assert.Equal(
            ["Ada"],
            roll.CGuildRollRows.Where(row => row.CCatalogAuthorChosen).Select(row => row.CCatalogAuthorName));
        Assert.Equal("Ada", roll.CGuildRollVita.CVitaName);
        Assert.Equal(["1"], roll.CGuildRollVita.CVitaFellows.Select(fellow => fellow.CFellowCount));
    }

    [Fact]
    public void GuildRollRead_UnmatchedQuery_AnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildQuerySet("Zed");

        CGuildRoll roll = guild.CGuildRollRead();

        Assert.True(roll.CGuildRollEmpty);
        Assert.Empty(roll.CGuildRollRows);
    }

    [Fact]
    public void GuildRollRead_ChosenAuthorGone_ReadsTheVitaAfterTheClose()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        guild.CGuildAuthorSelect(ada.LAuthorId);
        engine.TEngineAuthorDelete(ada.LAuthorId, false);

        CGuildRoll roll = guild.CGuildRollRead();

        Assert.False(roll.CGuildRollVita.CVitaNamed);
        Assert.Equal(TInterface.TLocalizationTextRead("Guild.Unnamed"), roll.CGuildRollVita.CVitaName);
    }

    [Fact]
    public void GuildRollRead_StoredAndUncreditedRows_CarryTheirIconsAndWordedCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CGuild guild = TGuild.TGuildPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        engine.TEngineCitationCreate("Orphan");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(
                0, "English", "one word", null, TInterfaceState.TStateAnchorRead(book.LReferenceId)));
        engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(
                0, "English", "two words", null, TInterfaceState.TStateAnchorRead(book.LReferenceId)));

        CGuildRoll roll = guild.CGuildRollRead();

        Assert.Equal(["unlink", "guild"], roll.CGuildRollRows.Select(row => row.CCatalogAuthorIcon));
        Assert.Equal([string.Empty, "2"], roll.CGuildRollRows.Select(row => row.CCatalogAuthorCount));
    }
}
