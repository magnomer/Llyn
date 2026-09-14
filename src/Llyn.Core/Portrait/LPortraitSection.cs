using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitSection(
    string LPortraitSectionHeading,
    IReadOnlyList<LPortraitLine> LPortraitSectionLine,
    string LPortraitSectionNote,
    IReadOnlyList<LPortraitMedia> LPortraitSectionImage,
    IReadOnlyList<LPortraitMedia> LPortraitSectionVideo)
{
    public static LPortraitSection LPortraitSectionCreate(string heading, string text)
    {
        return new LPortraitSection(heading, [new LPortraitLine(string.Empty, text)], string.Empty, [], []);
    }

    public static LPortraitSection LPortraitSectionCreate(string heading, IReadOnlyList<LPortraitLine> lines)
    {
        return new LPortraitSection(heading, lines, string.Empty, [], []);
    }
}
