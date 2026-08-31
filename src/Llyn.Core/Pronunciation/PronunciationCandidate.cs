namespace Llyn.Core;

/// <summary>One pronunciation candidate returned by a lookup source.</summary>
/// <param name="Source">The source the candidate came from.</param>
/// <param name="Phonetic">The bare phonetic form, without enclosing brackets.</param>
public sealed record PronunciationCandidate(LookupSource Source, string Phonetic);
