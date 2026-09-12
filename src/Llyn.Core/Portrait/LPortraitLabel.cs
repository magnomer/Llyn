namespace Llyn.Core;

public sealed record LPortraitLabel(
    string LPortraitLabelUnknown,
    string LPortraitLabelMeaning,
    string LPortraitLabelMeanings,
    string LPortraitLabelCollocation,
    string LPortraitLabelCollocations,
    string LPortraitLabelIncoming,
    string LPortraitLabelNote)
{
    public static LPortraitLabel LPortraitLabelDefault { get; } = new(
        "Unknown",
        "Meaning",
        "Meanings",
        "Collocation",
        "Collocations",
        "Links here",
        "Note");
}
