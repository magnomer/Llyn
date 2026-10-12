using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupMerge
{
    [Fact]
    public void MarkupImport_AppendKnownMeaning_JoinsCardAndKeepsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, string.Empty,
            [TInterface.TCardCreate("a glowing coal", 1), TInterface.TCardCreate("a remnant", 2)],
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
        string path = TInterface.TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        Assert.Equal(ember.LEntryId, Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal(3, draft.LEntryDraftMeanings.Count);
        Assert.Equal(coalId, draft.LEntryDraftMeanings[0].LCardDraftId);
        Assert.Equal("fire", Assert.Single(draft.LEntryDraftMeanings[0].LCardDraftTag).LTagDraftText);
        Assert.Equal("a remnant", draft.LEntryDraftMeanings[1].LCardDraftMeaning.TStateValueShow());
        Assert.Equal("a spark of feeling", draft.LEntryDraftMeanings[2].LCardDraftMeaning.TStateValueShow());
        Assert.Equal(3, draft.LEntryDraftMeanings[2].LCardDraftPosition);
        Assert.Empty(outcome.TMarkupOutcomeOmission);
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
        string path = TInterface.TMarkupSave(workspace, text);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal(2, draft.LEntryDraftPronunciations.Count);
        Assert.Equal("ˈɛmbər", draft.LEntryDraftPronunciations[0].LPronunciationDraftIpa);
        Assert.Equal("ˈɛmbɚ", draft.LEntryDraftPronunciations[1].LPronunciationDraftIpa);
        Assert.Equal("stored note\n\nfile note", draft.LEntryDraftNote);
    }

    [Fact]
    public void MarkupImport_AppendKnownScheme_KeepsStoredRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, string.Empty, [], []) with
        {
            LEntryDraftTranscriptions = [TInterface.TTranscriptionDraftCreate("Respelling", "EM-ber")],
        });

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <transcription><scheme>Respelling</scheme><text>EM-bur</text></transcription>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        Assert.Equal(ember.LEntryId, Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal("EM-ber", Assert.Single(draft.LEntryDraftTranscriptions).LTranscriptionDraftText);
    }

    [Fact]
    public void MarkupImport_AppendUnknownRegister_KeepsOneRegister()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <meaning>
                  <definition>a glowing coal</definition>
                  <register state="unknown"/>
                </meaning>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        LEntry ember = Assert.Single(engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]).TMarkupOutcomeEntry);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        LCardDraft card = Assert.Single(draft.LEntryDraftMeanings);
        Assert.True(Assert.Single(card.LCardDraftRegister).LRegisterDraftName.LStateValueUncertain);
    }

    [Fact]
    public void MarkupImport_AppendKnownNote_KeepsNoteOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, "stored note", [], []));

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <note>file note</note>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        for (int round = 0; round < 2; round++)
        {
            engine.TEngineMarkupImport(
                engine.TEngineMarkupRead(path),
                [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);
        }

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal("stored note\n\nfile note", draft.LEntryDraftNote);
    }

    [Fact]
    public void MarkupImport_AppendSpacedNote_KeepsStoredNote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, "first line\nsecond line", [], []));

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <note>first line   &#13;
            second line</note>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal("first line\nsecond line", draft.LEntryDraftNote);
    }

    [Fact]
    public void MarkupImport_AppendAudioOnly_KeepsOnePronunciation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, "ember.mp3");
        File.WriteAllBytes(file, [0]);

        string text = $"""
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <pronunciation><audio>{file}</audio></pronunciation>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        LEntry ember = Assert.Single(engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]).TMarkupOutcomeEntry);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal(file, Assert.Single(draft.LEntryDraftPronunciations).LPronunciationDraftAudio);
    }

    [Fact]
    public void MarkupImport_AppendUnknownImage_KeepsOneImage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
                <meaning>
                  <definition>a glowing coal</definition>
                  <image><location state="unknown"/></image>
                </meaning>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        LEntry ember = Assert.Single(engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]).TMarkupOutcomeEntry);
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, ember.LEntryId)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        LCardDraft card = Assert.Single(draft.LEntryDraftMeanings);
        Assert.True(Assert.Single(card.LCardDraftImage).LImageDraftLocation.LStateValueUncertain);
    }
}
