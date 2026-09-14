using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupExport
{
    [Fact]
    public void MarkupExport_LinkedEntry_WritesNamesNotIds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty,
            [
                TCardCreate("shine", 1) with
                {
                    LCardDraftChild = [TCardCreate("shine softly", 1)],
                },
            ],
            []));
        long senseId = Assert.Single(
            Assert.Single(Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings)
                .LCardDraftChild).LCardDraftId;

        LEntry braise = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "braise", "French", string.Empty, string.Empty, [TCardCreate("ember", 1)], []));

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            TInterface.TStateValueCreate("1998"),
            LReferenceKind.LReferenceKindBook,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor lee = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));
        engine.TEngineAuthorAttach(reference.LReferenceId, kim.LAuthorId, 0);
        engine.TEngineAuthorAttach(reference.LReferenceId, lee.LAuthorId, 1);

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
            "ember", "English", string.Empty, string.Empty,
            [
                TInterface.TCardDraftCreate(
                    TInterface.TStateValueCreate("glowing coal"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("a small piece of burning coal"),
                    [sentence], [], [braise.LEntryId], [], [], 1) with
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
        string text = File.ReadAllText(path);

        LMarkupEntry entry = Assert.Single(TInterface.TMarkupParse(text, out IReadOnlyList<LMarkupOmission> omissions));
        Assert.Empty(omissions);
        Assert.DoesNotContain("id", text);

        Assert.Equal("ember", entry.LMarkupEntryHeadword);
        Assert.Equal(2, entry.LMarkupEntryMeaning.Count);
        Assert.Single(entry.LMarkupEntryMeaning[0].LMarkupCardChild);
        Assert.Equal(
            "dying ember",
            Assert.Single(entry.LMarkupEntryCollocation).LMarkupCardExpression.TStateValueShow());

        LMarkupTranslation translation = Assert.Single(entry.LMarkupEntryMeaning[0].LMarkupCardTranslation);
        Assert.Equal("braise", translation.LMarkupTranslationHeadword);
        Assert.Equal("French", translation.LMarkupTranslationLanguage);

        LMarkupExample example = Assert.IsType<LMarkupExample>(
            Assert.Single(entry.LMarkupEntryMeaning[0].LMarkupCardSentence).LMarkupSentenceExample);
        LMarkupMention mention = Assert.Single(example.LMarkupExampleMention);
        Assert.Equal("glow", mention.LMarkupMentionHeadword);
        Assert.Equal("English", mention.LMarkupMentionLanguage);
        Assert.Equal("1.1", mention.LMarkupMentionSense);

        LMarkupReference cited = Assert.IsType<LMarkupReference>(example.LMarkupExampleReference);
        Assert.Equal("A Dictionary", cited.LMarkupReferenceTitle.TStateValueShow());
        Assert.Equal(["Kim", "Lee"], cited.LMarkupReferenceAuthor);
    }

    [Fact]
    public void MarkupExport_UnknownTitle_WritesStateAttribute()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ash", "English", string.Empty, string.Empty,
            [
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnknown,
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("what a fire leaves"),
                    [], [], [], [], [], 1),
            ],
            []));

        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport([entry.LEntryId], path);
        string text = File.ReadAllText(path);

        Assert.Contains("state=\"unknown\"", text);
        LMarkupEntry read = Assert.Single(TInterface.TMarkupParse(text));
        Assert.Equal(LState.LStateUnknown, Assert.Single(read.LMarkupEntryMeaning).LMarkupCardTitle.LStateValueState);
    }

    [Fact]
    public void MarkupExport_MissingEntry_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineMarkupExport([99], path));

        Assert.Equal(LRefusal.LRefusalEntry, refusal.LRefusalReason);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task MarkupExport_PortraitMarkupFormat_WritesSameText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "soot", "English", string.Empty, "a note",
            [TCardCreate("black powder", 1)],
            []));

        string direct = Path.Combine(workspace.TWorkspaceFolder, "direct.llx");
        string portrait = Path.Combine(workspace.TWorkspaceFolder, "portrait.llx");
        engine.TEngineMarkupExport([entry.LEntryId], direct);
        await engine.TEnginePortraitExport(
            entry.LEntryId,
            portrait,
            LPortraitFormat.LPortraitFormatMarkup,
            TInterface.TPortraitLabelCreate(
                "?", "Meaning", "Meanings", "Collocation", "Collocations", "Incoming", "Note"));

        Assert.Equal(File.ReadAllText(direct), File.ReadAllText(portrait));
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
