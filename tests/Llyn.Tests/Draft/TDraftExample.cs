using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftExample
{
    [Fact]
    public void ExampleSave_HeldSentence_SurvivesScan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long held;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft started = engine.TEngineExampleStart("Corpus", null);
            held = started.LDraftId;

            engine.TEngineExampleSave(started with
            {
                LDraftExample = TDraftSentenceCreate(started, "she knelt to kindle the damp logs"),
            });

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

        engine.TEngineExampleSave(started with
        {
            LDraftExample = TDraftSentenceCreate(started, "she knelt to kindle the damp logs"),
        });

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

            LExample sentence = TDraftSentenceCreate(committed, "she knelt to kindle the damp logs");
            engine.TEngineExampleSave(committed with { LDraftExample = sentence });

            LExample stored = engine.TEngineExampleCreate(sentence);

            engine.TEngineExampleSave(committed with
            {
                LDraftEntry = stored.LExampleId,
                LDraftExample = stored,
            });
            engine.TEngineExampleSave(edited with
            {
                LDraftExample = TDraftSentenceCreate(edited, "the ember still glowed"),
            });
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

        engine.TEngineExampleSave(started with
        {
            LDraftExample = TDraftSentenceCreate(started, "she knelt to kindle the damp logs") with
            {
                LExampleLanguage = string.Empty,
            },
        });

        LExample stored = engine.TEngineExampleCommit(started.LDraftId);

        Assert.Equal(string.Empty, stored.LExampleLanguage);

        LExample? written = engine.TEngineExampleRead(stored.LExampleId);

        Assert.NotNull(written);
        Assert.Equal(string.Empty, written.LExampleLanguage);
    }

    private static LExample TDraftSentenceCreate(LDraft draft, string text)
    {
        Assert.NotNull(draft.LDraftExample);

        return draft.LDraftExample with
        {
            LExampleLanguage = "English",
            LExampleText = TInterface.TStateValueCreate(text),
        };
    }
}
