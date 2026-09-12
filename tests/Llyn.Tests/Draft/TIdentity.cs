using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TIdentity
{
    [Fact]
    public void DraftSave_TwoNewSentencesSharingText_MintsTwoIdsAndCommitStoresTwoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        LEntryDraft stored = engine.TEngineDraftSave(started with
        {
            LDraftContent = TIdentityContentCreate(
                [
                    TInterface.TSentenceDraftCreate("she knelt to kindle the damp logs"),
                    TInterface.TSentenceDraftCreate("she knelt to kindle the damp logs"),
                ]),
        });

        IReadOnlyList<LSentenceDraft> sentences = stored.LEntryDraftMeanings[0].LCardDraftSentence;
        Assert.Equal(2, sentences.Count);
        Assert.True(sentences[0].LSentenceDraftId < 0);
        Assert.True(sentences[1].LSentenceDraftId < 0);
        Assert.NotEqual(sentences[0].LSentenceDraftId, sentences[1].LSentenceDraftId);
        Assert.True(sentences[0].LSentenceDraftExample!.LExampleDraftId < 0);
        Assert.NotEqual(
            sentences[0].LSentenceDraftExample!.LExampleDraftId,
            sentences[1].LSentenceDraftExample!.LExampleDraftId);

        LOutcome outcome = engine.TEngineOutcomeCommit(started.LDraftId);

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
        Assert.NotEqual(
            outcome.LOutcomeIdentity[sentences[0].LSentenceDraftExample!.LExampleDraftId],
            outcome.LOutcomeIdentity[sentences[1].LSentenceDraftExample!.LExampleDraftId]);
    }

    [Fact]
    public void DraftSave_SentencePickingStoredExample_KeepsIdAndCommitReusesRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0,
            "English",
            TInterface.TStateValueCreate("the speech kindled a hope"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateAnchorRead(null)));

        LDraft started = engine.TEngineDraftStart("editor", null);
        LEntryDraft stored = engine.TEngineDraftSave(started with
        {
            LDraftContent = TIdentityContentCreate(
                [
                    TInterface.TSentenceDraftCreate(
                        example.LExampleText, example.LExampleId, TInterface.TStateAnchorRead(null)),
                ]),
        });

        LSentenceDraft sentence = Assert.Single(stored.LEntryDraftMeanings[0].LCardDraftSentence);
        Assert.Equal(example.LExampleId, sentence.LSentenceDraftExample!.LExampleDraftId);

        LOutcome outcome = engine.TEngineOutcomeCommit(started.LDraftId);

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.DoesNotContain(example.LExampleId, outcome.LOutcomeIdentity.Keys);
        Assert.Equal(
            example.LExampleId,
            workspace.TWorkspaceCountRead("SELECT example_id FROM sense_example;"));
    }

    [Fact]
    public void DraftCommit_EveryNegativeIdHeld_AppearsInTheOutcomeMap()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        LEntryDraft content = TIdentityContentCreate(
            [TInterface.TSentenceDraftCreate("he kindled the dry brush")]);
        LCardDraft card = content.LEntryDraftMeanings[0] with
        {
            LCardDraftSituation = [TInterface.TSituationDraftCreate("story telling")],
            LCardDraftRegister = [TInterface.TRegisterDraftCreate("gruff")],
            LCardDraftTag = TInterface.TTagDraftCreate("fire", "literary"),
            LCardDraftImage = [TInterface.TImageDraftCreate("media/fire.jpg")],
            LCardDraftVideo = [TInterface.TVideoDraftCreate("media/kindling.mp4", "00:12-00:19")],
        };

        LEntryDraft stored = engine.TEngineDraftSave(started with
        {
            LDraftContent = content with { LEntryDraftMeanings = [card] },
        });

        List<long> minted = [];
        TIdentityNegativeRead(stored, minted);
        Assert.Equal(10, minted.Count);
        Assert.All(minted, id => Assert.True(id < 0));

        LOutcome outcome = engine.TEngineOutcomeCommit(started.LDraftId);

        Assert.Equal(minted.Count, outcome.LOutcomeIdentity.Count);
        foreach (long id in minted)
        {
            Assert.True(outcome.LOutcomeIdentity.TryGetValue(id, out long real));
            Assert.True(real > 0);
        }

        LCardDraft loaded = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(outcome.LOutcomeEntry.LEntryId)).LEntryDraftMeanings[0];
        Assert.Equal(outcome.LOutcomeIdentity[card.LCardDraftId], loaded.LCardDraftId);
        Assert.Equal(
            outcome.LOutcomeIdentity[stored.LEntryDraftMeanings[0].LCardDraftSituation[0].LSituationDraftId],
            loaded.LCardDraftSituation[0].LSituationDraftId);
        Assert.Equal(
            outcome.LOutcomeIdentity[stored.LEntryDraftMeanings[0].LCardDraftRegister[0].LRegisterDraftId],
            loaded.LCardDraftRegister[0].LRegisterDraftId);
        Assert.Equal(
            outcome.LOutcomeIdentity[stored.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftId],
            loaded.LCardDraftSentence[0].LSentenceDraftId);
    }

    [Fact]
    public void DraftArchiveSave_ItemWithoutId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        LDraft draft = TInterface.TDraftCreate(
            TInterface.TIdentityCreate(),
            "editor",
            0,
            TIdentityContentCreate([TInterface.TSentenceDraftCreate("he kindled the dry brush")]),
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, draft));
    }

    private static LEntryDraft TIdentityContentCreate(IReadOnlyList<LSentenceDraft> sentences)
    {
        return TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            "/ˈkɪnd(ə)l/",
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    TInterface.TStateValueCreate("set alight"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("to set something burning"),
                    sentences,
                    [],
                    [],
                    string.Empty,
                    [],
                    [],
                    1,
                    TInterface.TIdentityCreate()),
            ],
            []);
    }

    private static void TIdentityNegativeRead(LEntryDraft content, List<long> minted)
    {
        if (content.LEntryDraftPronunciation is LPronunciationDraft spoken)
        {
            TIdentityNegativeAdd(spoken.LPronunciationDraftId, minted);
        }

        TIdentityNegativeRead(content.LEntryDraftMeanings, minted);
        TIdentityNegativeRead(content.LEntryDraftCollocations, minted);
    }

    private static void TIdentityNegativeRead(IReadOnlyList<LCardDraft> cards, List<long> minted)
    {
        foreach (LCardDraft card in cards)
        {
            TIdentityNegativeAdd(card.LCardDraftId, minted);
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                TIdentityNegativeAdd(sentence.LSentenceDraftId, minted);
                TIdentityNegativeAdd(sentence.LSentenceDraftExample?.LExampleDraftId ?? 0, minted);
            }

            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                TIdentityNegativeAdd(situation.LSituationDraftId, minted);
            }

            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                TIdentityNegativeAdd(register.LRegisterDraftId, minted);
            }

            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                TIdentityNegativeAdd(tag.LTagDraftId, minted);
            }

            foreach (LImageDraft image in card.LCardDraftImage)
            {
                TIdentityNegativeAdd(image.LImageDraftId, minted);
            }

            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                TIdentityNegativeAdd(video.LVideoDraftId, minted);
            }

            TIdentityNegativeRead(card.LCardDraftChild, minted);
        }
    }

    private static void TIdentityNegativeAdd(long id, List<long> minted)
    {
        if (id < 0)
        {
            minted.Add(id);
        }
    }
}
