using Llyn.Core;

namespace Llyn.Tests;

internal static class TPortraitSample
{
    internal static LPortrait TPortraitSampleCreate()
    {
        LPortraitLabel label = TInterface.TPortraitLabelCreate(
            "Unreadable", "Meaning", "Meanings", "Collocation", "Collocations", "Links here", "Note");

        LPortraitCard sense = TInterface.TPortraitCardCreate(
            1,
            "set alight",
            "Meaning",
            string.Empty,
            "to set something burning",
            ["around a hearth <cold>"],
            ["formal"],
            [TInterface.TPortraitLinkCreate("e2", "불붙이다", "Korean")],
            [TInterface.TPortraitExampleCreate("(+with)", "she knelt to kindle the damp logs")],
            ["literal", "fire & light"],
            [],
            [TInterface.TPortraitMediaCreate("https://youtu.be/dQw4w9WgXcQ", "0:12-0:30", true)]);

        LPortraitCard phrase = TInterface.TPortraitCardCreate(
            1,
            string.Empty,
            "Collocation",
            "kindle interest",
            "to cause interest to begin",
            [],
            [],
            [],
            [],
            [],
            [],
            []);

        return TInterface.TPortraitCreate(
            "kindle",
            "English",
            "/ˈkɪnd(ə)l/",
            ["verb"],
            [sense],
            [phrase],
            [TInterface.TPortraitUsageCreate("불붙이다", "set alight", "Meaning", "Korean")],
            "Chiefly literary.",
            true,
            label);
    }
}
