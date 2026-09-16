namespace Llyn.Core;

public sealed record LEntry(
    long LEntryId,
    string LEntryHeadword,
    string LEntryLanguage,
    int LEntryGrasp,
    string? LEntryAddedUtc,
    string? LEntryUpdatedUtc);
