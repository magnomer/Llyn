namespace Llyn.Core;

public sealed record LSyllable(
    long LSyllablePronunciationId,
    int LSyllablePosition,
    string? LSyllableOnset,
    string? LSyllableMedial,
    string LSyllableNucleus,
    string? LSyllableCoda,
    int? LSyllableToneNumber,
    string? LSyllableTonePoints);
