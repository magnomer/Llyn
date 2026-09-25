using System.Collections.Generic;
using System.IO;
using System.Text;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupLink
{
    private const string TMarkupOwn = """
        <llyn>
          <entry>
            <headword>glow</headword>
            <language>English</language>
            <meaning>
              <definition>give out light</definition>
              <sentence>
                <example>
                  <text>Embers glow.</text>
                  <mention>
                    <offset>7</offset>
                    <length>4</length>
                    <headword>glow</headword>
                    <language>English</language>
                    <sense>1</sense>
                  </mention>
                </example>
              </sentence>
            </meaning>
          </entry>
        </llyn>
        """;

    private const string TMarkupCross = """
        <llyn>
          <entry>
            <headword>ember</headword>
            <language>English</language>
            <meaning>
              <definition>a glowing coal</definition>
              <sentence>
                <example>
                  <text>Embers glow.</text>
                  <mention>
                    <offset>7</offset>
                    <length>4</length>
                    <headword>glow</headword>
                    <language>English</language>
                    <sense>1.1</sense>
                  </mention>
                </example>
              </sentence>
            </meaning>
          </entry>
          <entry>
            <headword>glow</headword>
            <language>English</language>
            <meaning>
              <definition>shine</definition>
              <meaning>
                <definition>shine softly</definition>
                <sentence>
                  <example>
                    <text>An ember glows.</text>
                    <mention>
                      <offset>3</offset>
                      <length>5</length>
                      <headword>ember</headword>
                      <language>English</language>
                      <sense>1</sense>
                    </mention>
                  </example>
                </sentence>
              </meaning>
            </meaning>
          </entry>
        </llyn>
        """;

    private const string TMarkupBare = """
        <llyn>
          <entry>
            <headword>glow</headword>
            <language>English</language>
            <meaning>
              <definition>give out light</definition>
              <sentence>
                <example>
                  <text>Embers glow.</text>
                  <mention>
                    <offset>7</offset>
                    <length>4</length>
                  </mention>
                </example>
              </sentence>
            </meaning>
          </entry>
        </llyn>
        """;

    [Fact]
    public void MarkupImport_ReplaceOwnSense_PointsMentionAtNewSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty, [TInterface.TCardCreate("shine", 1)], []));
        long oldId = Assert.Single(
            Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings).LCardDraftId;

        string path = TMarkupSave(workspace, TMarkupOwn);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        Assert.Empty(outcome.TMarkupOutcomeOmission);
        LCardDraft card = Assert.Single(
            Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings);
        Assert.NotEqual(oldId, card.LCardDraftId);
        LMentionDraft mention = Assert.Single(TExampleRead(card).LExampleDraftMention);
        Assert.Equal(glow.LEntryId, mention.LMentionDraftEntry);
        Assert.Equal(card.LCardDraftId, mention.LMentionDraftSense);
    }

    [Fact]
    public void MarkupImport_TwoNewEntriesCitingEachOther_LinksBothSenses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, TMarkupCross);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [
                TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew),
                TInterface.TMarkupIntakeCreate(1, LMarkupMode.LMarkupModeNew),
            ]);

        Assert.Empty(outcome.TMarkupOutcomeOmission);
        LEntryDraft ember = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(outcome.TMarkupOutcomeEntry[0].LEntryId));
        LEntryDraft glow = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(outcome.TMarkupOutcomeEntry[1].LEntryId));
        LCardDraft soft = Assert.Single(Assert.Single(glow.LEntryDraftMeanings).LCardDraftChild);

        LMentionDraft toGlow = Assert.Single(
            TExampleRead(Assert.Single(ember.LEntryDraftMeanings)).LExampleDraftMention);
        Assert.Equal(soft.LCardDraftId, toGlow.LMentionDraftSense);

        LMentionDraft toEmber = Assert.Single(TExampleRead(soft).LExampleDraftMention);
        Assert.Equal(Assert.Single(ember.LEntryDraftMeanings).LCardDraftId, toEmber.LMentionDraftSense);
    }

    [Fact]
    public void MarkupImport_SenseBeyondTree_KeepsEntryAndReportsLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, TMarkupOwn.Replace("<sense>1</sense>", "<sense>4</sense>"));
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        LMarkupOmission omission = Assert.Single(outcome.TMarkupOutcomeOmission);
        Assert.Contains("sense \"4\"", omission.LMarkupOmissionText);
        Assert.Equal(2, omission.LMarkupOmissionLine);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        LMentionDraft mention = Assert.Single(
            TExampleRead(Assert.Single(draft.LEntryDraftMeanings)).LExampleDraftMention);
        Assert.Equal(0, mention.LMentionDraftSense);
    }

    [Fact]
    public void MarkupImport_MentionPastText_DropsMentionAndReports()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, TMarkupOwn.Replace("<length>4</length>", "<length>40</length>"));
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Contains("mention at 7", Assert.Single(outcome.TMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        Assert.Empty(TExampleRead(Assert.Single(draft.LEntryDraftMeanings)).LExampleDraftMention);
    }

    [Fact]
    public void MarkupImport_MentionOffsetOverflow_DropsMention()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, TMarkupBare.Replace("<offset>7</offset>", "<offset>2147483647</offset>"));
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Contains("mention at 2147483647", Assert.Single(outcome.TMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        Assert.Empty(TExampleRead(Assert.Single(draft.LEntryDraftMeanings)).LExampleDraftMention);
    }

    [Fact]
    public void MarkupImport_UncLocations_DropsRowsAndBlanksAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        const string text = """
            <llyn>
              <entry>
                <headword>glow</headword>
                <language>English</language>
                <pronunciation><ipa>ɡloʊ</ipa><audio>\\host\share\glow.mp3</audio></pronunciation>
                <meaning>
                  <definition>give out light</definition>
                  <image><location>\\host\share\glow.png</location></image>
                  <image><location>https://example.test/glow.png</location></image>
                  <video><location>file://host/share/glow.mp4</location></video>
                </meaning>
              </entry>
            </llyn>
            """;
        string path = TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Equal(3, outcome.TMarkupOutcomeOmission.Count);
        Assert.All(
            outcome.TMarkupOutcomeOmission,
            omission => Assert.StartsWith("location", omission.LMarkupOmissionText));
        LEntryDraft draft = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        Assert.Equal(string.Empty, Assert.Single(draft.LEntryDraftPronunciations).LPronunciationDraftAudio);
        LCardDraft card = Assert.Single(draft.LEntryDraftMeanings);
        Assert.Equal(
            "https://example.test/glow.png",
            Assert.Single(card.LCardDraftImage).LImageDraftLocation.TStateValueShow());
        Assert.Empty(card.LCardDraftVideo);
    }

    [Fact]
    public void MarkupImport_MentionWithoutHeadword_KeepsSpanAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TMarkupSave(workspace, TMarkupBare);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Empty(outcome.TMarkupOutcomeOmission);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        LMentionDraft mention = Assert.Single(
            TExampleRead(Assert.Single(draft.LEntryDraftMeanings)).LExampleDraftMention);
        Assert.Equal(0, mention.LMentionDraftEntry);
        Assert.Equal(7, mention.LMentionDraftOffset);

        string exported = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport([outcome.TMarkupOutcomeEntry[0].LEntryId], exported);
        LMarkupCard card = Assert.Single(
            Assert.Single(engine.TEngineMarkupRead(exported).LMarkupCargoEntry).LMarkupEntryMeaning);
        LMarkupMention written = Assert.Single(TExampleRead(card).LMarkupExampleMention);
        Assert.Equal(string.Empty, written.LMarkupMentionHeadword);
    }

    [Fact]
    public void MarkupImport_ReplaceWithoutLanguage_KeepsStoredLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty, [TInterface.TCardCreate("shine", 1)], []));

        string path = TMarkupSave(workspace, TMarkupOwn.Replace("<language>English</language>", string.Empty));
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId)]);

        Assert.Equal("English", Assert.Single(engine.TEngineEntryFind(string.Empty)).LEntryLanguage);
    }

    [Fact]
    public void MarkupImport_ReferenceWithoutTitle_CreatesOwnReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference blank = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindOther,
            TInterface.TStateValueCreate("a note"),
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));

        string text = TMarkupOwn.Replace(
            "</example>",
            "<reference><kind>web</kind></reference></example>");
        string path = TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        long? cited = TExampleRead(Assert.Single(draft.LEntryDraftMeanings)).LExampleDraftReference.LStateAnchorId;
        Assert.NotNull(cited);
        Assert.NotEqual(blank.LReferenceId, cited);
    }

    private static LExampleDraft TExampleRead(LCardDraft card)
    {
        return Assert.IsType<LExampleDraft>(Assert.Single(card.LCardDraftSentence).LSentenceDraftExample);
    }

    private static LMarkupExample TExampleRead(LMarkupCard card)
    {
        return Assert.IsType<LMarkupExample>(Assert.Single(card.LMarkupCardSentence).LMarkupSentenceExample);
    }

    private static string TMarkupSave(TWorkspace workspace, string text)
    {
        string path = Path.Combine(workspace.TWorkspaceFolder, "import.llx");
        File.WriteAllText(path, text, new UTF8Encoding(false));
        return path;
    }
}
