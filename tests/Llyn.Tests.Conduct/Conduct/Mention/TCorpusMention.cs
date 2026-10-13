using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCorpusMention
{
    [Fact]
    public void ChipRead_EngineFails_ShowsTheFindFailureAndAnswersNoChips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TTranscriptMention.TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        IReadOnlyList<CMentionLabel> labels = TInterfaceMention.TMentionFailRead(
            engine, corpus.CCorpusDesk, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Empty(labels);
        Assert.Equal(["Mention.FindFailed"], asked);
    }

    [Fact]
    public void MentionSenseRead_EngineFails_ShowsTheFindFailureAndAnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TTranscriptMention.TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        CMentionSense? menu = TInterfaceMention.TMentionSenseRead(
            engine,
            corpus.CCorpusDesk,
            TEnvoyFake.TEnvoyCreate(false, asked),
            _ => throw new InvalidOperationException("no meanings"));

        Assert.Null(menu);
        Assert.Equal(["Mention.FindFailed"], asked);
    }

    [Fact]
    public void MentionSenseRead_ReadyRows_LeadsWithTheWholeEntryAndMapsEachRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        List<string> keys = [];
        CCorpus corpus = TTranscriptMention.TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        CMentionSense? menu = TInterfaceMention.TMentionSenseRead(
            engine,
            corpus.CCorpusDesk,
            TEnvoyFake.TEnvoyCreate(false, asked),
            args =>
            {
                keys.Add((string)args![6]!);
                return new List<(long LMeaningId, string LMeaningName, int LMeaningDepth)>
                {
                    (1, "first", 0),
                    (2, "first-a", 1),
                    (3, "second", 0),
                };
            });

        Assert.Equal("Mention.Sense", menu!.CMentionSenseKey);
        Assert.Equal(
            [
                (0L, TInterface.TLocalizationTextRead("Mention.Whole"), 0),
                (1L, "first", 0),
                (2L, "first-a", 1),
                (3L, "second", 0),
            ],
            menu.CMentionSenseRow.Select(static row => (row.CMeaningId, row.CMeaningName, row.CMeaningDepth)).ToList());
        Assert.Equal(["Display.Unknown"], keys);
        Assert.Empty(asked);
    }

    [Fact]
    public void MentionSenseRead_EngineFails_HandsTheEnvoyTheReadyNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TTranscriptMention.TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<(string, CLedgerNotice)> handed = [];
        CEnvoy envoy = TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyFailureShow"] = args =>
            {
                handed.Add(((string)args![0]!, (CLedgerNotice)args[1]!));
                return null;
            },
        });

        TInterfaceMention.TMentionSenseRead(
            engine, corpus.CCorpusDesk, envoy, _ => throw new InvalidOperationException("no meanings"));

        (string key, CLedgerNotice notice) = Assert.Single(handed);
        Assert.Equal("Mention.FindFailed", key);
        Assert.Equal("Notice.Unexpected", notice.CLedgerNoticeKey);
    }

    [Fact]
    public void CorpusMentionFind_NoChosenExample_FindsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Null(corpus.CCorpusMentionFind("a dog", 2));
        Assert.Empty(asked);
    }

    [Fact]
    public void CorpusMentionFind_UnsavedTranscriptKept_AsksAndOffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        corpus.CCorpusDiptych.CDiptychEntryCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterfaceState.TStateValueCreate("a dog")));

        Assert.Null(corpus.CCorpusMentionFind("a dog", 2));
        Assert.Equal(["Leave"], asked);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);
    }

    [Fact]
    public void AnthologyMentionFind_EngineFails_ShowsTheFindFailureAndOffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        List<string> asked = [];

        CMentionOffer? offer = TInterfaceMention.TAnthologyMentionFind(
            engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked), cat.LExampleId, null);

        Assert.Null(offer);
        Assert.Equal(["Mention.FindFailed"], asked);
    }

    [Fact]
    public void AnthologyMentionFind_OneEntry_OpensItAndOffersAnEmptyMenu()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        List<long> arrived = TMentionLibraryAdd(atelier);

        CMentionOffer? offer = TInterfaceMention.TAnthologyMentionFind(
            engine, atelier, TEnvoyFake.TEnvoyCreate(false, []), cat.LExampleId, [8]);

        Assert.NotNull(offer);
        Assert.Equal(2, offer!.CMentionOfferUnit);
        Assert.Empty(offer.CMentionOfferEntry);
        Assert.Equal([8L], arrived);
    }

    [Fact]
    public void AnthologyMentionFind_ManyEntries_OffersThemUnderTheFoundWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        List<long> arrived = TMentionLibraryAdd(atelier);

        CMentionOffer? offer = TInterfaceMention.TAnthologyMentionFind(
            engine, atelier, TEnvoyFake.TEnvoyCreate(false, []), cat.LExampleId, [8, 9]);

        Assert.NotNull(offer);
        Assert.Equal(2, offer!.CMentionOfferUnit);
        Assert.Equal([8L, 9L], offer.CMentionOfferEntry.Select(entry => entry.CTranslationTargetId));
        Assert.Empty(arrived);
    }

    private static List<long> TMentionLibraryAdd(CAtelier atelier)
    {
        List<long> arrived = [];
        atelier.CAtelierNavigation.TNavigationTabAdd(
            "Library", static () => true, static () => 0, static _ => { }, arrived.Add);
        return arrived;
    }
}
