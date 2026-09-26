using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TTranslationClerk
{
    [Fact]
    public void TranslationClerkFind_ExactHeadword_ListsItBeforePartialMatches()
    {
        TVaultFake entries = new();
        entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "breakfast", "English", 0, null, null));
        entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "break", "English", 0, null, null));
        LTranslationClerk clerk = TInterface.TTranslationClerkCreate(TInterface.TRigClerkCreate(entries));

        IReadOnlyList<LEntry> found = clerk.TTranslationClerkFind("break", null);

        Assert.Equal(["break", "breakfast"], found.Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void TranslationClerkFind_OwnEntry_IsLeftOut()
    {
        TVaultFake entries = new();
        LEntry own = entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "break", "English", 0, null, null));
        entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "부수다", "Korean", 0, null, null));
        LTranslationClerk clerk = TInterface.TTranslationClerkCreate(TInterface.TRigClerkCreate(entries));

        IReadOnlyList<LEntry> found = clerk.TTranslationClerkFind(string.Empty, own.LEntryId);

        LEntry single = Assert.Single(found);
        Assert.Equal("부수다", single.LEntryHeadword);
    }

    [Fact]
    public void TranslationClerkResolve_TwoEntriesShareHeadword_AnswersNothing()
    {
        TVaultFake entries = new();
        entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "break", "English", 0, null, null));
        entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "Break", "German", 0, null, null));
        LTranslationClerk clerk = TInterface.TTranslationClerkCreate(TInterface.TRigClerkCreate(entries));

        Assert.Null(clerk.TTranslationClerkResolve("break", null));
    }

    [Fact]
    public void TranslationClerkResolve_OneExactHeadword_AnswersIt()
    {
        TVaultFake entries = new();
        entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "breakfast", "English", 0, null, null));
        LEntry exact = entries.TVaultFakeAdd(TInterface.TEntryCreate(0, "break", "English", 0, null, null));
        LTranslationClerk clerk = TInterface.TTranslationClerkCreate(TInterface.TRigClerkCreate(entries));

        Assert.Equal(exact.LEntryId, clerk.TTranslationClerkResolve(" break ", null)?.LEntryId);
        Assert.Null(clerk.TTranslationClerkResolve("brea", null));
    }
}
