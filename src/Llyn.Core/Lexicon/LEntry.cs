namespace Llyn.Core;

public sealed record LEntry(
    string LEntryId,
    string LEntryHeadword,
    string LEntryLanguage,
    string? LEntryProficiency,
    string? LEntryFrequency,
    string? LEntryAddedUtc,
    string? LEntryUpdatedUtc);
