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
            LDraftExample = TInterfaceExample.TExampleCreate(
                0, "English", TInterfaceState.TStateValueCreate("a cat"), null, TInterfaceState.TStateAnchorRead(null)),
        };

        LDraft glossed = clerk.TDraftClerkApply(
            example, TInterface.TGlossAdditionCreate(example.LDraftId, 0, 0, "French", 0));

        Assert.False(TInterface.TGlossEmptyCheck(entry, 0, 0));
        Assert.True(TInterface.TGlossEmptyCheck(example, 0, 0));
        Assert.False(TInterface.TGlossEmptyCheck(glossed, 0, 0));
    }

    [Fact]
    public void DraftClerkApply_GlossAdditionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, long sentence) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TGlossAdditionCreate(
                draft.LDraftId, card, sentence, "French", 0, LCardKind.LCardKindCollocation));

        Assert.Empty(TTwinExampleRead(applied.LDraftContent.LEntryDraftMeanings).LExampleDraftGloss);
        Assert.Single(TTwinExampleRead(applied.LDraftContent.LEntryDraftCollocations).LExampleDraftGloss);
    }

    [Fact]
    public void DraftClerkApply_MentionAdditionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, long sentence) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TMentionAdditionCreate(
                draft.LDraftId, card, sentence, 0, 3, 0, 0, LCardKind.LCardKindCollocation));

        Assert.Empty(TTwinExampleRead(applied.LDraftContent.LEntryDraftMeanings).LExampleDraftMention);
        Assert.Single(TTwinExampleRead(applied.LDraftContent.LEntryDraftCollocations).LExampleDraftMention);
    }

    [Fact]
    public void DraftClerkApply_SentenceAdditionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TSentenceAdditionCreate(draft.LDraftId, card, 1, LCardKind.LCardKindCollocation));

        Assert.Single(applied.LDraftContent.LEntryDraftMeanings[0].LCardDraftSentence);
        Assert.Equal(2, applied.LDraftContent.LEntryDraftCollocations[0].LCardDraftSentence.Count);
    }

    [Fact]
    public void DraftClerkApply_TitleOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TRequestTitleCreate(
                draft.LDraftId, card, TInterfaceState.TStateValueCreate("blaze"), LCardKind.LCardKindCollocation));

        Assert.Equal(
            draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftTitle,
            applied.LDraftContent.LEntryDraftMeanings[0].LCardDraftTitle);
        Assert.Equal(
            TInterfaceState.TStateValueCreate("blaze"),
            applied.LDraftContent.LEntryDraftCollocations[0].LCardDraftTitle);
    }

    [Fact]
    public void DraftClerkApply_ExpressionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TRequestExpressionCreate(
                draft.LDraftId, card, TInterfaceState.TStateValueCreate("set alight"), LCardKind.LCardKindCollocation));

        Assert.Equal(
            draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftExpression,
            applied.LDraftContent.LEntryDraftMeanings[0].LCardDraftExpression);
        Assert.Equal(
            TInterfaceState.TStateValueCreate("set alight"),
            applied.LDraftContent.LEntryDraftCollocations[0].LCardDraftExpression);
    }

    [Fact]
    public void DraftClerkApply_RemovalOfCollocationSharingMeaningIds_RemovesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TRequestRemovalCreate(draft.LDraftId, card, LCardKind.LCardKindCollocation));

        Assert.Single(applied.LDraftContent.LEntryDraftMeanings);
        Assert.Empty(applied.LDraftContent.LEntryDraftCollocations);
    }

    [Fact]
    public void DraftClerkApply_SenseAdditionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(draft, TInterface.TRequestSenseCreate(draft.LDraftId, card, card));

        Assert.Equal(
            draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftSense,
            applied.LDraftContent.LEntryDraftMeanings[0].LCardDraftSense);
        Assert.Equal([card], applied.LDraftContent.LEntryDraftCollocations[0].LCardDraftSense);
    }

    [Fact]
    public void DraftClerkApply_TagAdditionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TTagAdditionCreate(draft.LDraftId, card, "blaze", 0, LCardKind.LCardKindCollocation));

        TTwinChangeCheck(draft, applied, static row => row.LCardDraftTag.Count);
    }

    [Fact]
    public void DraftClerkApply_TranslationPickOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TTranslationPickCreate(draft.LDraftId, card, 9041, 0, LCardKind.LCardKindCollocation));

        TTwinChangeCheck(draft, applied, static row => row.LCardDraftTranslation.Count);
    }

    [Fact]
    public void DraftClerkApply_RegisterRemovalOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft held, long card, _) = TDraftTwinCreate();
        LRegisterDraft register = TInterface.TRegisterDraftCreate(
            TInterfaceState.TStateValueCreate("archaic"), TInterface.TIdentityCreate());
        LDraft draft = held with
        {
            LDraftContent = held.LDraftContent with
            {
                LEntryDraftMeanings =
                    [held.LDraftContent.LEntryDraftMeanings[0] with { LCardDraftRegister = [register] }],
                LEntryDraftCollocations =
                    [held.LDraftContent.LEntryDraftCollocations[0] with { LCardDraftRegister = [register] }],
            },
        };

        LDraft applied = clerk.TDraftClerkApply(
            draft,
            TInterface.TRegisterRemovalCreate(
                draft.LDraftId, card, register.LRegisterDraftId, LCardKind.LCardKindCollocation));

        TTwinChangeCheck(draft, applied, static row => row.LCardDraftRegister.Count, -1);
    }

    [Fact]
    public void DraftClerkApply_SituationRemovalOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();
        long situation = draft.LDraftContent.LEntryDraftCollocations[0].LCardDraftSituation[0].LSituationDraftId;

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TSituationRemovalCreate(draft.LDraftId, card, situation, LCardKind.LCardKindCollocation));

        TTwinChangeCheck(draft, applied, static row => row.LCardDraftSituation.Count, -1);
    }

    [Fact]
    public void DraftClerkApply_ImageAdditionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TImageAdditionCreate(draft.LDraftId, card, 0, LCardKind.LCardKindCollocation));

        TTwinChangeCheck(draft, applied, static row => row.LCardDraftImage.Count);
    }

    [Fact]
    public void DraftClerkApply_VideoAdditionOnCollocationSharingMeaningIds_ChangesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft draft, long card, _) = TDraftTwinCreate();

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TVideoAdditionCreate(draft.LDraftId, card, 0, LCardKind.LCardKindCollocation));

        TTwinChangeCheck(draft, applied, static row => row.LCardDraftVideo.Count);
    }

    [Fact]
    public void DraftClerkApply_ShiftOfCollocationSharingMeaningIds_MovesTheCollocationOnly()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        (LDraft held, long card, _) = TDraftTwinCreate();
        LCardDraft other = held.LDraftContent.LEntryDraftMeanings[0] with
        {
            LCardDraftId = card + 1,
            LCardDraftPosition = 2,
        };
        LDraft draft = held with
        {
            LDraftContent = held.LDraftContent with
            {
                LEntryDraftMeanings = [.. held.LDraftContent.LEntryDraftMeanings, other],
                LEntryDraftCollocations = [.. held.LDraftContent.LEntryDraftCollocations, other],
            },
        };

        LDraft applied = clerk.TDraftClerkApply(
            draft, TInterface.TRequestShiftCreate(draft.LDraftId, card, 0, 1, LCardKind.LCardKindCollocation));

        Assert.Equal(card, applied.LDraftContent.LEntryDraftMeanings[0].LCardDraftId);
        Assert.Equal(card + 1, applied.LDraftContent.LEntryDraftCollocations[0].LCardDraftId);
        Assert.Equal(card, applied.LDraftContent.LEntryDraftCollocations[1].LCardDraftId);
    }

    [Fact]
    public void CardShiftRead_CollocationSharingMeaningIds_ReadsTheCollocationList()
    {
        (LDraft held, long card, _) = TDraftTwinCreate();
        LCardDraft other = held.LDraftContent.LEntryDraftCollocations[0] with
        {
            LCardDraftId = card + 1,
            LCardDraftPosition = 2,
        };
        LEntryDraft content = held.LDraftContent with
        {
            LEntryDraftCollocations = [.. held.LDraftContent.LEntryDraftCollocations, other],
        };

        Assert.Equal(1, TInterface.TCardShiftRead(content, card, 1, LCardKind.LCardKindCollocation));
        Assert.Null(TInterface.TCardShiftRead(content, card, 1));
    }

    private static void TTwinChangeCheck(LDraft draft, LDraft applied, Func<LCardDraft, int> count, int step = 1)
    {
        Assert.Equal(
            count(draft.LDraftContent.LEntryDraftMeanings[0]), count(applied.LDraftContent.LEntryDraftMeanings[0]));
        Assert.Equal(
            count(draft.LDraftContent.LEntryDraftCollocations[0]) + step,
            count(applied.LDraftContent.LEntryDraftCollocations[0]));
    }

    private static (LDraft, long, long) TDraftTwinCreate()
    {
        LDraft held = TInterface.TDraftNestedCreate("Input", "kindle");
        LCardDraft meaning = held.LDraftContent.LEntryDraftMeanings[0];
        LSentenceDraft sentence = meaning.LCardDraftSentence[0];
        LDraft draft = held with
        {
            LDraftContent = held.LDraftContent with
            {
                LEntryDraftMeanings = [meaning with { LCardDraftChild = [] }],
                LEntryDraftCollocations = [meaning with { LCardDraftChild = [] }],
            },
        };

        return (draft, meaning.LCardDraftId, sentence.LSentenceDraftId);
    }

    private static LExampleDraft TTwinExampleRead(IReadOnlyList<LCardDraft> cards) =>
        cards[0].LCardDraftSentence[0].LSentenceDraftExample!;

    [Fact]
    public void DraftClerkApply_UnknownRequestKind_ThrowsArgument()
    {
        LDraftClerk clerk = TInterface.TDraftClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");

        Assert.Throws<ArgumentException>(() =>
            clerk.TDraftClerkApply(draft, TInterface.TRequestStrayCreate(draft.LDraftId)));
    }
}
