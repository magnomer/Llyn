using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupTarget
{
    [Fact]
    public void MarkupFind_StoredEntry_CountsMeaningAndCollocationCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "tally", "English", string.Empty, string.Empty,
            [TInterface.TCardCreate("one", 1), TInterface.TCardCreate("two", 2)],
            [TInterface.TCardCreate("three", 1)]));

        LMarkupTarget target = Assert.Single(
            engine.TEngineMarkupFind(TInterface.TMarkupEntryCreate("tally", "English")));

        Assert.Equal(
            (entry.LEntryId, 2, 1),
            (target.LMarkupTargetId, target.LMarkupTargetMeaning, target.LMarkupTargetCollocation));
    }

    [Fact]
    public void MarkupFind_NoStoredMatch_FindsNoTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Empty(engine.TEngineMarkupFind(TInterface.TMarkupEntryCreate("missing", "English")));
    }

    [Fact]
    public void MarkupFind_Matches_FindsTheEntryIds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "found", "English", string.Empty, string.Empty, [TInterface.TCardCreate("one", 1)], []));

        Assert.Equal(
            [entry.LEntryId],
            engine.TEngineMarkupFind(TInterface.TMarkupEntryCreate("found", "English"))
                .Select(static target => target.LMarkupTargetId));
    }

    [Fact]
    public void MarkupTargetCreate_NestedChildren_CountsEveryCard()
    {
        LCardDraft leaf = TInterface.TDraftCardCreate("leaf");
        LCardDraft parent = TInterface.TDraftCardCreate("parent") with { LCardDraftChild = [leaf, leaf] };

        LMarkupTarget target = TInterface.TMarkupTargetCreate(
            7, TInterface.TEntryDraftCreate("nest", "English", string.Empty, string.Empty, [parent, leaf], [leaf]));

        Assert.Equal(
            (7L, 4, 1), (target.LMarkupTargetId, target.LMarkupTargetMeaning, target.LMarkupTargetCollocation));
    }
}
