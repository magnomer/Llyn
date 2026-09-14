using System.Collections.Generic;
using System.IO;
using System.Text;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupImport
{
    private const string TMarkupPair = """
        <llyn>
          <entry>
            <headword>ember</headword>
            <language>English</language>
            <meaning>
              <definition>a glowing coal</definition>
              <translation>
                <headword>braise</headword>
                <language>French</language>
              </translation>
            </meaning>
          </entry>
          <entry>
            <headword>braise</headword>
            <language>French</language>
            <meaning>
              <definition>ember</definition>
              <translation>
                <headword>ember</headword>
                <language>English</language>
              </translation>
            </meaning>
          </entry>
        </llyn>
        """;

    private const string TMarkupLone = """
        <llyn>
          <entry>
            <headword>ember</headword>
            <language>English</language>
            <meaning>
              <definition>a glowing coal</definition>
              <translation>
                <headword>braise</headword>
                <language>French</language>
              </translation>
            </meaning>
          </entry>
        </llyn>
        """;

    [Fact]
    public void MarkupImport_TwoEntriesLinked_StoresBothWithLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, TMarkupPair);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(2));

        Assert.Empty(outcome.LMarkupOutcomeOmission);
        Assert.Equal(2, outcome.LMarkupOutcomeEntry.Count);
        LEntry ember = outcome.LMarkupOutcomeEntry[0];
        LEntry braise = outcome.LMarkupOutcomeEntry[1];

        LEntryDraft emberDraft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        LEntryDraft braiseDraft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(braise.LEntryId));
        LCardDraft emberCard = Assert.Single(emberDraft.LEntryDraftMeanings);
        LCardDraft braiseCard = Assert.Single(braiseDraft.LEntryDraftMeanings);
        Assert.Equal(braise.LEntryId, Assert.Single(emberCard.LCardDraftTranslation));
        Assert.Equal(ember.LEntryId, Assert.Single(braiseCard.LCardDraftTranslation));
        Assert.Equal("a glowing coal", emberCard.LCardDraftMeaning.TStateValueShow());
    }

    [Fact]
    public void MarkupImport_AbsentTranslation_StoresEntryAndReportsOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, TMarkupLone);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1));

        LEntry ember = Assert.Single(outcome.LMarkupOutcomeEntry);
        Assert.Contains("braise", Assert.Single(outcome.LMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Empty(Assert.Single(draft.LEntryDraftMeanings).LCardDraftTranslation);
    }

    [Fact]
    public void MarkupImport_TwinTargets_DropsLinkAndReportsOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("braise", "French", string.Empty, string.Empty, [], []));
        engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("Braise", "french", string.Empty, string.Empty, [], []));

        string path = TMarkupSave(workspace, TMarkupLone);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1));

        LEntry ember = Assert.Single(outcome.LMarkupOutcomeEntry);
        Assert.Contains("braise", Assert.Single(outcome.LMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Empty(Assert.Single(draft.LEntryDraftMeanings).LCardDraftTranslation);
    }

    [Fact]
    public void MarkupImport_MalformedFile_RefusesAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, "<llyn><entry><headword>ember</headword></llyn>");
        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1)));

        Assert.Equal(LRefusal.LRefusalMarkup, refusal.LRefusalReason);
        Assert.Empty(engine.TEngineEntryFind(string.Empty));
    }

    [Fact]
    public void MarkupImport_BlankSecondHeadword_StoresNeither()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
              </entry>
              <entry>
                <headword> </headword>
                <language>English</language>
              </entry>
            </llyn>
            """;
        string path = TMarkupSave(workspace, text);
        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(2)));

        Assert.Equal(LRefusal.LRefusalHeadword, refusal.LRefusalReason);
        Assert.Empty(engine.TEngineEntryFind(string.Empty));
    }

    [Fact]
    public void MarkupImport_ExportedFile_RoundTrips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty,
            [TCardCreate("shine", 1) with { LCardDraftChild = [TCardCreate("shine softly", 1)] }],
            []));
        long senseId = Assert.Single(
            Assert.Single(Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings)
                .LCardDraftChild).LCardDraftId;

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            TInterface.TStateValueCreate("1998"),
            LReferenceKind.LReferenceKindBook,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        engine.TEngineAuthorAttach(reference.LReferenceId, kim.LAuthorId, 0);

        LSentenceDraft sentence = TInterface.TSentenceDraftCreate(
            TInterface.TStateValueCreate("The ember glows."),
            0,
            TInterface.TStateAnchorRead(reference.LReferenceId));
        sentence = sentence with
        {
            LSentenceDraftExample = Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample) with
            {
                LExampleDraftMention = [TInterface.TMentionDraftCreate(0, 10, 5, glow.LEntryId, senseId)],
            },
        };

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, "a note",
            [
                TInterface.TCardDraftCreate(
                    TInterface.TStateValueCreate("glowing coal"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("a small piece of burning coal"),
                    [sentence], [], [glow.LEntryId], ["fire"], [], 1) with
                {
                    LCardDraftChild = [TCardCreate("a fading one", 1)],
                },
                TCardCreate("a remnant", 2),
            ],
            [
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("dying ember"),
                    LStateValue.LStateValueUnspecified,
                    [], [], [], [], [], 1),
            ]));

        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport([ember.LEntryId], path);
        engine.TEngineEntryDelete(ember.LEntryId);

        LMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1));

        Assert.Empty(outcome.LMarkupOutcomeOmission);
        LEntry imported = Assert.Single(outcome.LMarkupOutcomeEntry);
        Assert.NotEqual(ember.LEntryId, imported.LEntryId);
        Assert.Equal("ember", imported.LEntryHeadword);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(imported.LEntryId));
        Assert.Equal("a note", draft.LEntryDraftNote);
        Assert.Equal(2, draft.LEntryDraftMeanings.Count);
        LCardDraft first = draft.LEntryDraftMeanings[0];
        Assert.Equal("glowing coal", first.LCardDraftTitle.TStateValueShow());
        Assert.Equal("a fading one", Assert.Single(first.LCardDraftChild).LCardDraftMeaning.TStateValueShow());
        Assert.Equal(glow.LEntryId, Assert.Single(first.LCardDraftTranslation));
        Assert.Equal("fire", Assert.Single(first.LCardDraftTag).LTagDraftText);
        Assert.Equal(
            "dying ember",
            Assert.Single(draft.LEntryDraftCollocations).LCardDraftExpression.TStateValueShow());

        LExampleDraft example = Assert.IsType<LExampleDraft>(
            Assert.Single(first.LCardDraftSentence).LSentenceDraftExample);
        Assert.Equal("The ember glows.", example.LExampleDraftText.TStateValueShow());
        LMentionDraft mention = Assert.Single(example.LExampleDraftMention);
        Assert.Equal(glow.LEntryId, mention.LMentionDraftEntry);
        Assert.Equal(senseId, mention.LMentionDraftSense);
        Assert.Equal(reference.LReferenceId, example.LExampleDraftReference.LStateAnchorId);
        Assert.Single(engine.TEngineReferenceRead());
    }

    [Fact]
    public void MarkupImport_AppendKnownMeaning_JoinsCardAndKeepsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, string.Empty,
            [TCardCreate("a glowing coal", 1), TCardCreate("a remnant", 2)],
            []));
        long coalId = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId))
            .LEntryDraftMeanings[0].LCardDraftId;

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <meaning>
                  <definition>a glowing coal</definition>
                  <tag>fire</tag>
                </meaning>
                <meaning>
                  <definition>a spark of feeling</definition>
                </meaning>
              </entry>
            </llyn>
            """;
        string path = TMarkupSave(workspace, text);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        Assert.Equal(ember.LEntryId, Assert.Single(outcome.LMarkupOutcomeEntry).LEntryId);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal(3, draft.LEntryDraftMeanings.Count);
        Assert.Equal(coalId, draft.LEntryDraftMeanings[0].LCardDraftId);
        Assert.Equal("fire", Assert.Single(draft.LEntryDraftMeanings[0].LCardDraftTag).LTagDraftText);
        Assert.Equal("a remnant", draft.LEntryDraftMeanings[1].LCardDraftMeaning.TStateValueShow());
        Assert.Equal("a spark of feeling", draft.LEntryDraftMeanings[2].LCardDraftMeaning.TStateValueShow());
        Assert.Equal(3, draft.LEntryDraftMeanings[2].LCardDraftPosition);
        Assert.Empty(outcome.LMarkupOutcomeOmission);
    }

    [Fact]
    public void MarkupImport_AppendKnownIpa_SkipsRowAndJoinsNote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", "ˈɛmbər", "stored note", [], []));

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <pronunciation><ipa>ˈɛmbər</ipa></pronunciation>
                <pronunciation><ipa>ˈɛmbɚ</ipa></pronunciation>
                <note>file note</note>
              </entry>
            </llyn>
            """;
        string path = TMarkupSave(workspace, text);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal(2, draft.LEntryDraftPronunciations.Count);
        Assert.Equal("ˈɛmbər", draft.LEntryDraftPronunciations[0].LPronunciationDraftIpa);
        Assert.Equal("ˈɛmbɚ", draft.LEntryDraftPronunciations[1].LPronunciationDraftIpa);
        Assert.Equal("stored note\n\nfile note", draft.LEntryDraftNote);
    }

    [Fact]
    public void MarkupImport_ReplaceEntry_KeepsIdGraspFavoriteAndRenames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty,
            [TCardCreate("shine", 1), TCardCreate("warmth", 2)],
            []));
        engine.TEngineGraspSave(glow.LEntryId, 3);
        engine.TEngineFavoriteSave(glow.LEntryId);

        string path = TMarkupSave(workspace, TMarkupLone);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        LEntry replaced = Assert.Single(outcome.LMarkupOutcomeEntry);
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
            "glow", "English", string.Empty, string.Empty, [TCardCreate("shine", 1)], []));
        long senseId = Assert.Single(
            Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings).LCardDraftId;

        LSentenceDraft sentence = TInterface.TSentenceDraftCreate(
            TInterface.TStateValueCreate("The ember glows."), 0, LStateAnchor.LStateAnchorUnspecified);
        sentence = sentence with
        {
            LSentenceDraftExample = Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample) with
            {
                LExampleDraftMention = [TInterface.TMentionDraftCreate(0, 10, 5, glow.LEntryId, senseId)],
            },
        };
        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, string.Empty,
            [
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("a glowing coal"),
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
        string path = TMarkupSave(workspace, text);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        Assert.Contains("sense", Assert.Single(outcome.LMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        LExampleDraft example = Assert.IsType<LExampleDraft>(
            Assert.Single(Assert.Single(draft.LEntryDraftMeanings).LCardDraftSentence).LSentenceDraftExample);
        LMentionDraft mention = Assert.Single(example.LExampleDraftMention);
        Assert.Equal(glow.LEntryId, mention.LMentionDraftEntry);
        Assert.Equal(0, mention.LMentionDraftSense);
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
                    TInterface.TStateValueCreate("shine"),
                    [TInterface.TSentenceDraftCreate("The coals glow.")],
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
        string path = TMarkupSave(workspace, text);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

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
            "glow", "English", string.Empty, string.Empty, [TCardCreate("shine", 1)], []));
        engine.TEngineDraftStart("test", glow.LEntryId);

        string path = TMarkupSave(workspace, TMarkupPair);
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

    private static string TMarkupSave(TWorkspace workspace, string text)
    {
        string path = Path.Combine(workspace.TWorkspaceFolder, "import.llx");
        File.WriteAllText(path, text, new UTF8Encoding(false));
        return path;
    }

    private static IReadOnlyList<LMarkupIntake> TMarkupIntakeCreate(int count)
    {
        List<LMarkupIntake> intakes = new(count);
        for (int index = 0; index < count; index++)
        {
            intakes.Add(TInterface.TMarkupIntakeCreate(index, LMarkupMode.LMarkupModeNew));
        }

        return intakes;
    }

    private static LCardDraft TCardCreate(string definition, int position)
    {
        return TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate(definition),
            [], [], [], [], [], position);
    }
}
