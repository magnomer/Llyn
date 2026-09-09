using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitMarkup
{
    [Fact]
    public void MarkupDraftFormat_WrittenEntry_ReadsBackAsTheSameDraft()
    {
        LEntryDraft written = TPortraitMarkupCreate();

        string text = TInterface.TMarkupDraftFormat(written, []);
        IReadOnlyList<LEntryDraft> read = TInterface.TMarkupRead(text);

        LEntryDraft back = Assert.Single(read);

        Assert.Equal(written.LEntryDraftHeadword, back.LEntryDraftHeadword);
        Assert.Equal(written.LEntryDraftLanguage, back.LEntryDraftLanguage);
        Assert.Equal(written.LEntryDraftPronunciation, back.LEntryDraftPronunciation);
        Assert.Equal(written.LEntryDraftNote, back.LEntryDraftNote);
        Assert.Equal(written.LEntryDraftSpeeches, back.LEntryDraftSpeeches);
        Assert.Single(back.LEntryDraftMeanings);
        Assert.Single(back.LEntryDraftCollocations);

        LCardDraft sense = back.LEntryDraftMeanings[0];
        Assert.Equal("set alight", sense.LCardDraftTitle.TStateValueShow());
        Assert.Equal("to set something burning", sense.LCardDraftMeaning.TStateValueShow());
        Assert.Equal(["literal"], sense.LCardDraftTag);
        Assert.Equal(
            "she knelt to kindle the damp logs",
            sense.LCardDraftExample[0].LExampleDraftText.TStateValueShow());
        Assert.Equal("with", sense.LCardDraftExample[0].LExampleDraftParticle.TStateValueShow());
        Assert.Equal(
            "kindle interest",
            back.LEntryDraftCollocations[0].LCardDraftExpression.TStateValueShow());
    }

    [Fact]
    public void MarkupDraftFormat_UnreadableField_ReadsBackAsUnreadable()
    {
        LCardDraft card = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnknown,
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("kept"),
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            1);

        LEntryDraft written = TInterface.TEntryDraftCreate(
            "kindle", "English", string.Empty, string.Empty, [card], []);

        LEntryDraft back = Assert.Single(
            TInterface.TMarkupRead(TInterface.TMarkupDraftFormat(written, [])));

        Assert.Equal(LState.LStateUnknown, back.LEntryDraftMeanings[0].LCardDraftTitle.LStateValueState);
        Assert.Equal(
            LState.LStateUnspecified,
            back.LEntryDraftMeanings[0].LCardDraftExpression.LStateValueState);
    }

    [Fact]
    public void MarkupDraftFormat_ClosingTagInsideText_LeavesTheDocumentReadable()
    {
        LCardDraft card = TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate("a </meaning> inside"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("kept"),
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            1);

        LEntryDraft written = TInterface.TEntryDraftCreate(
            "kindle", "English", string.Empty, string.Empty, [card], []);

        LEntryDraft back = Assert.Single(
            TInterface.TMarkupRead(TInterface.TMarkupDraftFormat(written, [])));

        Assert.Equal("kept", back.LEntryDraftMeanings[0].LCardDraftMeaning.TStateValueShow());
    }

    private static LEntryDraft TPortraitMarkupCreate()
    {
        LCardDraft sense = TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("to set something burning"),
            [
                TInterface.TExampleDraftCreate(
                    TInterface.TStateValueCreate("she knelt to kindle the damp logs"),
                    string.Empty,
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("with"),
                    LStateValue.LStateValueUnspecified),
            ],
            [TInterface.TSituationDraftCreate("around a hearth")],
            [],
            string.Empty,
            ["literal"],
            [TInterface.TStateValueCreate("media/kindle.jpg")],
            1);

        LCardDraft phrase = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("kindle interest"),
            TInterface.TStateValueCreate("to cause interest to begin"),
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            1);

        return TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            "/ˈkɪnd(ə)l/",
            "Chiefly literary.",
            [sense],
            [phrase],
            string.Empty,
            null,
            ["verb"]);
    }
}
