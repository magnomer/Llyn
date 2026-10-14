using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAncestor
{
    [Fact]
    public void AncestorInsertion_AboveTheHeadword_TakesOverTheRoots()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long first = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;

        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));

        IReadOnlyList<LAncestorDraft> map = TAncestorHeldRead(engine, held.LDraftId);
        Assert.Equal(2, map.Count);
        Assert.Null(map[0].LAncestorDraftChild);
        Assert.Equal(map[0].LAncestorDraftId, map.Single(box => box.LAncestorDraftId == first).LAncestorDraftChild);
    }

    [Fact]
    public void AncestorInsertion_AboveABox_TakesOverItsSources()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        (long root, long first, long second) = TAncestorTreeCreate(engine, held.LDraftId);

        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, root));

        IReadOnlyList<LAncestorDraft> map = TAncestorHeldRead(engine, held.LDraftId);
        LAncestorDraft inserted = Assert.Single(map, box => box.LAncestorDraftChild == root);
        Assert.Equal(
            [first, second],
            map.Where(box => box.LAncestorDraftChild == inserted.LAncestorDraftId).Select(box => box.LAncestorDraftId));
    }

    [Fact]
    public void AncestorAddition_BesideABox_SharesItsChild()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        (long root, long first, long second) = TAncestorTreeCreate(engine, held.LDraftId);

        engine.TEngineRequestApply(TInterface.TAncestorAdditionCreate(held.LDraftId, first));

        IReadOnlyList<long> sources = TAncestorHeldRead(engine, held.LDraftId)
            .Where(box => box.LAncestorDraftChild == root)
            .Select(box => box.LAncestorDraftId)
            .ToList();
        Assert.Equal(3, sources.Count);
        Assert.Equal((first, second), (sources[0], sources[2]));
    }

    [Fact]
    public void AncestorRemoval_BoxWithSources_HandsThemItsPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        (long root, long first, long second) = TAncestorTreeCreate(engine, held.LDraftId);

        engine.TEngineRequestApply(TInterface.TAncestorRemovalCreate(held.LDraftId, root));

        IReadOnlyList<LAncestorDraft> map = TAncestorHeldRead(engine, held.LDraftId);
        Assert.Equal([first, second], map.Select(static box => box.LAncestorDraftId));
        Assert.All(map, static box => Assert.Null(box.LAncestorDraftChild));
    }

    [Fact]
    public void AncestorTarget_BoxWithSources_SetsThemAside()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rennen");
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        (long root, _, _) = TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineRequestApply(TInterface.TAncestorLanguageCreate(held.LDraftId, root, "Middle English", 0));

        engine.TEngineRequestApply(TInterface.TAncestorTargetCreate(held.LDraftId, root, source));

        LAncestorDraft linked = Assert.Single(TAncestorHeldRead(engine, held.LDraftId));
        Assert.Equal((source, "rennen", string.Empty), (
            linked.LAncestorDraftTarget, linked.LAncestorDraftText, linked.LAncestorDraftLanguage));
    }

    [Fact]
    public void AncestorRestoration_AfterPick_RestoresTheSources()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rennen");
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        (long root, long first, long second) = TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(held.LDraftId, root, "renne"));
        engine.TEngineRequestApply(TInterface.TAncestorLanguageCreate(held.LDraftId, root, "Middle English", 0));
        engine.TEngineRequestApply(TInterface.TAncestorTargetCreate(held.LDraftId, root, source));

        engine.TEngineRequestApply(TInterface.TAncestorRestorationCreate(held.LDraftId, root));

        IReadOnlyList<LAncestorDraft> map = TAncestorHeldRead(engine, held.LDraftId);
        Assert.Equal([root, first, second], map.Select(static box => box.LAncestorDraftId));
        Assert.Equal(((long?)null, "renne", "Middle English"), (
            map[0].LAncestorDraftTarget, map[0].LAncestorDraftText, map[0].LAncestorDraftLanguage));
        Assert.All(map.Skip(1), box => Assert.Equal(root, box.LAncestorDraftChild));
    }

    [Fact]
    public void AncestorRestoration_SecondUndo_RefusesTheBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rennen");
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        (long root, _, _) = TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineRequestApply(TInterface.TAncestorTargetCreate(held.LDraftId, root, source));
        engine.TEngineRequestApply(TInterface.TAncestorRestorationCreate(held.LDraftId, root));

        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineRequestApply(TInterface.TAncestorRestorationCreate(held.LDraftId, root)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
    }

    [Fact]
    public void AncestorRestoration_TextAfterPick_RefusesTheBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rennen");
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        (long root, _, _) = TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineRequestApply(TInterface.TAncestorTargetCreate(held.LDraftId, root, source));
        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(held.LDraftId, root, "renne"));

        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineRequestApply(TInterface.TAncestorRestorationCreate(held.LDraftId, root)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
        Assert.Single(TAncestorHeldRead(engine, held.LDraftId));
    }

    [Fact]
    public void AncestorText_LinkedBox_ClearsTheLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rennen");
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;
        engine.TEngineRequestApply(TInterface.TAncestorTargetCreate(held.LDraftId, box, source));

        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(held.LDraftId, box, "renne"));

        LAncestorDraft written = Assert.Single(TAncestorHeldRead(engine, held.LDraftId));
        Assert.Equal(((long?)null, "renne"), (written.LAncestorDraftTarget, written.LAncestorDraftText));
    }

    [Fact]
    public void AncestorInsertion_AboveALinkedBox_RefusesTheSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rennen");
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;
        engine.TEngineRequestApply(TInterface.TAncestorTargetCreate(held.LDraftId, box, source));

        LRefusal inserted = Assert.Throws<LRefusal>(
            () => engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, box)));
        LRefusal spoken = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TAncestorLanguageCreate(held.LDraftId, box, "Latin", 0)));

        Assert.Equal(LRefusal.LRefusalLink, inserted.LRefusalReason);
        Assert.Equal(LRefusal.LRefusalLink, spoken.LRefusalReason);
        Assert.Single(TAncestorHeldRead(engine, held.LDraftId));
    }

    [Fact]
    public void AncestorTarget_UnknownEntry_RefusesTheLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TAncestorEntryCreate(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TAncestorTargetCreate(held.LDraftId, box, entryId + 900)));

        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);
    }

    [Fact]
    public void AncestorLanguage_Tongue_ClearsTheAppLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        long tongue = archive.TAncestorTongueCreate("Proto-Germanic").LAncestorTongueId;
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;
        engine.TEngineRequestApply(TInterface.TAncestorLanguageCreate(held.LDraftId, box, "Latin", 0));

        engine.TEngineRequestApply(TInterface.TAncestorLanguageCreate(held.LDraftId, box, "Latin", tongue));

        LAncestorDraft written = Assert.Single(TAncestorHeldRead(engine, held.LDraftId));
        Assert.Equal((string.Empty, (long?)tongue), (written.LAncestorDraftLanguage, written.LAncestorDraftTongue));
    }

    [Fact]
    public void AncestorLanguage_UnknownTongue_RefusesTheItem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TAncestorLanguageCreate(held.LDraftId, box, string.Empty, 900)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
    }

    [Fact]
    public void AncestorRelation_BuiltinThenNone_ClearsTheRelation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        long relation = archive.TAncestorRelationRead()[0].LAncestorRelationId;
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;

        engine.TEngineRequestApply(TInterface.TAncestorRelationCreate(held.LDraftId, box, relation));
        long? named = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftRelation;
        engine.TEngineRequestApply(TInterface.TAncestorRelationCreate(held.LDraftId, box, 0));

        Assert.Equal(relation, named);
        Assert.Null(TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftRelation);
    }

    [Fact]
    public void AncestorRelation_UnknownRelation_RefusesTheItem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TAncestorRelationCreate(held.LDraftId, box, 900)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
    }

    [Fact]
    public void AncestorGloss_TextBox_StoresTheGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        long box = TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId;

        engine.TEngineRequestApply(TInterface.TAncestorGlossCreate(held.LDraftId, box, "to run"));

        Assert.Equal("to run", TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftGloss);
    }

    [Fact]
    public void AncestorRemoval_UnknownBox_RefusesTheItem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft held = engine.TEngineDraftStart("Input", TAncestorEntryCreate(engine, "run"));

        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineRequestApply(TInterface.TAncestorRemovalCreate(held.LDraftId, 900)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
    }

    internal static (long TAncestorRoot, long TAncestorFirst, long TAncestorSecond) TAncestorTreeCreate(
        LEngine engine, long draftId)
    {
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(draftId, 0));
        long root = TAncestorHeldRead(engine, draftId)[0].LAncestorDraftId;
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(draftId, root));
        long first = TAncestorHeldRead(engine, draftId).Single(box => box.LAncestorDraftChild == root).LAncestorDraftId;
        engine.TEngineRequestApply(TInterface.TAncestorAdditionCreate(draftId, first));
        long second = TAncestorHeldRead(engine, draftId)
            .Single(box => box.LAncestorDraftChild == root && box.LAncestorDraftId != first)
            .LAncestorDraftId;
        return (root, first, second);
    }

    internal static IReadOnlyList<LAncestorDraft> TAncestorHeldRead(LEngine engine, long draftId)
    {
        return Assert.IsType<LDraft>(engine.TEngineDraftRead(draftId)).LDraftContent.LEntryDraftAncestors;
    }

    internal static long TAncestorEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [], [])).LEntryId;
    }
}
