using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitSection(
    string LPortraitSectionHeading,
    IReadOnlyList<LPortraitLine> LPortraitSectionLine,
    string LPortraitSectionNote,
    IReadOnlyList<LPortraitMedia> LPortraitSectionImage,
    IReadOnlyList<LPortraitMedia> LPortraitSectionVideo,
    int LPortraitSectionPosition = 0,
    IReadOnlyList<string>? LPortraitSectionChip = null,
    IReadOnlyList<LPortraitLink>? LPortraitSectionLink = null,
    IReadOnlyList<LPortraitSection>? LPortraitSectionChild = null,
    LPortraitRole LPortraitSectionRole = LPortraitRole.LPortraitRoleBand)
{
    public IReadOnlyList<string> LPortraitSectionChip { get; init; } = LPortraitSectionChip ?? [];

    public IReadOnlyList<LPortraitLink> LPortraitSectionLink { get; init; } = LPortraitSectionLink ?? [];

    public IReadOnlyList<LPortraitSection> LPortraitSectionChild { get; init; } = LPortraitSectionChild ?? [];

    public static LPortraitSection LPortraitSectionCreate(string heading, string text)
    {
        return new LPortraitSection(heading, [new LPortraitLine(string.Empty, text)], string.Empty, [], []);
    }

    public static LPortraitSection LPortraitSectionCreate(string heading, IReadOnlyList<LPortraitLine> lines)
    {
        return new LPortraitSection(heading, lines, string.Empty, [], []);
    }

    public static LPortraitSection LPortraitSectionCreate(string heading, IReadOnlyList<LPortraitSection> children)
    {
        return new LPortraitSection(heading, [], string.Empty, [], [], LPortraitSectionChild: children);
    }

    public static LPortraitSection LPortraitSectionCreate(
        string heading, LPortraitRole role, IReadOnlyList<string> chips, IReadOnlyList<LPortraitLink> links)
    {
        return new LPortraitSection(
            heading,
            [],
            string.Empty,
            [],
            [],
            LPortraitSectionChip: chips,
            LPortraitSectionLink: links,
            LPortraitSectionRole: role);
    }
}
