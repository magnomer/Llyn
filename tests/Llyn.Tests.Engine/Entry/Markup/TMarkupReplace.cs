using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupReplace
{
    [Fact]
    public void MarkupImport_ReplaceEntry_KeepsIdGraspFavoriteAndRenames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty,
            [TInterface.TCardCreate("shine", 1), TInterface.TCardCreate("warmth", 2)],
            []));
        engine.TEngineGraspSave(glow.LEntryId, 3);
        engine.TEngineFavoriteSave(glow.LEntryId);

        string path = TInterface.TMarkupSave(workspace, TInterface.TMarkupLone);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        LEntry replaced = Assert.Single(outcome.TMarkupOutcomeEntry);
        Assert.Equal(glow.LEntryId, replaced.LEntryId);
        Assert.Equal("ember", replaced.LEntryHeadword);
        Assert.Equal(3, engine.TEngineGraspRead(glow.LEntryId));
        Assert.True(engine.TEngineFavoriteCheck(glow.LEntryId));
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId));
        Assert.Equal("a glowing coal", Assert.Single(draft.LEntryDraftMeanings).LCardDraftMeaning.TStateValueShow());
        Assert.Single(engine.TEngineEntryFind(string.Empty));
    }

    [Fact]
    public void MarkupImport_ReplaceCitedSense_ClearsMentionSenseAndReportsOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty, [TInterface.TCardCreate("shine", 1)], []));
        long senseId = Assert.Single(
            Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings).LCardDraftId;

        LSentenceDraft sentence = TInterfaceExample.TSentenceDraftCreate(
            TInterfaceState.TStateValueCreate("The ember glows."), 0, LStateAnchor.LStateAnchorUnspecified);
        sentence = sentence with
        {
            LSentenceDraftExample = Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample) with
            {
                LExampleDraftMention = [TInterfaceMentionSpan.TMentionDraftCreate(0, 10, 5, glow.LEntryId, senseId)],
            },
        };
        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, string.Empty,
            [
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    TInterfaceState.TStateValueCreate("a glowing coal"),
                    [sentence], [], [], [], [], 1),
            ],
            []));

        const string text = """
            <llyn>
              <entry>
                <headword>glow</headword>
                <language>English</language>
                <meaning><definition>give out light</definition></meaning>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        Assert.Contains("sense", Assert.Single(outcome.TMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        LExampleDraft example = Assert.IsType<LExampleDraft>(
            Assert.Single(Assert.Single(draft.LEntryDraftMeanings).LCardDraftSentence).LSentenceDraftExample);
        LMentionDraft mention = Assert.Single(example.LExampleDraftMention);
        Assert.Equal(glow.LEntryId, mention.LMentionDraftEntry);
        Assert.Equal(0, mention.LMentionDraftSense);
    }

    [Fact]
    public void MarkupImport_ReplaceCitedSense_ReportsLineAndText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty, [TInterface.TCardCreate("shine", 1)], []));
        long senseId = Assert.Single(
            Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings).LCardDraftId;

        LSentenceDraft sentence = TInterfaceExample.TSentenceDraftCreate(
            TInterfaceState.TStateValueCreate("The ember glows."), 0, LStateAnchor.LStateAnchorUnspecified);
        sentence = sentence with
        {
            LSentenceDraftExample = Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample) with
            {
                LExampleDraftMention = [TInterfaceMentionSpan.TMentionDraftCreate(0, 10, 5, glow.LEntryId, senseId)],
            },
        };
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, string.Empty,
            [
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    TInterfaceState.TStateValueCreate("a glowing coal"),
                    [sentence], [], [], [], [], 1),
            ],
            []));

        const string text = """
            <llyn>
              <entry>
                <headword>glow</headword>
                <language>English</language>
                <meaning><definition>give out light</definition></meaning>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        LMarkupOmission omission = Assert.Single(outcome.TMarkupOutcomeOmission);
        Assert.Equal(
            (2, "sense of example \"The ember glows.\""),
            (omission.LMarkupOmissionLine, omission.LMarkupOmissionText));
    }

    [Fact]
    public void MarkupImport_ReplaceEntry_KeepsSituationAndDetachedExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty,
            [
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    TInterfaceState.TStateValueCreate("shine"),
                    [TInterfaceExample.TSentenceDraftCreate("The coals glow.")],
                    [TInterface.TSituationDraftCreate("by the fire")],
                    [], [], [], 1),
            ],
            []));
        LEntryDraft before = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId));
        LCardDraft card = Assert.Single(before.LEntryDraftMeanings);
        long exampleId = Assert.IsType<LExampleDraft>(Assert.Single(card.LCardDraftSentence).LSentenceDraftExample)
            .LExampleDraftId;
        long situationId = Assert.Single(card.LCardDraftSituation).LSituationDraftId;

        const string text = """
            <llyn>
              <entry>
                <headword>glow</headword>
                <language>English</language>
                <meaning><definition>give out light</definition></meaning>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        LEntryDraft after = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId));
        Assert.Empty(Assert.Single(after.LEntryDraftMeanings).LCardDraftSentence);
        Assert.NotNull(engine.TEngineSituationRead(situationId));
        LExample example = Assert.IsType<LExample>(engine.TEngineExampleRead(exampleId));
        Assert.Equal("The coals glow.", example.LExampleText.TStateValueShow());
    }

    [Fact]
    public void MarkupImport_HeldDraftOnTarget_RefusesWholeFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty, [TInterface.TCardCreate("shine", 1)], []));
        engine.TEngineDraftStart("test", glow.LEntryId);

        string path = TInterface.TMarkupSave(workspace, TInterface.TMarkupPair);
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [
                TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew),
                TInterface.TMarkupIntakeCreate(1, LMarkupMode.LMarkupModeReplace, glow.LEntryId),
            ]));

        Assert.Equal(LRefusal.LRefusalStale, refusal.LRefusalReason);
        LEntry kept = Assert.Single(engine.TEngineEntryFind(string.Empty));
        Assert.Equal("glow", kept.LEntryHeadword);
        Assert.Equal("shine", Assert.Single(
            Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings)
            .LCardDraftMeaning.TStateValueShow());
    }
}
