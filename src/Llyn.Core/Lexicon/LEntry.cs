namespace Llyn.Core;

/// <summary>
/// A lexical entry: the headword record and the root that owns its written forms, parts of speech,
/// and every other subordinate lexical structure. <see cref="LEntryId"/> is the identity — an opaque
/// program-generated id — and the headword is never identity, so two entries with the same headword
/// stay distinct records.
/// </summary>
/// <param name="LEntryId">Opaque, program-generated stable id.</param>
/// <param name="LEntryHeadword">Main headword; display text, not an identifier.</param>
/// <param name="LEntryLanguage">Language identifier the entry belongs to.</param>
/// <param name="LEntryProficiency">Optional proficiency information.</param>
/// <param name="LEntryFrequency">Optional frequency information.</param>
/// <param name="LEntryAddedUtc">Optional creation timestamp, ISO 8601 UTC.</param>
/// <param name="LEntryUpdatedUtc">Optional last-modification timestamp, ISO 8601 UTC.</param>
public sealed record LEntry(
    string LEntryId,
    string LEntryHeadword,
    string LEntryLanguage,
    string? LEntryProficiency,
    string? LEntryFrequency,
    string? LEntryAddedUtc,
    string? LEntryUpdatedUtc);
