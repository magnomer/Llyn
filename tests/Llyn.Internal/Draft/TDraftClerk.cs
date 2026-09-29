using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftClerk
{
    [Fact]
    public void DraftClerkApply_HeadwordRequest_ChangesHeadword()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");

        LDraft applied = clerk.TDraftClerkApply(draft, TInterface.TRequestHeadwordCreate(draft.LDraftId, "ember"));

        Assert.Equal("ember", applied.LDraftContent.LEntryDraftHeadword);
        Assert.Equal("kindle", draft.LDraftContent.LEntryDraftHeadword);
    }

    [Fact]
    public void DraftClerkApply_TagAddition_MintsIdBelowZero()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");
        long card = draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;

        LDraft applied = clerk.TDraftClerkApply(draft, TInterface.TTagAdditionCreate(draft.LDraftId, card, "fire", 0));

        IReadOnlyList<LTagDraft> tags = applied.LDraftContent.LEntryDraftMeanings[0].LCardDraftTag;
        Assert.Equal(draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftTag.Count + 1, tags.Count);
        Assert.Equal("fire", tags[0].LTagDraftText);
        Assert.True(tags[0].LTagDraftId < 0);
    }

    [Fact]
    public void DraftClerkApply_TagText_TrimsTheWording()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");
        long card = draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;
        LDraft added = clerk.TDraftClerkApply(draft, TInterface.TTagAdditionCreate(draft.LDraftId, card, "fire", 0));
        long tag = added.LDraftContent.LEntryDraftMeanings[0].LCardDraftTag[0].LTagDraftId;

        LDraft applied = clerk.TDraftClerkApply(added, TInterface.TTagTextCreate(draft.LDraftId, tag, "  flame "));

        Assert.Equal("flame", applied.LDraftContent.LEntryDraftMeanings[0].LCardDraftTag[0].LTagDraftText);
    }

    [Fact]
    public void DraftListParse_UnsettledWithoutComma_KeepsTheTextWhole()
    {
        List<string> handed = [];

        string rest = TInterface.TDraftListParse(" ani", false, part => { handed.Add(part); return true; });

        Assert.Equal(" ani", rest);
        Assert.Empty(handed);
    }

    [Fact]
    public void DraftListParse_TypedList_HandsTrimmedPartsAndKeepsRefusedBeforeTheRest()
    {
        List<string> handed = [];

        string rest = TInterface.TDraftListParse(
            " cat , , zzyzx,  do", false, part => { handed.Add(part); return part != "zzyzx"; });

        Assert.Equal(["cat", "zzyzx"], handed);
        Assert.Equal("zzyzx, do", rest);
    }

    [Fact]
    public void DraftListParse_Settled_HandsEveryPartAndKeepsOnlyTheRefused()
    {
        List<string> handed = [];

        string taken = TInterface.TDraftListParse("cat, dog ", true, part => { handed.Add(part); return true; });
        string blank = TInterface.TDraftListParse("   ", true, part => { handed.Add(part); return true; });

        Assert.Equal(["cat", "dog"], handed);
        Assert.Equal(string.Empty, taken);
        Assert.Equal(string.Empty, blank);
    }

    [Fact]
    public void DraftClerkApply_DuplicateScheme_ThrowsRefusal()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");

        LDraft once = clerk.TDraftClerkApply(
            draft, TInterface.TTranscriptionAdditionCreate(draft.LDraftId, "Pinyin", 0));

        Assert.Single(once.LDraftContent.LEntryDraftTranscriptions);
        Assert.Throws<LRefusal>(() =>
            clerk.TDraftClerkApply(once, TInterface.TTranscriptionAdditionCreate(draft.LDraftId, "Pinyin", 1)));
    }

    [Fact]
    public void GlossEmptyCheck_ExampleWithAndWithoutGloss_AnswersOnlyForAnExampleHoldingNone()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft entry = TInterface.TDraftNestedCreate("Input", "kindle");
        LDraft example = entry with
        {
            LDraftExample = TInterface.TExampleCreate(
                0, "English", TInterface.TStateValueCreate("a cat"), null, TInterface.TStateAnchorRead(null)),
        };

        LDraft glossed = clerk.TDraftClerkApply(
            example, TInterface.TGlossAdditionCreate(example.LDraftId, 0, 0, "French", 0));

        Assert.False(TInterface.TGlossEmptyCheck(entry, 0, 0));
        Assert.True(TInterface.TGlossEmptyCheck(example, 0, 0));
        Assert.False(TInterface.TGlossEmptyCheck(glossed, 0, 0));
    }

    [Fact]
    public void DraftClerkApply_UnknownRequestKind_ThrowsArgument()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");

        Assert.Throws<ArgumentException>(() =>
            clerk.TDraftClerkApply(draft, TInterface.TRequestStrayCreate(draft.LDraftId)));
    }
}
