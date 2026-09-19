using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftFact
{
    [Fact]
    public void EntryDraftNoted_EmptySections_ReportsEverySectionAbsent()
    {
        LEntryDraft draft = TInterface.TEntryDraftCreate("wolf", "English", string.Empty, string.Empty, [], []);

        Assert.False(draft.LEntryDraftNoted);
        Assert.False(draft.LEntryDraftMarked);
        Assert.False(draft.LEntryDraftDefined);
        Assert.False(draft.LEntryDraftCollocated);
        Assert.False(draft.LEntryDraftReflected);
        Assert.False(draft.LEntryDraftTargeted);
        Assert.Empty(draft.LEntryDraftAccents);
    }

    [Fact]
    public void EntryDraftAccents_ThreePronunciations_SkipsThePrimary()
    {
        LEntryDraft draft = TInterface.TEntryDraftCreate("colour", "English", "ˈkʌlə", "a note", [], []) with
        {
            LEntryDraftPronunciations =
            [
                TInterface.TPronunciationDraftCreate("ˈkʌlə", "RP"),
                TInterface.TPronunciationDraftCreate("ˈkʌlɚ", "GA"),
                TInterface.TPronunciationDraftCreate(string.Empty, "AU"),
            ],
        };

        Assert.Equal(["GA", "AU"], draft.LEntryDraftAccents.Select(spoken => spoken.LPronunciationDraftVariety));
        Assert.True(draft.LEntryDraftAccents[0].LPronunciationDraftNotated);
        Assert.False(draft.LEntryDraftAccents[1].LPronunciationDraftNotated);
        Assert.True(draft.LEntryDraftAccents[0].TPronunciationDraftMatch("GA"));
        Assert.True(draft.LEntryDraftNoted);
    }

    [Fact]
    public void EntryDraftTargets_TranslationsOnBothCardKinds_ListsEachIdOnce()
    {
        LCardDraft meaning = TInterface.TCardDraftCreate("one", "two", "three", [], [], [7, 9], [], [], 0);
        LCardDraft collocation = TInterface.TCardDraftCreate("one", "two", "three", [], [], [9, 11], [], [], 0);
        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "wolf", "English", string.Empty, string.Empty, [meaning], [collocation]);

        Assert.Equal([7, 9, 11], draft.LEntryDraftTargets);
        Assert.True(draft.LEntryDraftTargeted);
        Assert.True(draft.LEntryDraftDefined);
        Assert.True(draft.LEntryDraftCollocated);
    }

    [Fact]
    public void CardDraftTally_NestedChildren_CountsEveryCard()
    {
        LCardDraft leaf = TInterface.TCardDraftCreate("leaf", "", "", [], [], [], [], [], 0);
        LCardDraft branch = TInterface.TCardDraftCreate("branch", "", "", [], [], [], [], [], 0) with
        {
            LCardDraftChild = [leaf, leaf],
        };
        LCardDraft root = TInterface.TCardDraftCreate("root", "", "", [], [], [], [], [], 0) with
        {
            LCardDraftChild = [branch],
            LCardDraftSentence = [TInterface.TSentenceDraftCreate("a sentence")],
        };

        Assert.Equal(4, root.LCardDraftTally);
        Assert.True(root.LCardDraftExemplified);
        Assert.False(leaf.LCardDraftExemplified);
    }

    [Fact]
    public void StateValueShown_UnknownAndBlank_ReportsNothingToShow()
    {
        LStateValue written = TInterface.TStateValueCreate("wolf");
        LStateValue unreadable = TInterface.TStateUnreadableCreate("w0lf");

        Assert.Equal("wolf", written.LStateValueShown);
        Assert.True(written.LStateValueSound);
        Assert.True(written.TStateValueMatch("wolf"));
        Assert.Null(LStateValue.LStateValueUnknown.LStateValueShown);
        Assert.True(LStateValue.LStateValueUnknown.LStateValueUncertain);
        Assert.Null(LStateValue.LStateValueUnspecified.LStateValueShown);
        Assert.True(unreadable.LStateValueLegible);
        Assert.False(unreadable.LStateValueSound);
    }

    [Fact]
    public void StateAnchorShown_ZeroAndStoredIds_ReportsOnlyTheStoredOne()
    {
        LStateAnchor linked = TInterface.TStateAnchorCreate(42);

        Assert.Equal(42, linked.LStateAnchorShown);
        Assert.True(linked.LStateAnchorLinked);
        Assert.True(linked.TStateAnchorMatch(42));
        Assert.False(linked.TStateAnchorMatch(41));
        Assert.Null(LStateAnchor.LStateAnchorUnspecified.LStateAnchorShown);
        Assert.False(LStateAnchor.LStateAnchorUnspecified.LStateAnchorLinked);
    }

    [Fact]
    public void MentionPieceLinked_SilentAndLinkedMentions_MarksOnlyTheLinkedPiece()
    {
        IReadOnlyList<LMentionPiece> pieces = TInterface.TMentionSpanDivide(
            "the grey wolf", [TInterface.TMentionCreate(1, 4, 4, 5, 2), TInterface.TMentionCreate(2, 9, 4, 0)]);

        Assert.Equal([0, 4, 8, 9], pieces.Select(piece => piece.LMentionPieceOffset));
        Assert.Equal([4, 8, 9, 13], pieces.Select(piece => piece.LMentionPieceEnd));
        Assert.Equal([false, true, false, false], pieces.Select(piece => piece.LMentionPieceLinked));
        Assert.True(pieces[1].LMentionPieceStored!.LMentionSensed);
        Assert.True(TInterface.TMentionDraftCreate(1, 4, 4, 5).LMentionDraftLinked);
        Assert.False(TInterface.TMentionDraftCreate(2, 9, 4, 0).LMentionDraftLinked);
    }

    [Fact]
    public void MentionResultSingle_OneTarget_NamesItsId()
    {
        LMentionResult single = TInterface.TMentionResultCreate(
            0, 4, null, [TInterface.TTranslationTargetCreate(8, "wolf", "English")]);
        LMentionResult many = TInterface.TMentionResultCreate(
            0,
            4,
            null,
            [
                TInterface.TTranslationTargetCreate(8, "wolf", "English"),
                TInterface.TTranslationTargetCreate(9, "loup", "French"),
            ]);

        Assert.True(single.LMentionResultSingle);
        Assert.Equal(8, single.LMentionResultFirst);
        Assert.False(single.LMentionResultMany);
        Assert.True(many.LMentionResultMany);
        Assert.Equal(0, many.LMentionResultFirst);
    }
}
