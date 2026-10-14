using System;
using System.Collections.Generic;
using System.Linq;

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
    LPortraitRole LPortraitSectionRole = LPortraitRole.LPortraitRoleBand,
    bool LPortraitSectionFold = false,
    LLineage? LPortraitSectionLineage = null)
{
    public IReadOnlyList<string> LPortraitSectionChip { get; init; } = LPortraitSectionChip ?? [];

    public IReadOnlyList<LPortraitLink> LPortraitSectionLink { get; init; } = LPortraitSectionLink ?? [];

    public IReadOnlyList<LPortraitSection> LPortraitSectionChild { get; init; } = LPortraitSectionChild ?? [];

    internal static LPortraitSection LPortraitSectionOpen(LPortraitSection section)
    {
        ArgumentNullException.ThrowIfNull(section);

        return section with
        {
            LPortraitSectionChild = section.LPortraitSectionChild.Select(LPortraitSectionOpen).ToList(),
            LPortraitSectionFold = false,
        };
    }

    public static LPortraitSection LPortraitSectionCreate(string heading, string text)
    {
        return new LPortraitSection(heading, [new LPortraitLine(string.Empty, text)], string.Empty, [], []);
    }

    public static void LPortraitSectionAdd(
        List<LPortraitSection> sections, string heading, LStateValue value, string mark)
    {
        ArgumentNullException.ThrowIfNull(sections);

        string text = LPortraitText.LPortraitTextRead(value, mark);
        if (text.Length > 0)
        {
            sections.Add(LPortraitSectionCreate(heading, text));
        }
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
