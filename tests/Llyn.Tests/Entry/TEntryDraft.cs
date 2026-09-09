using System;

using Llyn.Core;
using Llyn.ShellEngine;

using Xunit;

namespace Llyn.Tests;

public sealed class TEntryDraft
{
    private static readonly string[] TEntryDraftQuery =
    [
        "SELECT id, headword, language FROM entry;",
        "SELECT * FROM sense;",
        "SELECT * FROM collocation;",
        "SELECT * FROM example;",
        "SELECT * FROM sense_example;",
        "SELECT * FROM collocation_example;",
        "SELECT * FROM situation;",
        "SELECT * FROM sense_situation;",
        "SELECT * FROM collocation_situation;",
        "SELECT * FROM register;",
        "SELECT * FROM sense_register;",
        "SELECT * FROM collocation_register;",
        "SELECT * FROM sense_tag;",
        "SELECT * FROM collocation_tag;",
        "SELECT * FROM sense_translation;",
        "SELECT * FROM collocation_translation;",
        "SELECT * FROM image;",
        "SELECT * FROM sense_image;",
        "SELECT * FROM collocation_image;",
        "SELECT * FROM video;",
        "SELECT * FROM sense_video;",
        "SELECT * FROM collocation_video;",
    ];

    [Fact]
    public void EntryDraft_FrameWithoutExample_SurvivesSave()
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
                    string.Empty,
                    string.Empty,
                    "to stay until something happens",
                    [TInterface.TSentenceDraftCreate(
                        TInterface.TStateValueCreate("for"), TInterface.TStateValueCreate("Patient"))],
                    [],
                    [],
                    string.Empty,
                    [],
                    [],
                    1),
            ],
            []));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sense_example WHERE example_id IS NULL;"));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LSentenceDraft sentence = Assert.Single(
            Assert.Single(loaded.LEntryDraftMeanings).LCardDraftSentence);

        Assert.Null(sentence.LSentenceDraftExample);
        Assert.Equal("for", sentence.LSentenceDraftParticle.TStateValueShow());
        Assert.Equal("Patient", sentence.LSentenceDraftDependence.TStateValueShow());

        engine.TEngineEntryUpdate(entry.LEntryId, loaded);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sense_example WHERE example_id IS NULL;"));
        Assert.Empty(workspace.TWorkspaceRowRead("PRAGMA foreign_key_check;"));
    }

    [Fact]
    public void EntryDraft_ExampleSharedByTwoCards_SavesOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a unit of language",
                [TInterface.TSentenceDraftCreate("he said a word")],
                [],
                [],
                string.Empty,
                [],
                [],
                1)],
            [TInterface.TCardDraftCreate(
                string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LSentenceDraft quoted = Assert.Single(
            Assert.Single(loaded.LEntryDraftMeanings).LCardDraftSentence);
        LCardDraft phrase = Assert.Single(loaded.LEntryDraftCollocations);

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftCollocations = [phrase with { LCardDraftSentence = [quoted] }],
        });

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
        Assert.Equal(
            1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_example;"));

        LEntryDraft reread = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LSentenceDraft onSense = Assert.Single(
            Assert.Single(reread.LEntryDraftMeanings).LCardDraftSentence);
        LSentenceDraft onPhrase = Assert.Single(
            Assert.Single(reread.LEntryDraftCollocations).LCardDraftSentence);

        Assert.Equal(
            TEntryExampleRead(onSense).LExampleDraftId,
            TEntryExampleRead(onPhrase).LExampleDraftId);
        Assert.Empty(workspace.TWorkspaceRowRead("PRAGMA foreign_key_check;"));
    }

    [Fact]
    public void EntryDraft_NestedSense_KeepsTree()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [
                TEntryCardCreate("to set alight", 1) with
                {
                    LCardDraftChild =
                    [
                        TEntryCardCreate("to rouse a feeling", 1) with
                        {
                            LCardDraftChild = [TEntryCardCreate("to rouse a crowd", 1)],
                        },
                        TEntryCardCreate("to catch fire", 2),
                    ],
                },
                TEntryCardCreate("to bear young", 2),
            ],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));

        Assert.Equal(2, loaded.LEntryDraftMeanings.Count);
        LCardDraft first = loaded.LEntryDraftMeanings[0];
        Assert.Equal(2, first.LCardDraftChild.Count);
        Assert.Equal(
            "to rouse a crowd",
            Assert.Single(first.LCardDraftChild[0].LCardDraftChild).LCardDraftMeaning.TStateValueShow());
        Assert.Equal(1, first.LCardDraftChild[0].LCardDraftPosition);
        Assert.Equal(2, first.LCardDraftChild[1].LCardDraftPosition);
        Assert.Empty(loaded.LEntryDraftMeanings[1].LCardDraftChild);

        IReadOnlyList<string> before = workspace.TWorkspaceRowRead(
            "SELECT id, ifnull(parent_id, ''), position FROM sense ORDER BY id;");

        engine.TEngineEntryUpdate(entry.LEntryId, loaded);

        Assert.Equal(
            before,
            workspace.TWorkspaceRowRead(
                "SELECT id, ifnull(parent_id, ''), position FROM sense ORDER BY id;"));
        Assert.Empty(workspace.TWorkspaceRowRead("PRAGMA foreign_key_check;"));
    }

    [Fact]
    public void EntryDraft_NestedCollocation_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [],
            [
                TEntryCardCreate("briefly", 1) with
                {
                    LCardDraftChild = [TEntryCardCreate("in short", 1)],
                },
            ]);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntrySave(draft));
        Assert.Equal(LRefusal.LRefusalCollocation, refusal.LRefusalReason);
    }

    [Fact]
    public void EntryDraft_CardFields_SurviveSave()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry target = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "점화하다", "Korean", string.Empty, string.Empty, [], []));

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty,
                    string.Empty,
                    "to set alight",
                    [],
                    [TInterface.TSituationDraftCreate(
                        TInterface.TStateValueCreate("writing a formal letter"),
                        string.Empty,
                        TInterface.TStateValueCreate("the register an institution expects"),
                        TInterface.TStateValueCreate("writing"))],
                    [target.LEntryId],
                    string.Empty,
                    [],
                    [],
                    1,
                    video: [TInterface.TVideoDraftCreate("media/kindling.mp4", "00:12-00:19")],
                    register: [TInterface.TRegisterDraftCreate(
                        TInterface.TStateValueCreate("courtroom"), string.Empty, "English")]) with
                {
                    LCardDraftGloss = "ignite",
                    LCardDraftLanguage = "English",
                    LCardDraftLabels = "[\"literal\"]",
                },
            ],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);

        Assert.Equal("ignite", card.LCardDraftGloss);
        Assert.Equal("English", card.LCardDraftLanguage);
        Assert.Equal("[\"literal\"]", card.LCardDraftLabels);
        Assert.Equal([target.LEntryId], card.LCardDraftTranslation);

        LSituationDraft situation = Assert.Single(card.LCardDraftSituation);
        Assert.Equal("writing a formal letter", situation.LSituationDraftTitle.TStateValueShow());
        Assert.Equal(
            "the register an institution expects",
            situation.LSituationDraftDescription.TStateValueShow());
        Assert.Equal("writing", situation.LSituationDraftKind.TStateValueShow());

        LRegisterDraft register = Assert.Single(card.LCardDraftRegister);
        Assert.Equal("courtroom", register.LRegisterDraftName.TStateValueShow());
        Assert.Equal("English", register.LRegisterDraftLanguage);

        Assert.Equal("00:12-00:19", Assert.Single(card.LCardDraftVideo).LVideoDraftSpan.TStateValueShow());

        engine.TEngineEntryUpdate(entry.LEntryId, loaded);

        LEntryDraft reread = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        Assert.Equal(card.LCardDraftGloss, Assert.Single(reread.LEntryDraftMeanings).LCardDraftGloss);
        Assert.Equal(card.LCardDraftLabels, Assert.Single(reread.LEntryDraftMeanings).LCardDraftLabels);
        Assert.Equal(
            card.LCardDraftTranslation, Assert.Single(reread.LEntryDraftMeanings).LCardDraftTranslation);
    }

    [Fact]
    public void EntryDraft_UntouchedEntry_WritesNoChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry target = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "점화하다", "Korean", string.Empty, string.Empty, [], []));

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            "/ˈkɪnd(ə)l/",
            "Chiefly literary.",
            [
                TInterface.TCardDraftCreate(
                    TInterface.TStateValueCreate("set alight"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("to set something burning"),
                    [
                        TInterface.TSentenceDraftCreate("he kindled the dry brush"),
                        TInterface.TSentenceDraftCreate(
                            TInterface.TStateValueCreate("with"),
                            TInterface.TStateValueCreate("Instrument")),
                    ],
                    [TInterface.TSituationDraftCreate("story telling")],
                    [target.LEntryId],
                    string.Empty,
                    ["fire", "literary"],
                    [TInterface.TStateValueCreate("media/fire.jpg")],
                    1,
                    video: [TInterface.TVideoDraftCreate("media/kindling.mp4", "00:12-00:19")],
                    register: [TInterface.TRegisterDraftCreate("formal")]) with
                {
                    LCardDraftGloss = "ignite",
                    LCardDraftLanguage = "English",
                    LCardDraftLabels = "[\"literal\"]",
                    LCardDraftChild = [TEntryCardCreate("to rouse a feeling", 1)],
                },
            ],
            [TInterface.TCardDraftCreate(
                string.Empty,
                "kindle interest",
                "to make someone care",
                [TInterface.TSentenceDraftCreate("the speech kindled a hope")],
                [],
                [target.LEntryId],
                string.Empty,
                ["idiom"],
                [],
                1)]));

        IReadOnlyList<string> before = TEntryTableRead(workspace);

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        engine.TEngineEntryUpdate(entry.LEntryId, loaded);

        Assert.Equal(before, TEntryTableRead(workspace));
        Assert.Empty(workspace.TWorkspaceRowRead("PRAGMA foreign_key_check;"));
    }

    private static LCardDraft TEntryCardCreate(string meaning, int position)
    {
        return TInterface.TCardDraftCreate(
            string.Empty, string.Empty, meaning, [], [], [], string.Empty, [], [], position);
    }

    private static LExampleDraft TEntryExampleRead(LSentenceDraft sentence)
    {
        return Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample);
    }

    private static IReadOnlyList<string> TEntryTableRead(TWorkspace workspace)
    {
        List<string> rows = [];
        foreach (string query in TEntryDraftQuery)
        {
            List<string> held = [.. workspace.TWorkspaceRowRead(query)];
            held.Sort(StringComparer.Ordinal);
            rows.Add(query);
            rows.AddRange(held);
        }

        return rows;
    }
}
