namespace Llyn.Core;

public sealed record LSynonym(
    string LSynonymId,
    string LSynonymCollocationId,
    int LSynonymPosition,
    string? LSynonymTargetEntry,
    string? LSynonymTargetMeaning);
