namespace Llyn.Core;

public sealed record LSyllable(
    long LSyllablePronunciationId,
    int LSyllablePosition,
    string? LSyllableOrthography,
    string? LSyllableLocal,
    string? LSyllableOnset,
    string? LSyllableMedial,
    string LSyllableNucleus,
    string? LSyllableCoda,
    int? LSyllableToneNumber,
    string? LSyllableToneLocal,
    string? LSyllableTonePoints);
