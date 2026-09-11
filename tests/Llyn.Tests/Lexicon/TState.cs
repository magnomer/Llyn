using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TState
{
    [Fact]
    public void EntrySave_UnreadableAndEmptyFields_StoresEachState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnknown,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [TInterface.TSentenceDraftCreate(
                    LStateValue.LStateValueUnknown, 0, LStateAnchor.LStateAnchorUnspecified)],
                [TInterface.TSituationDraftCreate(LStateValue.LStateValueUnknown, 0)],
                [],
                string.Empty,
                ["spoken"], [], 1)],
            []));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sense WHERE title_state = 'unknown' AND title IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE text_state = 'unknown' AND text IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM situation WHERE title_state = 'unknown' AND title IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);

        Assert.Equal(LStateValue.LStateValueUnknown, card.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, card.LCardDraftExpression);
        Assert.Equal(
            LStateValue.LStateValueUnknown,
            TExampleDraftRead(Assert.Single(card.LCardDraftSentence)).LExampleDraftText);
        Assert.Equal(
            LStateValue.LStateValueUnknown,
            Assert.Single(card.LCardDraftSituation).LSituationDraftTitle);
        Assert.Equal(["spoken"], card.LCardDraftTag);
    }

    [Fact]
    public void EntrySave_FieldNeverWritten_StoresNoRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [
                    TInterface.TSentenceDraftCreate("he said a word"),
                    TInterface.TSentenceDraftCreate(
                        LStateValue.LStateValueUnknown, 0, LStateAnchor.LStateAnchorUnspecified),
                    TInterface.TSentenceDraftCreate("   "),
                ],
                [],
                [],
                string.Empty,
                [], [], 1)],
            []));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void EntrySave_UnreadableCitation_DiffersFromNoSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [
                    TInterface.TSentenceDraftCreate(
                        TInterface.TStateValueCreate("he said a word"),
                        0,
                        LStateAnchor.LStateAnchorUnknown),
                    TInterface.TSentenceDraftCreate("not a word was spoken"),
                ],
                [],
                [],
                string.Empty,
                [], [], 1)],
            []));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE source_state = 'unknown' AND source_id IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE source_state = 'unspecified' AND source_id IS NULL;"));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);

        Assert.Equal(
LStateAnchor.LStateAnchorUnknown,
            TExampleDraftRead(card.LCardDraftSentence[0]).LExampleDraftReference);
        Assert.Equal(
LStateAnchor.LStateAnchorUnspecified,
            TExampleDraftRead(card.LCardDraftSentence[1]).LExampleDraftReference);
    }

    [Fact]
    public void EntryUpdate_ClearedUnreadableField_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnknown,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnknown,
                [],
                [TInterface.TSituationDraftCreate(LStateValue.LStateValueUnknown, 0)],
                [],
                string.Empty,
                [], [], 1)],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        long situationId = card.LCardDraftSituation[0].LSituationDraftId;

        engine.TEngineEntryUpdate(stored.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftTitle = TInterface.TStateValueCreate("the plain meaning"),
                    LCardDraftMeaning = LStateValue.LStateValueUnspecified,
                    LCardDraftSituation =
                    [
                        card.LCardDraftSituation[0] with
                        {
                            LSituationDraftTitle = TInterface.TStateValueCreate("in court"),
                        },
                    ],
                },
            ],
        });

        LEntryDraft second = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft written = Assert.Single(second.LEntryDraftMeanings);

        Assert.Equal(TInterface.TStateValueCreate("the plain meaning"), written.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, written.LCardDraftMeaning);

        LSituationDraft situation = Assert.Single(written.LCardDraftSituation);
        Assert.Equal(situationId, situation.LSituationDraftId);
        Assert.Equal(TInterface.TStateValueCreate("in court"), situation.LSituationDraftTitle);
    }

    [Theory]
    [InlineData("a unit of language", false)]
    [InlineData("a unit of language", true)]
    [InlineData("", false)]
    [InlineData("", true)]
    [InlineData("   ", false)]
    [InlineData("   ", true)]
    [InlineData(null, false)]
    [InlineData(null, true)]
    public void StateValueResolve_TextAndMark_MapsOneStateEach(string? text, bool unreadable)
    {
        LStateValue resolved = TInterface.TStateValueResolve(text, unreadable);

        if (unreadable)
        {
            Assert.Equal(LStateValue.LStateValueUnknown, resolved);
            Assert.Null(resolved.LStateValueText);
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            Assert.Equal(LStateValue.LStateValueUnspecified, resolved);
            return;
        }

        Assert.Equal(TInterface.TStateValueCreate(text), resolved);
    }

    [Fact]
    public void StateValueResolve_WhitespaceRoundTrip_ReportsNoChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineDraftSave(started with
        {
            LDraftContent = TInterface.TEntryDraftCreate(
                "word",
                "English",
                string.Empty,
                string.Empty,
                [TInterface.TCardDraftCreate(
                    TInterface.TStateValueResolve(string.Empty, true),
                    TInterface.TStateValueResolve("   ", false),
                    TInterface.TStateValueResolve("a unit of language", false),
                    [],
                    [],
                    [],
                    string.Empty,
                    [],
                    [],
                    0)],
                []),
        });

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        LDraft opened = engine.TEngineDraftStart("editor", stored.LEntryId);
        LCardDraft card = Assert.Single(opened.LDraftContent.LEntryDraftMeanings);

        Assert.Equal(LStateValue.LStateValueUnknown, card.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, card.LCardDraftExpression);
        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));

        engine.TEngineDraftSave(opened with
        {
            LDraftContent = opened.LDraftContent with
            {
                LEntryDraftMeanings =
                [
                    card with
                    {
                        LCardDraftTitle = TInterface.TStateValueResolve(
                            card.LCardDraftTitle.TStateValueShow(),
                            card.LCardDraftTitle.LStateValueState == LState.LStateUnknown),
                        LCardDraftExpression = TInterface.TStateValueResolve(
                            card.LCardDraftExpression.TStateValueShow(),
                            card.LCardDraftExpression.LStateValueState == LState.LStateUnknown),
                        LCardDraftMeaning = TInterface.TStateValueResolve(
                            card.LCardDraftMeaning.TStateValueShow(),
                            card.LCardDraftMeaning.LStateValueState == LState.LStateUnknown),
                    },
                ],
            },
        });

        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));
    }

    private static LExampleDraft TExampleDraftRead(LSentenceDraft sentence)
    {
        return Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample);
    }
}
