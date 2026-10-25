using System.Collections.Generic;
using System.IO;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCollocationSense
{
    [Fact]
    public void CollocationSave_LinkedMeanings_StoresLinkOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TCollocationSenseSave(engine);

        LEntryDraft stored = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft root = stored.LEntryDraftMeanings[0];
        Assert.Equal(
            [stored.LEntryDraftMeanings[1].LCardDraftId, root.LCardDraftChild[0].LCardDraftId],
            Assert.Single(stored.LEntryDraftCollocations).LCardDraftSense);
    }

    [Fact]
    public void RequestContentApply_LinkedMeanings_SendsLinksInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TRequestContentApply(started.LDraftId, TCollocationSenseCreate());
        LEntry committed = engine.TEngineDraftCommit(started.LDraftId);

        IReadOnlyList<LCardDraft> meanings = answered.LDraftContent.LEntryDraftMeanings;
        Assert.Equal(
            [meanings[1].LCardDraftId, meanings[0].LCardDraftChild[0].LCardDraftId],
            Assert.Single(answered.LDraftContent.LEntryDraftCollocations).LCardDraftSense);
        LEntryDraft stored = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(committed.LEntryId));
        Assert.Equal(
            ["2", "1.1"],
            Assert.Single(stored.LEntryDraftCollocations).LCardDraftSense
                .Select(sense => TInterface.TMeaningPathRead(stored.LEntryDraftMeanings, sense)));
    }

    [Fact]
    public void MeaningSave_LinkedMeaningDropped_DropsLinkRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LEntryDraft stored = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft root = stored.LEntryDraftMeanings[0];

        engine.TEngineEntryUpdate(entry.LEntryId, stored with { LEntryDraftMeanings = [root] });

        LEntryDraft updated = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft collocation = Assert.Single(updated.LEntryDraftCollocations);
        Assert.Equal([root.LCardDraftChild[0].LCardDraftId], collocation.LCardDraftSense);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_sense;"));
        Assert.Equal(
            root.LCardDraftId,
            TInterface.TCollocationHomeRead(updated.LEntryDraftMeanings, collocation, false));
    }

    [Fact]
    public void CollocationSave_LinkEdit_RecordsCollocationUpdate()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LEntryDraft stored = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft collocation = Assert.Single(stored.LEntryDraftCollocations);

        engine.TEngineEntryUpdate(
            entry.LEntryId,
            stored with
            {
                LEntryDraftCollocations =
                [
                    collocation with { LCardDraftSense = [.. collocation.LCardDraftSense.Reverse()] },
                ],
            });

        long? revision = engine.TEngineRevisionRead();
        Assert.NotNull(revision);
        Assert.Contains(
            workspace.TRevisionChangeRead(revision.Value),
            change => change.LRevisionDeltaSubject == "collocation"
                && change.LRevisionDeltaKind == "update"
                && change.LRevisionDeltaTarget == collocation.LCardDraftId);
    }

    [Fact]
    public void CardSenseAttach_NestedMeaning_KeepsLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LDraft held = engine.TEngineDraftStart("Input", entry.LEntryId);
        LCardDraft branch = held.LDraftContent.LEntryDraftMeanings[0].LCardDraftChild[1];
        long other = held.LDraftContent.LEntryDraftMeanings[1].LCardDraftId;
        long collocation = held.LDraftContent.LEntryDraftCollocations[0].LCardDraftId;

        LDraft answered = engine.TCardSenseAttach(held.LDraftId, collocation, branch.LCardDraftId);
        LEntry committed = engine.TEngineDraftCommit(held.LDraftId);

        long first = held.LDraftContent.LEntryDraftMeanings[0].LCardDraftChild[0].LCardDraftId;
        Assert.Equal(
            [other, first, branch.LCardDraftId],
            answered.LDraftContent.LEntryDraftCollocations[0].LCardDraftSense);
        LEntryDraft stored = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(committed.LEntryId));
        Assert.Equal([other, first, branch.LCardDraftId], stored.LEntryDraftCollocations[0].LCardDraftSense);
    }

    [Fact]
    public void CardSenseDetach_TwoLinks_KeepsOtherLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LDraft held = engine.TEngineDraftStart("Input", entry.LEntryId);
        long other = held.LDraftContent.LEntryDraftMeanings[1].LCardDraftId;
        long first = held.LDraftContent.LEntryDraftMeanings[0].LCardDraftChild[0].LCardDraftId;
        long collocation = held.LDraftContent.LEntryDraftCollocations[0].LCardDraftId;

        LDraft answered = engine.TCardSenseDetach(held.LDraftId, collocation, other);

        Assert.Equal([first], answered.LDraftContent.LEntryDraftCollocations[0].LCardDraftSense);
    }

    [Fact]
    public void CardRemove_LinkedParentMeaning_SweepsDescendantLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LDraft held = engine.TEngineDraftStart("Input", entry.LEntryId);
        long root = held.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;
        long other = held.LDraftContent.LEntryDraftMeanings[1].LCardDraftId;

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestRemovalCreate(held.LDraftId, root));

        Assert.Equal([other], answered.LDraftContent.LEntryDraftCollocations[0].LCardDraftSense);
    }

    [Fact]
    public void CardMove_LinkedMeaning_KeepsLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LDraft held = engine.TEngineDraftStart("Input", entry.LEntryId);
        long first = held.LDraftContent.LEntryDraftMeanings[0].LCardDraftChild[0].LCardDraftId;
        long other = held.LDraftContent.LEntryDraftMeanings[1].LCardDraftId;

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestShiftCreate(held.LDraftId, first, 0, 0));

        Assert.Equal(first, answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftId);
        Assert.Equal([other, first], answered.LDraftContent.LEntryDraftCollocations[0].LCardDraftSense);
    }

    [Fact]
    public void CardApply_ExpressionOnMeaning_RefusesRequest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LDraft held = engine.TEngineDraftStart("Input", entry.LEntryId);
        long first = held.LDraftContent.LEntryDraftMeanings[0].LCardDraftChild[0].LCardDraftId;

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestExpressionCreate(
                held.LDraftId, first, TInterfaceState.TStateValueCreate("set down"))));

        Assert.Equal(LRefusal.LRefusalCard, refusal.LRefusalReason);
    }

    [Fact]
    public void CardApply_EmptyExpressionOnMeaning_LeavesTheMeaningUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LDraft held = engine.TEngineDraftStart("Input", entry.LEntryId);
        LCardDraft first = held.LDraftContent.LEntryDraftMeanings[0].LCardDraftChild[0];

        engine.TEngineRequestApply(TInterface.TRequestExpressionCreate(
            held.LDraftId, first.LCardDraftId, LStateValue.LStateValueUnspecified));
        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestExpressionCreate(
            held.LDraftId, first.LCardDraftId, TInterfaceState.TStateValueCreate(" ")));

        LCardDraft kept = answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftChild[0];
        Assert.Equal(first.LCardDraftId, kept.LCardDraftId);
        Assert.Equal(first.LCardDraftExpression, kept.LCardDraftExpression);
        Assert.Equal(first.LCardDraftMeaning, kept.LCardDraftMeaning);
    }

    [Fact]
    public void EntryExport_LinkedCollocation_RoundTripsSensePaths()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport([entry.LEntryId], path);
        engine.TEngineEntryDelete(entry.LEntryId);

        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Empty(outcome.TMarkupOutcomeOmission);
        LEntry imported = Assert.Single(outcome.TMarkupOutcomeEntry);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(imported.LEntryId));
        LCardDraft collocation = Assert.Single(draft.LEntryDraftCollocations);
        IReadOnlyList<LCardDraft> meanings = draft.LEntryDraftMeanings;
        Assert.Equal(
            ["2", "1.1"],
            collocation.LCardDraftSense.Select(sense => TInterface.TMeaningPathRead(meanings, sense)));
    }

    [Fact]
    public void LiveryCardAppend_NestedHome_RendersUnderNestedMeaning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LEntryDraft draft = page.LLiveryPageDraft;
        LCardDraft collocation = draft.LEntryDraftCollocations[0];
        page = page with
        {
            LLiveryPageDraft = draft with
            {
                LEntryDraftCollocations =
                [
                    collocation with
                    {
                        LCardDraftSense =
                        [
                            draft.LEntryDraftMeanings[0].LCardDraftChild[0].LCardDraftId,
                            draft.LEntryDraftMeanings[1].LCardDraftId,
                        ],
                    },
                ],
            },
        };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        int nested = body.IndexOf("<span class=\"llyn-number\">1.1</span>", StringComparison.Ordinal);
        int home = body.IndexOf(
            "<details class=\"llyn-collocation\" open>\n<summary><span class=\"llyn-phrase\">set off</span></summary>",
            StringComparison.Ordinal);
        int sibling = body.IndexOf("<span class=\"llyn-number\">1.2</span>", StringComparison.Ordinal);
        int further = body.IndexOf("<span class=\"llyn-number\">2</span>", StringComparison.Ordinal);
        int stub = body.IndexOf("<span class=\"llyn-stub\">Display.CollocationStub</span>", StringComparison.Ordinal);
        Assert.True(nested >= 0 && nested < home && home < sibling);
        Assert.True(further >= 0 && further < stub);
        Assert.DoesNotContain("## Display.CollocationOther", body, StringComparison.Ordinal);
        Assert.Contains(
            "<div class=\"llyn-collocations\">\n\n<span class=\"llyn-heading\">Display.CollocationHeading</span>",
            body,
            StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryCardAppend_FoldedCollocation_WritesClosedRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LEntryDraft held = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        engine.TEngineFoldSave(
            entry.LEntryId, LCardKind.LCardKindCollocation, held.LEntryDraftCollocations[0].LCardDraftId);
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains(
            "<details class=\"llyn-collocation\">\n<summary><span class=\"llyn-phrase\">set off</span></summary>",
            body,
            StringComparison.Ordinal);
        Assert.DoesNotContain("<details class=\"llyn-collocation\" open>", body, StringComparison.Ordinal);
        Assert.DoesNotContain("<span class=\"llyn-title\">set off</span>", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryCardAppend_UnlinkedCollocation_WritesOtherRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TCollocationSenseSave(engine);
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LEntryDraft draft = page.LLiveryPageDraft;
        page = page with
        {
            LLiveryPageDraft = draft with
            {
                LEntryDraftCollocations = [draft.LEntryDraftCollocations[0] with { LCardDraftSense = [] }],
            },
        };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains(
            "## Display.CollocationOther\n\n<div class=\"llyn-collocations\">\n\n"
            + "<details class=\"llyn-collocation\" open>\n<summary><span class=\"llyn-phrase\">set off</span>",
            body,
            StringComparison.Ordinal);
        Assert.DoesNotContain("<span class=\"llyn-title\">set off</span>", body, StringComparison.Ordinal);
        Assert.DoesNotContain("Display.CollocationHeading", body, StringComparison.Ordinal);
    }

    private static LEntry TCollocationSenseSave(LEngine engine) =>
        engine.TEngineEntrySave(TCollocationSenseCreate());

    private static LEntryDraft TCollocationSenseCreate()
    {
        LCardDraft first = TInterface.TCardCreate("to place firmly", 1) with
        {
            LCardDraftId = TInterface.TIdentityCreate(),
        };
        LCardDraft second = TInterface.TCardCreate("to place loosely", 2) with
        {
            LCardDraftId = TInterface.TIdentityCreate(),
        };
        LCardDraft root = TInterface.TCardCreate("to place", 1) with
        {
            LCardDraftId = TInterface.TIdentityCreate(),
            LCardDraftChild = [first, second],
        };
        LCardDraft other = TInterface.TCardCreate("to harden", 2) with
        {
            LCardDraftId = TInterface.TIdentityCreate(),
        };
        LCardDraft collocation = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("set off"),
            LStateValue.LStateValueUnspecified,
            [], [], [], [], [], 1) with
        {
            LCardDraftSense = [other.LCardDraftId, first.LCardDraftId],
        };
        return TInterface.TEntryDraftCreate(
            "set", "English", string.Empty, string.Empty, [root, other], [collocation]);
    }
}
