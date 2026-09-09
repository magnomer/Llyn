namespace Llyn.Core;

public sealed record LPortraitLabel(
    string LPortraitLabelUnreadable,
    string LPortraitLabelMeaning,
    string LPortraitLabelMeanings,
    string LPortraitLabelCollocation,
    string LPortraitLabelCollocations,
    string LPortraitLabelIncoming,
    string LPortraitLabelNote)
{
    public static LPortraitLabel LPortraitLabelDefault { get; } = new(
        "Unreadable",
        "Meaning",
        "Meanings",
        "Collocation",
        "Collocations",
        "Links here",
        "Note");
}
