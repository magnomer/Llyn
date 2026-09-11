namespace Llyn.Core;

public sealed record LSynonym(
    long LSynonymId,
    long LSynonymCollocationId,
    int LSynonymPosition,
    long? LSynonymTargetEntry,
    long? LSynonymTargetMeaning);
