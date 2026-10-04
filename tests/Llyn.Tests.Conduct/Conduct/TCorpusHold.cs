using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCorpusHold
{
    [Fact]
    public void CorpusCreate_TranscriptEdited_HandsTheHeldExampleThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineDelaySet(0);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        int marshalled = 0;
        CCorpus corpus = CCorpus.CCorpusCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusScribeToggle(true);
        List<CExample> drafted = [];
        corpus.CCorpusTranscript.CTranscriptDraftChanged += drafted.Add;

        corpus.CCorpusTranscript.CTranscriptMentionAdd("a cat sat", 2, 3, 0);
        corpus.CCorpusDesk.CDeskPersist();

        Assert.True(marshalled > 0);
        Assert.Single(drafted);
        Assert.Equal("a cat sat", drafted[^1].CExampleText.CStateValueText);
    }

    [Fact]
    public void CorpusTranscriptChanged_StoredExampleThenCancel_RaisesItsTallyThenABlankExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        engine.TRequestQuoteApply(TCorpus.TCorpusEntrySave(engine).LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<CExample> held = [];
        corpus.CCorpusTranscriptChanged += held.Add;
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusScribeToggle(true);
        CExample stored = held[^1];

        corpus.CCorpusSession.CSessionCancel();

        string once = TInterface.TLocalizationTextRead("Example.UsageOne");
        Assert.Equal(
            ("a cat sat", "Example.Text", once),
            (stored.CExampleText.CStateValueText, stored.CExampleTextHint, stored.CExampleTally));
        CExample blank = held[^1];
        Assert.Equal(
            (string.Empty, "Example.Text", once),
            (blank.CExampleText.CStateValueText, blank.CExampleTextHint, blank.CExampleTally));
        Assert.Empty(blank.CExampleGloss);
    }

    [Fact]
    public void AtelierClose_QuotingEntryOnTheEditor_CancelsTheEditorDeskAndStopsPlayback()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int stopped = 0;
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ =>
            {
                stopped++;
                return null;
            },
        });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, media);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusExampleCreate();
        Assert.True(corpus.CCorpusEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(corpus.CCorpusEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }
}
