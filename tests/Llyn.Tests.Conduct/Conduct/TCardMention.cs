using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardMention
{
    private const string TMentionText = "she knelt to kindle the damp logs";

    [Fact]
    public void SenseSet_SelectionInsideLinkedMention_OffersItsMeaningsAndNarrowsItToTheSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entry, long sense) = TMentionEntryCreate(engine);
        (CDesk desk, CSentence sentence, long sheet, long row) = TMentionPrepare(engine);
        sentence.CSentenceMentionAdd(sheet, row, TMentionText, 13, 6, entry);

        Assert.True(sentence.CSentenceSenseCheck(sheet, row, TMentionText, 13, 6));
        CMentionSense menu = sentence.CSentenceSenseRead(sheet, row, TMentionText, 13, 6)!;
        Assert.Equal("Mention.Sense", menu.CMentionSenseKey);
        Assert.Equal(new CMeaning(0, "Mention.Whole", 0), menu.CMentionSenseRow[0]);
        Assert.Equal([0L, sense], menu.CMentionSenseRow.Select(static meaning => meaning.CMeaningId));
        sentence.CSentenceSenseSet(sheet, row, TMentionText, 13, 6, sense);

        Assert.Equal(sense, Assert.Single(TMentionRead(desk, sheet)).LMentionDraftSense);
        Assert.Equal("set alight", Assert.Single(sentence.CSentenceMentionRead()[row]).CMentionLabelSense);
    }

    [Fact]
    public void MentionCheck_SilentMention_AllowsUnlinkButOffersNoSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (_, CSentence sentence, long sheet, long row) = TMentionPrepare(engine);

        sentence.CSentenceMentionAdd(sheet, row, TMentionText, 13, 6, 0);

        Assert.True(sentence.CSentenceMentionCheck(sheet, row, TMentionText, 13, 6));
        Assert.False(sentence.CSentenceMentionCheck(sheet, row, TMentionText, 0, 3));
        Assert.False(sentence.CSentenceSenseCheck(sheet, row, TMentionText, 13, 6));
        Assert.Null(sentence.CSentenceSenseRead(sheet, row, TMentionText, 13, 6));
    }

    [Fact]
    public void MentionRemove_BySelectionAndByChip_DropsEachMention()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entry, _) = TMentionEntryCreate(engine);
        (CDesk desk, CSentence sentence, long sheet, long row) = TMentionPrepare(engine);
        sentence.CSentenceMentionAdd(sheet, row, TMentionText, 13, 6, entry);
        sentence.CSentenceMentionAdd(sheet, row, TMentionText, 29, 4, 0);

        sentence.CSentenceMentionRemove(sheet, row, TMentionText, 13, 6);
        long silent = Assert.Single(TMentionRead(desk, sheet)).LMentionDraftId;
        sentence.CSentenceMentionRemove(sheet, row, silent);

        Assert.Empty(TMentionRead(desk, sheet));
    }

    [Fact]
    public void MentionRead_LinkedAndSilentMentions_NamesTheHeadwordAndKeysTheSilentChip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entry, _) = TMentionEntryCreate(engine);
        (_, CSentence sentence, long sheet, long row) = TMentionPrepare(engine);
        sentence.CSentenceMentionAdd(sheet, row, TMentionText, 13, 6, entry);
        sentence.CSentenceMentionAdd(sheet, row, TMentionText, 29, 4, 0);

        IReadOnlyList<CMentionLabel> labels = sentence.CSentenceMentionRead()[row];

        Assert.Equal(2, labels.Count);
        Assert.Equal("kindle", labels[0].CMentionLabelWord);
        Assert.Equal("kindle", labels[0].CMentionLabelName);
        Assert.Null(labels[0].CMentionLabelKey);
        Assert.Equal("logs", labels[1].CMentionLabelWord);
        Assert.Empty(labels[1].CMentionLabelName);
        Assert.Empty(labels[1].CMentionLabelSense);
        Assert.Equal("Mention.Silent", labels[1].CMentionLabelKey);
    }

    [Fact]
    public void MentionRead_EngineFails_ShowsTheFindFailureAndKeysEverySentenceEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        (CDesk desk, _, _, long row) = TMentionPrepare(engine);
        CSentence failing = TInterfaceMention.TSentenceFailCreate(
            engine, desk, TEnvoyFake.TEnvoyCreate(false, asked));
        LEntryDraft content = desk.TDeskRead()!.LDraftContent;
        long[] sentences = content.LEntryDraftMeanings.Concat(content.LEntryDraftCollocations)
            .SelectMany(static card => card.LCardDraftSentence)
            .Select(static sentence => sentence.LSentenceDraftId)
            .Order()
            .ToArray();

        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines = failing.CSentenceMentionRead();

        Assert.Equal(["Mention.FindFailed"], asked);
        Assert.Contains(row, sentences);
        Assert.Equal(sentences, lines.Keys.Order());
        Assert.All(lines.Values, static labels => Assert.Empty(labels));
    }

    [Fact]
    public void SentenceMentionRead_UnknownRows_KeysOnlyHeldSentences()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        (CDesk desk, _, _, long row) = TMentionPrepare(engine);
        IReadOnlyList<long> sentences = desk.TDeskSentenceRead();
        LMentionLabel kindle = TInterfaceMention.TMentionLabelCreate(7, "kindle", 3);
        Dictionary<long, IReadOnlyList<LMentionLabel>> lines = new() { [row] = [kindle, kindle] };
        foreach (long stray in new[] { 0L, -1L, long.MaxValue, long.MinValue }.Except(sentences))
        {
            lines[stray] = [kindle];
        }

        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> read = TInterfaceMention.TSentenceLineCreate(
            engine, desk, TEnvoyFake.TEnvoyCreate(false, asked), lines).CSentenceMentionRead();

        Assert.Empty(asked);
        Assert.Contains(row, sentences);
        Assert.Equal(sentences, read.Keys.Order());
        Assert.Equal(["kindle", "kindle"], read[row].Select(static label => label.CMentionLabelWord));
        Assert.All(sentences.Where(id => id != row), id => Assert.Empty(read[id]));
    }

    [Fact]
    public void SentenceMentionRead_NullLines_ShowsTheFindFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        (CDesk desk, _, _, _) = TMentionPrepare(engine);
        IReadOnlyList<long> sentences = desk.TDeskSentenceRead();

        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> read = TInterfaceMention.TSentenceLineCreate(
            engine, desk, TEnvoyFake.TEnvoyCreate(false, asked), null).CSentenceMentionRead();

        Assert.Equal(["Mention.FindFailed"], asked);
        Assert.NotEmpty(sentences);
        Assert.Equal(sentences, read.Keys.Order());
        Assert.All(read.Values, static labels => Assert.Empty(labels));
    }

    [Fact]
    public void SentenceMentionRead_NoDraftHeld_AnswersNoEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        Dictionary<long, IReadOnlyList<LMentionLabel>> lines = new()
        {
            [1] = [TInterfaceMention.TMentionLabelCreate(7, "kindle", 0)],
        };

        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> read = TInterfaceMention.TSentenceLineCreate(
            engine, desk, TEnvoyFake.TEnvoyCreate(false, asked), lines).CSentenceMentionRead();

        Assert.Empty(read);
        Assert.Empty(asked);
    }

    private static (CDesk TMentionDesk, CSentence TMentionGate, long TMentionSheet, long TMentionRow)
        TMentionPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        CSentence sentence = TInterfaceConductCard.TSentenceCreate(engine, desk);
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long sheet = desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        desk.TDeskDefer(TInterface.TSentenceAdditionCreate(desk.CDeskId, sheet, 0));
        long row = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence[0].LSentenceDraftId;
        sentence.CSentenceTextSet(sheet, row, TMentionText);
        return (desk, sentence, sheet, row);
    }

    private static IReadOnlyList<LMentionDraft> TMentionRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence[0].LSentenceDraftExample!.LExampleDraftMention;
    }

    internal static (long TMentionEntry, long TMentionSense) TMentionEntryCreate(LEngine engine)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    TInterfaceState.TStateValueCreate("set alight"),
                    LStateValue.LStateValueUnspecified,
                    TInterfaceState.TStateValueCreate("to set something burning"),
                    [],
                    [],
                    [],
                    [],
                    [],
                    1,
                    TInterface.TIdentityCreate()),
            ],
            []));

        return (
            entry.LEntryId,
            Assert.Single(engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)).LMeaningId);
    }
}
