using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitMarkup
{
    [Fact]
    public void MarkupDraftFormat_WrittenEntry_ReturnsEveryTag()
    {
        string text = TInterface.TMarkupDraftFormat(TPortraitMarkupCreate(), []);

        Assert.Contains("<headword>kindle</headword>", text);
        Assert.Contains("<lang>English</lang>", text);
        Assert.Contains("<ipa>/ˈkɪnd(ə)l/</ipa>", text);
        Assert.Contains("<pos>verb</pos>", text);
        Assert.Contains("<note>Chiefly literary.</note>", text);
        Assert.Contains("<title>set alight</title>", text);
        Assert.Contains("<meaning>to set something burning</meaning>", text);
        Assert.Contains("<situation>around a hearth</situation>", text);
        Assert.Contains("<example par=\"with\">she knelt to kindle the damp logs</example>", text);
        Assert.Contains("<tag>literal</tag>", text);
        Assert.Contains("<image>media/kindle.jpg</image>", text);
        Assert.Contains("<expression>kindle interest</expression>", text);
    }

    [Fact]
    public void MarkupDraftFormat_UnreadableField_WritesAnEmptyTag()
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

        string text = TInterface.TMarkupDraftFormat(written, []);

        Assert.Contains("<title></title>", text);
        Assert.DoesNotContain("<expression>", text);
    }

    [Fact]
    public void MarkupDraftFormat_ClosingTagInsideText_DropsThatTag()
    {
        LCardDraft card = TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("a </meaning> inside"),
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            1);

        LEntryDraft written = TInterface.TEntryDraftCreate(
            "kindle", "English", string.Empty, string.Empty, [card], []);

        string text = TInterface.TMarkupDraftFormat(written, []);

        Assert.Contains("<meaning>a  inside</meaning>", text);
        Assert.Contains("<title>set alight</title>", text);
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
