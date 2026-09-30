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
    private const string TMentionText = "she knelt to kindle the damp logs";

    [Fact]
    public void CorpusSenseSet_LinkedMention_OffersItsMeaningsAndNarrowsItToTheSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, long sense) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusMentionAdd(TMentionText, 13, 6, entry);

        Assert.True(corpus.CCorpusSenseCheck(TMentionText, 13, 6));
        CMentionSense menu = corpus.CCorpusSenseRead(TMentionText, 13, 6)!;
        Assert.Equal("Mention.Sense", menu.CMentionSenseKey);
        Assert.Equal(new CMeaning(0, TInterface.TLocalizationTextRead("Mention.Whole"), 0), menu.CMentionSenseRow[0]);
        Assert.Equal([0L, sense], menu.CMentionSenseRow.Select(static meaning => meaning.CMeaningId));
        corpus.CCorpusSenseSet(TMentionText, 13, 6, sense);

        Assert.Equal(sense, Assert.Single(TMentionRead(corpus)).LMentionSenseId);
    }

    [Fact]
    public void CorpusMentionCheck_SilentMention_AllowsUnlinkButOffersNoSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));

        corpus.CCorpusMentionAdd(TMentionText, 13, 6, 0);

        Assert.True(corpus.CCorpusMentionCheck(TMentionText, 13, 6));
        Assert.False(corpus.CCorpusMentionCheck(TMentionText, 0, 3));
        Assert.False(corpus.CCorpusSenseCheck(TMentionText, 13, 6));
        Assert.Null(corpus.CCorpusSenseRead(TMentionText, 13, 6));
    }

    [Fact]
    public void CorpusMentionRemove_BySelectionAndByChip_DropsEachMention()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, _) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusMentionAdd(TMentionText, 13, 6, entry);
        corpus.CCorpusMentionAdd(TMentionText, 29, 4, 0);

        corpus.CCorpusMentionRemove(TMentionText, 13, 6);
        long silent = Assert.Single(TMentionRead(corpus)).LMentionId;
        corpus.CCorpusMentionRemove(silent);

        Assert.Empty(TMentionRead(corpus));
    }

    [Fact]
    public void CorpusMentionRead_LinkedAndSilentMentions_NamesTheHeadwordAndKeysTheSilentChip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, _) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusMentionAdd(TMentionText, 13, 6, entry);
        corpus.CCorpusMentionAdd(TMentionText, 29, 4, 0);

        IReadOnlyList<CMentionLabel> labels = corpus.CCorpusMentionRead();

        Assert.Equal(2, labels.Count);
        Assert.Equal("kindle", labels[0].CMentionLabelWord);
        Assert.Equal("kindle", labels[0].CMentionLabelName);
        Assert.Null(labels[0].CMentionLabelKey);
        Assert.Equal("logs", labels[1].CMentionLabelWord);
        Assert.Equal("Mention.Silent", labels[1].CMentionLabelKey);
    }

    [Fact]
    public void CorpusMentionRead_NoTranscriptHeld_AnswersNoChipsAndOffersNoMeanings()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        corpus.CCorpusMentionAdd(TMentionText, 13, 6, 0);

        Assert.Empty(corpus.CCorpusMentionRead());
        Assert.Null(corpus.CCorpusSenseRead(TMentionText, 13, 6));
        Assert.False(corpus.CCorpusMentionCheck(TMentionText, 13, 6));
        Assert.False(corpus.CCorpusSenseCheck(TMentionText, 13, 6));
    }

    [Fact]
    public void ChipRead_EngineFails_ShowsTheFindFailureAndAnswersNoChips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked));

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
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        CMentionSense? menu = TInterfaceMention.TMentionSenseRead(
            engine,
            corpus.CCorpusDesk,
            TEnvoyFake.TEnvoyCreate(false, asked),
            _ => throw new InvalidOperationException("no meanings"));

        Assert.Null(menu);
        Assert.Equal(["Mention.FindFailed"], asked);
    }

    [Fact]
    public void CorpusMention_TranscriptWord_OffersTheExampleLanguageAndLinksThePick()
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
        corpus.CCorpusMentionOffered += offered.Add;

        corpus.CCorpusMentionOpen("cat");
        corpus.CCorpusMentionAdd("a cat sat", 2, 3, english);

        Assert.Equal(english, Assert.Single(Assert.Single(offered).CProspectRows).CVistaRowId);
        Assert.True(offered[0].CProspectShown);
        CMentionLabel linked = Assert.Single(corpus.CCorpusMentionRead());
        Assert.Equal("cat", linked.CMentionLabelWord);
        Assert.True(linked.CMentionLabelLinked);
    }

    [Fact]
    public void CorpusMentionAdd_SpanOverEntry_LinksTheSpanWithoutASense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (long entry, _) = TCardMention.TMentionEntryCreate(engine);
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));

        corpus.CCorpusMentionAdd(TMentionText, 13, 6, entry);

        LMention added = Assert.Single(TMentionRead(corpus));
        Assert.Equal((13, 6), (added.LMentionOffset, added.LMentionLength));
        Assert.Equal(entry, added.LMentionEntryId);
        Assert.Equal(0, added.LMentionSenseId);
    }

    [Fact]
    public void MentionSenseRead_ReadyRows_LeadsWithTheWholeEntryAndMapsEachRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        List<string> keys = [];
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked));

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
        CCorpus corpus = TMentionPrepare(engine, atelier, TEnvoyFake.TEnvoyCreate(false, []));
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

        Assert.Null(corpus.CCorpusMentionFind(2));
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
        corpus.CCorpusExampleCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterface.TStateValueCreate("a dog")));

        Assert.Null(corpus.CCorpusMentionFind(2));
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
        Assert.Equal(2, offer!.CMentionOfferOffset);
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
        Assert.Equal(2, offer!.CMentionOfferOffset);
        Assert.Equal([8L, 9L], offer.CMentionOfferEntry.Select(entry => entry.CTranslationTargetId));
        Assert.Empty(arrived);
    }

    private static CCorpus TMentionPrepare(LEngine engine, CAtelier atelier, CEnvoy envoy)
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

    private static List<long> TMentionLibraryAdd(CAtelier atelier)
    {
        List<long> arrived = [];
        atelier.CAtelierNavigation.TNavigationTabAdd(
            "Library", static () => true, static () => 0, static _ => { }, arrived.Add);
        return arrived;
    }
}
