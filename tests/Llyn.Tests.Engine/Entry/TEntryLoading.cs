using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryLoading
{
    [Fact]
    public void EntryLoad_FrameWithNoExample_KeepsTheFrame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "wait",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "to stay until something happens",
                    [TInterface.TSentenceDraftCreate(
                        LStateValue.LStateValueUnspecified, 0,
                        LStateAnchor.LStateAnchorUnspecified, "for", "Patient")],
                    [], [], [], [], 1),
            ],
            []));

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LSentenceDraft sentence = Assert.Single(
            Assert.Single(draft.LEntryDraftMeanings).LCardDraftSentence);

        Assert.Equal("for", sentence.LSentenceDraftParticle.TStateValueShow());
        Assert.Equal("Patient", sentence.LSentenceDraftDependence.TStateValueShow());
        Assert.Null(sentence.LSentenceDraftExample);

        Assert.Empty(engine.TEngineUsageRead(LOwner.LOwnerExample));
    }

    [Fact]
    public void EntryLoad_VideoWithSpan_ReturnsBothLocationAndSpan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "to set alight",
                    [], [], [], [], [], 1,
                    video:
                    [
                        TInterface.TVideoDraftCreate(
                            "https://www.youtube.com/watch?v=IDw0RmBtOho", "00:30 - 02:30"),
                    ]),
            ],
            []));

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LVideoDraft video = Assert.Single(Assert.Single(draft.LEntryDraftMeanings).LCardDraftVideo);

        Assert.Equal("https://www.youtube.com/watch?v=IDw0RmBtOho", video.LVideoDraftLocation.TStateValueShow());
        Assert.Equal("00:30 - 02:30", video.LVideoDraftSpan.TStateValueShow());
    }

    [Fact]
    public void EntryLoad_SavedEntry_ReturnsSavedDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntryDraft word = TInterface.TEntryDraftCreate(
            "word",
            "English",
            "wɜːd",
            "a note",
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "a unit of language",
                    [TInterface.TSentenceDraftCreate("he said a word")],
                    [TInterface.TSituationDraftCreate("conversation")],
                    [], ["spoken"], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "a promise", [], [], [], [], [], 2),
            ],
            [
                TInterface.TCardDraftCreate(
                    string.Empty, "in a word", "briefly",
                    [TInterface.TSentenceDraftCreate("in a word, no")],
                    [TInterface.TSituationDraftCreate("summary")],
                    [], ["written"], [], 1),
            ]);

        LEntryDraft sword = TInterface.TEntryDraftCreate(
            "sword",
            "English",
            "sɔːd",
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a bladed weapon", [], [], [], [], [], 1)],
            []);

        LEntry stored = engine.TEngineEntrySave(word);
        engine.TEngineEntrySave(sword);

        Assert.Equal(["sword", "word"], engine.TEngineEntryFind(string.Empty).Select(e => e.LEntryHeadword));

        Assert.Equal(["sword", "word"], engine.TEngineEntryFind("WORD").Select(e => e.LEntryHeadword));
        Assert.Equal("sword", Assert.Single(engine.TEngineEntryFind("swo")).LEntryHeadword);
        Assert.Empty(engine.TEngineEntryFind("axe"));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(word.LEntryDraftHeadword, loaded.LEntryDraftHeadword);
        Assert.Equal(word.LEntryDraftLanguage, loaded.LEntryDraftLanguage);
        Assert.Equal(word.LEntryDraftIpa, loaded.LEntryDraftIpa);
        Assert.Equal(word.LEntryDraftNote, loaded.LEntryDraftNote);

        TEntryCardMatch(word.LEntryDraftMeanings, loaded.LEntryDraftMeanings);
        TEntryCardMatch(word.LEntryDraftCollocations, loaded.LEntryDraftCollocations);
    }

    private static IReadOnlyList<string> TEntrySituationRead(IReadOnlyList<LSituationDraft> drafts)
    {
        List<string> texts = new(drafts.Count);
        foreach (LSituationDraft draft in drafts)
        {
            texts.Add(draft.LSituationDraftTitle.TStateValueShow());
        }

        return texts;
    }

    private static IReadOnlyList<string> TEntryExampleRead(IReadOnlyList<LSentenceDraft> drafts)
    {
        List<string> texts = new(drafts.Count);
        foreach (LSentenceDraft draft in drafts)
        {
            texts.Add(draft.LSentenceDraftExample is LExampleDraft example
                ? example.LExampleDraftText.TStateValueShow()
                : string.Empty);
        }

        return texts;
    }

    private static void TEntryCardMatch(IReadOnlyList<LCardDraft> expected, IReadOnlyList<LCardDraft> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int index = 0; index < expected.Count; index++)
        {
            Assert.Equal(expected[index].LCardDraftTitle, actual[index].LCardDraftTitle);
            Assert.Equal(expected[index].LCardDraftExpression, actual[index].LCardDraftExpression);
            Assert.Equal(expected[index].LCardDraftMeaning, actual[index].LCardDraftMeaning);
            Assert.Equal(
                TEntryExampleRead(expected[index].LCardDraftSentence),
                TEntryExampleRead(actual[index].LCardDraftSentence));
            Assert.Equal(
                TEntrySituationRead(expected[index].LCardDraftSituation),
                TEntrySituationRead(actual[index].LCardDraftSituation));
            Assert.Equal(
                TInterface.TTagDraftRead(expected[index].LCardDraftTag),
                TInterface.TTagDraftRead(actual[index].LCardDraftTag));
            Assert.Equal(
                expected[index].LCardDraftTranslation, actual[index].LCardDraftTranslation);
        }
    }

    [Fact]
    public void EntryLoad_DownloadedRecording_ReturnsResolvedPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, "word.mp3");
        File.WriteAllBytes(file, [0]);

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [],
            [],
            file,
            "Wiktionary"));

        LPronunciationArchive pronunciations = TInterface.TPronunciationArchiveCreate(workspace.TWorkspaceDatabase);
        LPronunciation pronunciation = Assert.Single(pronunciations.TPronunciationRead(stored.LEntryId));

        LPronunciationAudio? audio = pronunciations.TPronunciationAudioRead(pronunciation.LPronunciationId);
        Assert.NotNull(audio);
        Assert.Equal(Path.Combine("audio", "english", "word.mp3"), audio.LPronunciationAudioFile);
        Assert.Equal("Wiktionary", audio.LPronunciationAudioSource);

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(file, loaded.LEntryDraftAudio);
        Assert.Equal("Wiktionary", loaded.LEntryDraftPronunciation!.LPronunciationDraftSource);
    }

    [Fact]
    public void EntryLoad_NoRecordingSaved_ReturnsNoRecording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word", "English", "wɜːd", string.Empty, [], []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(string.Empty, loaded.LEntryDraftAudio);
        Assert.Null(loaded.LEntryDraftPronunciation!.LPronunciationDraftSource);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pronunciation_audio;"));
    }

    [Fact]
    public void EntryLoad_TitleOnBothCardKinds_KeepsThemApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate("the plain meaning", string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(
                "the set phrase", "in a word", "briefly", [], [], [], [], [], 1)]));

        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(stored.LEntryId));
        Assert.Equal("the plain meaning", meaning.LMeaningTitle);
        LCollocation collocation = Assert.Single(
            TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase).TCollocationRead(stored.LEntryId));
        Assert.Equal("the set phrase", collocation.LCollocationTitle);

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal("the plain meaning", Assert.Single(loaded.LEntryDraftMeanings).LCardDraftTitle);
        Assert.Equal(
            "the set phrase",
            Assert.Single(loaded.LEntryDraftCollocations).LCardDraftTitle);
    }

    [Fact]
    public void EntryLoad_CardReferences_ReturnsWrittenOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a unit of language",
                [
                    TInterface.TSentenceDraftCreate("he said a word"),
                    TInterface.TSentenceDraftCreate("not a word was spoken"),
                ],
                [TInterface.TSituationDraftCreate("conversation")],
                [],
                ["verb", "formal", "spoken"], [], 1)],
            [TInterface.TCardDraftCreate(
                string.Empty,
                "in a word",
                "briefly",
                [TInterface.TSentenceDraftCreate("in a word, no")],
                [TInterface.TSituationDraftCreate("summary"), TInterface.TSituationDraftCreate("writing")],
                [],
                ["written", "idiom"], [], 1)]));

        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(stored.LEntryId));
        Assert.Equal(
            ["verb", "formal", "spoken"],
            TInterface.TTagArchiveCreate(workspace.TWorkspaceDatabase)
                .TTagMeaningRead(meaning.LMeaningId)
                .Select(tag => tag.LTagText));
        Assert.Equal(
            [0L, 1L, 2L],
            workspace.TWorkspaceColumnRead("SELECT position FROM sense_tag ORDER BY position;"));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);

        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);
        Assert.Equal(["verb", "formal", "spoken"], TInterface.TTagDraftRead(card.LCardDraftTag));
        Assert.Equal(
            ["he said a word", "not a word was spoken"],
            TEntryExampleRead(card.LCardDraftSentence));
        Assert.Equal(["conversation"], TEntrySituationRead(card.LCardDraftSituation));

        LCardDraft phrase = Assert.Single(loaded.LEntryDraftCollocations);
        Assert.Equal(["written", "idiom"], TInterface.TTagDraftRead(phrase.LCardDraftTag));
        Assert.Equal(["in a word, no"], TEntryExampleRead(phrase.LCardDraftSentence));
        Assert.Equal(["summary", "writing"], TEntrySituationRead(phrase.LCardDraftSituation));
    }

    [Fact]
    public void EntryLoad_EmptyCardField_ReturnsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "a meaning",
                    [TInterface.TSentenceDraftCreate("  ")], [], [], [], [], 1),
            ],
            []));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);
        Assert.Empty(card.LCardDraftSentence);
        Assert.Empty(card.LCardDraftSituation);
        Assert.Empty(card.LCardDraftTag);
    }

    [Fact]
    public void EntryLoad_CardShifted_ReturnsStoredPosition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "second", [], [], [], [], [], 2),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "third", [], [], [], [], [], 3),
            ],
            []));

        LDraft started = engine.TEngineDraftStart("Input", stored.LEntryId);
        engine.TEngineRequestApply(TInterface.TRequestShiftCreate(
            started.LDraftId, started.LDraftContent.LEntryDraftMeanings[2].LCardDraftId, 0, 0));
        engine.TEngineDraftCommit(started.LDraftId);


        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            ["third", "first", "second"],
            loaded.LEntryDraftMeanings.Select(card => card.LCardDraftMeaning.TStateValueShow()));
        Assert.Equal([1, 2, 3], loaded.LEntryDraftMeanings.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void EntryLoad_UnknownId_ReturnsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TEngineEntryLoad(9999));
    }
}
