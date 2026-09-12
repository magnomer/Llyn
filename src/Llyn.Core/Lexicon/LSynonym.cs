namespace Llyn.Core;

public sealed record LSynonym(
    long LSynonymId,
    long LSynonymCollocationId,
    int LSynonymPosition,
    LStateAnchor LSynonymTargetEntry,
    LStateAnchor LSynonymTargetMeaning)
{
    public LStateAnchor LSynonymTargetEntry { get; init; } =
        LSynonymTargetEntry ?? LStateAnchor.LStateAnchorUnspecified;

    public LStateAnchor LSynonymTargetMeaning { get; init; } =
        LSynonymTargetMeaning ?? LStateAnchor.LStateAnchorUnspecified;
}
