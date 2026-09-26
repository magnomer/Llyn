using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorSeed
{
    [Fact]
    public void TagAdd_FreshDraft_LinksFirstCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("botany");
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "membership");
        editor.TEditorOpen(null);

        editor.TEditorTagAdd(tag.LTagId);

        Assert.Contains(
            editor.TEditorDraftRead()?.LEntryDraftMeanings[0].LCardDraftTag ?? [],
            row => row.LTagDraftId == tag.LTagId);
        Assert.True(editor.LEditorChanged);
    }

    [Fact]
    public void RegisterAdd_FreshDraft_LinksFirstCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister register = engine.TEngineRegisterCreate("formal");
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "cohort");
        editor.TEditorOpen(null);

        editor.TEditorRegisterAdd(register.LRegisterId);

        Assert.Contains(
            editor.TEditorDraftRead()?.LEntryDraftMeanings[0].LCardDraftRegister ?? [],
            row => row.LRegisterDraftId == register.LRegisterId);
    }

    [Fact]
    public void SituationAdd_FreshDraft_LinksFirstCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterface.TStateValueCreate("at the market");
        LSituation situation = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "occurrence");
        editor.TEditorOpen(null);

        editor.TEditorSituationAdd(situation.LSituationId);

        Assert.Contains(
            editor.TEditorDraftRead()?.LEntryDraftMeanings[0].LCardDraftSituation ?? [],
            row => row.LSituationDraftId == situation.LSituationId);
    }

    [Fact]
    public void ExampleAdd_FreshDraft_FillsFirstSentence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", TInterface.TStateValueCreate("Water is wet."), null, LStateAnchor.LStateAnchorUnspecified));
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "quotation");
        editor.TEditorOpen(null);

        editor.TEditorExampleAdd(example.LExampleId);

        LExampleDraft? cited =
            editor.TEditorDraftRead()?.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample;
        Assert.Equal(example.LExampleId, cited?.LExampleDraftId);
    }

    [Fact]
    public void ReferenceAdd_FreshDraft_CitesFirstSentence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "footnote");
        editor.TEditorOpen(null);

        editor.TEditorReferenceAdd(book.LReferenceId);

        LExampleDraft? cited =
            editor.TEditorDraftRead()?.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample;
        Assert.True(cited?.LExampleDraftReference.TStateAnchorMatch(book.LReferenceId));
    }
}
