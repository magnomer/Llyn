using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal sealed class BBlueprintPart
{
    internal required string BBlueprintPartElement { get; init; }

    internal string? BBlueprintPartName { get; init; }

    internal string? BBlueprintPartClass { get; init; }

    internal string? BBlueprintPartSource { get; init; }

    internal required string BBlueprintPartTheme { get; init; }

    internal string? BBlueprintPartGlyphs { get; init; }

    internal string? BBlueprintPartKey { get; init; }

    internal IReadOnlyDictionary<string, IReadOnlyList<string>>? BBlueprintPartMarks { get; init; }

    internal required IReadOnlyList<int> BBlueprintPartMargin { get; init; }

    internal required IReadOnlyList<BBlueprintTrack> BBlueprintPartTracks { get; init; }

    internal bool BBlueprintPartAcross { get; init; }

    internal required IReadOnlyList<BBlueprintPart> BBlueprintPartChildren { get; init; }

    internal int BBlueprintPartRow { get; set; }

    internal int BBlueprintPartColumn { get; set; }

    internal int BBlueprintPartWidth { get; set; }

    internal int BBlueprintPartHeight { get; set; }

    internal IReadOnlyList<int> BBlueprintPartRules { get; set; } = [];
}
