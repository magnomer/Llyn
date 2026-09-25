using Llyn.Core;

namespace Llyn.Tests;

internal static class TPortraitSample
{
    internal static LPortraitPage TPortraitSampleCreate()
    {
        LPortraitSection mention = TInterface.TPortraitSectionCreate(
            "Mentions",
            0,
            [],
            [],
            [TInterface.TPortraitLinkCreate("log", "English")],
            [],
            [],
            [],
            LPortraitRole.LPortraitRoleBridge);

        LPortraitSection quote = TInterface.TPortraitSectionCreate(
            "Example",
            0,
            [
                TInterface.TPortraitLineCreate("(+with)", "she knelt to kindle the damp logs"),
                TInterface.TPortraitLineCreate("Korean", "그녀는 무릎을 꿇고 젖은 장작에 불을 붙였다"),
                TInterface.TPortraitLineCreate("Source", "Field Notes"),
            ],
            [],
            [],
            [],
            [],
            [mention],
            LPortraitRole.LPortraitRoleQuote);

        LPortraitSection sense = TInterface.TPortraitSectionCreate(
            "set alight",
            1,
            [TInterface.TPortraitLineCreate(string.Empty, "to set something burning")],
            [],
            [],
            [],
            [TInterface.TPortraitMediaCreate("https://youtu.be/dQw4w9WgXcQ", "0:12-0:30")],
            [
                TInterface.TPortraitSectionCreate(
                    "Situations",
                    0,
                    [],
                    ["around a hearth <cold>"],
                    [],
                    [],
                    [],
                    [],
                    LPortraitRole.LPortraitRoleScene),
                TInterface.TPortraitSectionCreate(
                    "Registers", 0, [], ["formal"], [], [], [], [], LPortraitRole.LPortraitRoleTone),
                TInterface.TPortraitSectionCreate(
                    "Translations",
                    0,
                    [],
                    [],
                    [TInterface.TPortraitLinkCreate("불붙이다", "Korean")],
                    [],
                    [],
                    [],
                    LPortraitRole.LPortraitRoleBridge),
                quote,
                TInterface.TPortraitSectionCreate(
                    "Tags", 0, [], ["literal", "fire & light"], [], [], [], [], LPortraitRole.LPortraitRoleLabel),
            ],
            LPortraitRole.LPortraitRoleCard);

        LPortraitSection phrase = TInterface.TPortraitSectionCreate(
            "Collocation",
            1,
            [TInterface.TPortraitLineCreate(string.Empty, "to cause interest to begin")],
            [],
            [],
            [],
            [],
            [
                TInterface.TPortraitSectionCreate(
                    string.Empty,
                    0,
                    [TInterface.TPortraitLineCreate(string.Empty, "kindle interest")],
                    [],
                    [],
                    [],
                    [],
                    [],
                    LPortraitRole.LPortraitRolePhrase),
            ],
            LPortraitRole.LPortraitRoleKind);

        return TInterface.TPortraitPageCreate(
            "kindle",
            "English",
            true,
            [TInterface.TPortraitLineCreate(string.Empty, "ˈkɪnd(ə)l", "/", "/")],
            ["verb"],
            [
                TInterface.TPortraitSectionCreate("Meanings", 0, [], [], [], [], [], [sense]),
                TInterface.TPortraitSectionCreate("Collocations", 0, [], [], [], [], [], [phrase]),
                TInterface.TPortraitSectionCreate(
                    "Links here",
                    0,
                    [],
                    [],
                    [],
                    [],
                    [],
                    [
                        TInterface.TPortraitSectionCreate(
                            string.Empty,
                            0,
                            [TInterface.TPortraitLineCreate("Meaning", "set alight")],
                            [],
                            [TInterface.TPortraitLinkCreate("불붙이다", "Korean")],
                            [],
                            [],
                            [],
                            LPortraitRole.LPortraitRoleUsage),
                    ]),
                TInterface.TPortraitSectionCreate(
                    "Note", [], "Chiefly *literary*.\r\n\r\n- poetic\n- archaic", [], []),
            ]);
    }
}
