using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftExample
{
    [Fact]
    public void RequestApply_HeldSentence_SurvivesScan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long held;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft started = engine.TEngineExampleStart("Corpus", null);
            held = started.LDraftId;

            TDraftSentenceApply(engine, started, "she knelt to kindle the damp logs", "English");

            Assert.True(engine.TEngineDraftCheck(held));
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LDraft> drafts = launched.TEngineDraftScan();
        LDraft? found = null;
        foreach (LDraft draft in drafts)
        {
            if (draft.LDraftId == held)
            {
                found = draft;
            }
        }

        Assert.NotNull(found);
        Assert.NotNull(found.LDraftExample);
        Assert.Equal(
            "she knelt to kindle the damp logs",
            found.LDraftExample.LExampleText.TStateValueShow());
        Assert.Contains(launched.TEngineLeftoverRead(), draft => draft.LDraftId == held);
    }

    [Fact]
    public void ExampleCommit_HeldSentence_LeavesNothingBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineExampleStart("Corpus", null);

        TDraftSentenceApply(engine, started, "she knelt to kindle the damp logs", "English");

        LExample stored = engine.TEngineExampleCommit(started.LDraftId);

        Assert.Equal(
            "she knelt to kindle the damp logs", stored.LExampleText.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(started.LDraftId));
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Empty(engine.TEngineLeftoverRead());

        LExample? written = engine.TEngineExampleRead(stored.LExampleId);

        Assert.NotNull(written);
        Assert.Equal("she knelt to kindle the damp logs", written.LExampleText.TStateValueShow());
    }

    [Fact]
    public void LeftoverSweep_SentenceMatchingStoredExample_SweepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long swept;
        long kept;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft committed = engine.TEngineExampleStart("Corpus", null);
            LDraft edited = engine.TEngineExampleStart("Corpus", null);

            swept = committed.LDraftId;
            kept = edited.LDraftId;

            LDraft written = TDraftSentenceApply(engine, committed, "she knelt to kindle the damp logs", "English");

            LExample stored = engine.TEngineExampleCreate(written.LDraftExample!);

            TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, written with
            {
                LDraftEntryId = stored.LExampleId,
                LDraftExample = stored,
            });
            TDraftSentenceApply(engine, edited, "the ember still glowed", "English");
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        launched.TEngineLeftoverSweep();

        Assert.Null(launched.TEngineDraftRead(swept));
        Assert.NotNull(launched.TEngineDraftRead(kept));
        Assert.Contains(launched.TEngineLeftoverRead(), draft => draft.LDraftId == kept);
    }

    [Fact]
    public void ExampleCommit_SentenceWithNoLanguage_StoresIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineExampleStart("Corpus", null);

        TDraftSentenceApply(engine, started, "she knelt to kindle the damp logs", string.Empty);

        LExample stored = engine.TEngineExampleCommit(started.LDraftId);

        Assert.Equal(string.Empty, stored.LExampleLanguage);

        LExample? written = engine.TEngineExampleRead(stored.LExampleId);

        Assert.NotNull(written);
        Assert.Equal(string.Empty, written.LExampleLanguage);
    }

    private static LDraft TDraftSentenceApply(LEngine engine, LDraft draft, string text, string language)
    {
        Assert.NotNull(draft.LDraftExample);

        engine.TEngineRequestApply(TInterface.TExampleLanguageCreate(draft.LDraftId, language));
        return engine.TEngineRequestApply(
            TInterface.TExampleTextCreate(draft.LDraftId, TInterface.TStateValueCreate(text)));
    }
}
