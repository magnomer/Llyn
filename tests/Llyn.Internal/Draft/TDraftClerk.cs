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
    public void DraftClerkApply_UnknownRequestKind_ThrowsArgument()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");

        Assert.Throws<ArgumentException>(() =>
            clerk.TDraftClerkApply(draft, TInterface.TRequestStrayCreate(draft.LDraftId)));
    }
}
