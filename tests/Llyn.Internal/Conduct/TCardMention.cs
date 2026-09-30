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
        Assert.Equal(sense, Assert.Single(sentence.CSentenceSenseRead(sheet, row, TMentionText, 13, 6)!).CMeaningId);
        sentence.CSentenceSenseSet(sheet, row, TMentionText, 13, 6, sense);

        Assert.Equal(sense, Assert.Single(TMentionRead(desk, sheet)).LMentionDraftSense);
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
        Assert.Equal("Mention.Silent", labels[1].CMentionLabelKey);
    }

    [Fact]
    public void MentionRead_EngineFails_ShowsTheFindFailureAndAnswersNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        (CDesk desk, _, _, _) = TMentionPrepare(engine);
        CSentence failing = TInterfaceMention.TSentenceFailCreate(
            engine, desk, TEnvoyFake.TEnvoyCreate(false, asked));

        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines = failing.CSentenceMentionRead();

        Assert.Empty(lines);
        Assert.Equal(["Mention.FindFailed"], asked);
    }

    private static (CDesk TMentionDesk, CSentence TMentionGate, long TMentionSheet, long TMentionRow)
        TMentionPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        CSentence sentence = TInterfaceConduct.TSentenceCreate(engine, desk);
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
                    TInterface.TStateValueCreate("set alight"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("to set something burning"),
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
