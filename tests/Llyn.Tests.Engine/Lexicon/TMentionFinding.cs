using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMentionFinding
{
    [Fact]
    public void MentionFind_StoredMentionCoversOffset_WinsOverCandidates()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TMentionEntryCreate(engine, "kindle", "English");
        LExample stored = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", "she knelt to kindle the damp logs", null, null) with
        {
            LExampleMention = [TInterface.TMentionCreate(0, 10, 9, 0)],
        });

        LMentionResult found = engine.TEngineMentionFind(stored.LExampleId, 15);

        Assert.Equal(10, found.LMentionResultOffset);
        Assert.Equal(0, found.LMentionResultStored!.LMentionEntryId);
        Assert.Empty(found.LMentionResultEntry);
        Assert.NotEqual(0, entryId);
    }

    [Fact]
    public void MentionFind_TwoHomographEntries_ReturnsBothInHeadwordOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long second = TMentionEntryCreate(engine, "Kindle", "English");
        long first = TMentionEntryCreate(engine, "kindle", "English");
        TMentionEntryCreate(engine, "kindle", "Welsh");

        LMentionResult found = engine.TEngineMentionFind("she knelt to kindle the damp logs", "English", 15, []);

        Assert.Equal(13, found.LMentionResultOffset);
        Assert.Null(found.LMentionResultStored);
        Assert.Equal([second, first], found.LMentionResultEntry.Select(target => target.LTranslationTargetId));
    }

    [Fact]
    public void MentionFind_ThreeHomographEntries_ListsAllInHeadwordOrderWithoutForeignOnes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long third = TMentionEntryCreate(engine, "kindle", "English");
        long first = TMentionEntryCreate(engine, "KINDLE", "English");
        long second = TMentionEntryCreate(engine, "Kindle", "English");
        long foreign = TMentionEntryCreate(engine, "kindle", "Welsh");

        LMentionResult found = engine.TEngineMentionFind("she knelt to kindle the damp logs", "English", 15, []);

        Assert.Equal([first, second, third], found.LMentionResultEntry.Select(target => target.LTranslationTargetId));
        Assert.DoesNotContain(foreign, found.LMentionResultEntry.Select(target => target.LTranslationTargetId));
        Assert.All(found.LMentionResultEntry, target => Assert.Equal("English", target.LTranslationTargetLanguage));
    }

    [Fact]
    public void MentionFind_InflectedFormWithoutHeadword_ReturnsSpanAndNoCandidates()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TMentionEntryCreate(engine, "kindle", "English");

        LMentionResult found = engine.TEngineMentionFind("she kindled the damp logs", "English", 6, []);
        LMentionResult blank = engine.TEngineMentionFind("she kindled the damp logs", "English", 3, []);

        Assert.Equal(4, found.LMentionResultOffset);
        Assert.Empty(found.LMentionResultEntry);
        Assert.Empty(blank.LMentionResultEntry);
    }

    [Fact]
    public void MentionFind_JapaneseText_PicksTheLongestHeadwordOverTheLetterRun()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TMentionEntryCreate(engine, "日本", "Japanese");
        long longest = TMentionEntryCreate(engine, "日本語", "Japanese");

        LMentionResult found = engine.TEngineMentionFind("私は日本語を話す", "Japanese", 3, []);
        LMentionResult fallback = engine.TEngineMentionFind("私は日本語を話す", "Japanese", 6, []);

        Assert.Equal(2, found.LMentionResultOffset);
        Assert.Equal(longest, Assert.Single(found.LMentionResultEntry).LTranslationTargetId);
        Assert.Equal(6, fallback.LMentionResultOffset);
        Assert.Empty(fallback.LMentionResultEntry);
    }

    private static long TMentionEntryCreate(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, language, string.Empty, string.Empty, [], [])).LEntryId;
    }
}
