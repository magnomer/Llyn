using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTranscriptMention
{
    private const string TMentionText = "she knelt to kindle the damp logs";

    [Fact]
    public void TranscriptSenseSet_LinkedMention_OffersItsMeaningsAndNarrowsItToTheSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, long sense) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 13, 6, entry);

        Assert.True(corpus.CCorpusTranscript.CTranscriptSenseCheck(TMentionText, 13, 6));
        CMentionSense menu = corpus.CCorpusTranscript.CTranscriptSenseRead(TMentionText, 13, 6)!;
        Assert.Equal("Mention.Sense", menu.CMentionSenseKey);
        Assert.Equal(new CMeaning(0, TInterface.TLocalizationTextRead("Mention.Whole"), 0), menu.CMentionSenseRow[0]);
        Assert.Equal([0L, sense], menu.CMentionSenseRow.Select(static meaning => meaning.CMeaningId));
        corpus.CCorpusTranscript.CTranscriptSenseSet(TMentionText, 13, 6, sense);

        Assert.Equal(sense, Assert.Single(TMentionRead(corpus)).LMentionSenseId);
    }

    [Fact]
    public void TranscriptMentionCheck_SilentMention_AllowsUnlinkButOffersNoSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));

        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 13, 6, 0);

        Assert.True(corpus.CCorpusTranscript.CTranscriptMentionCheck(TMentionText, 13, 6));
        Assert.False(corpus.CCorpusTranscript.CTranscriptMentionCheck(TMentionText, 0, 3));
        Assert.False(corpus.CCorpusTranscript.CTranscriptSenseCheck(TMentionText, 13, 6));
        Assert.Null(corpus.CCorpusTranscript.CTranscriptSenseRead(TMentionText, 13, 6));
    }

    [Fact]
    public void TranscriptMentionRemove_BySelectionAndByChip_DropsEachMention()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, _) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 13, 6, entry);
        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 29, 4, 0);

        corpus.CCorpusTranscript.CTranscriptMentionRemove(TMentionText, 13, 6);
        long silent = Assert.Single(TMentionRead(corpus)).LMentionId;
        corpus.CCorpusTranscript.CTranscriptMentionRemove(silent);

        Assert.Empty(TMentionRead(corpus));
    }

    [Fact]
    public void TranscriptMentionRead_LinkedAndSilentMentions_NamesTheHeadwordAndKeysTheSilentChip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, _) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 13, 6, entry);
        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 29, 4, 0);

        IReadOnlyList<CMentionLabel> labels = corpus.CCorpusTranscript.CTranscriptMentionRead();

        Assert.Equal(2, labels.Count);
        Assert.Equal("kindle", labels[0].CMentionLabelWord);
        Assert.Equal("kindle", labels[0].CMentionLabelName);
        Assert.Null(labels[0].CMentionLabelKey);
        Assert.Equal("logs", labels[1].CMentionLabelWord);
        Assert.Equal("Mention.Silent", labels[1].CMentionLabelKey);
    }

    [Fact]
    public void TranscriptMentionRead_NoExampleHeld_AnswersNoChipsAndOffersNoMeanings()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 13, 6, 0);

        Assert.Empty(corpus.CCorpusTranscript.CTranscriptMentionRead());
        Assert.Null(corpus.CCorpusTranscript.CTranscriptSenseRead(TMentionText, 13, 6));
        Assert.False(corpus.CCorpusTranscript.CTranscriptMentionCheck(TMentionText, 13, 6));
        Assert.False(corpus.CCorpusTranscript.CTranscriptSenseCheck(TMentionText, 13, 6));
    }

    [Fact]
    public void TranscriptMentionOpen_SelectedWord_OffersTheExampleLanguageAndLinksThePick()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long english = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        engine.TEngineTranslationCreate("cat", "French");
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusScribeToggle(true);

        List<CProspect> offered = [];
        corpus.CCorpusTranscript.CTranscriptMentionOffered += offered.Add;

        corpus.CCorpusTranscript.CTranscriptMentionOpen("cat");
        corpus.CCorpusTranscript.CTranscriptMentionAdd("a cat sat", 2, 3, english);

        Assert.Equal(english, Assert.Single(Assert.Single(offered).CProspectRows).CVistaRowId);
        Assert.True(offered[0].CProspectShown);
        CMentionLabel linked = Assert.Single(corpus.CCorpusTranscript.CTranscriptMentionRead());
        Assert.Equal("cat", linked.CMentionLabelWord);
        Assert.True(linked.CMentionLabelLinked);
    }

    [Fact]
    public void TranscriptMentionAdd_SpanOverEntry_LinksTheSpanWithoutASense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, _) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));

        corpus.CCorpusTranscript.CTranscriptMentionAdd(TMentionText, 13, 6, entry);

        LMention added = Assert.Single(TMentionRead(corpus));
        Assert.Equal((13, 6), (added.LMentionOffset, added.LMentionLength));
        Assert.Equal(entry, added.LMentionEntryId);
        Assert.Equal(0, added.LMentionSenseId);
    }

    internal static CCorpus TMentionPrepare(LEngine engine, CAtelier atelier, CEnvoy envoy)
    {
        engine.TEngineDelaySet(0);
        LExample stored = TCorpus.TCorpusExampleSave(engine, TMentionText);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, envoy);
        corpus.TCorpusExampleOpen(stored.LExampleId);
        corpus.CCorpusScribeToggle(true);
        return corpus;
    }

    private static IReadOnlyList<LMention> TMentionRead(CCorpus corpus)
    {
        return corpus.CCorpusDesk.TDeskRead()!.LDraftExample!.LExampleMention;
    }
}
