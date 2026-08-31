namespace Llyn.Core;

/// <summary>One pronunciation candidate returned by a lookup source.</summary>
/// <param name="LCandidateSource">The source the candidate came from.</param>
/// <param name="LCandidatePhonetic">The bare phonetic form, without enclosing brackets.</param>
public sealed record LCandidate(LOrigin LCandidateSource, string LCandidatePhonetic);
