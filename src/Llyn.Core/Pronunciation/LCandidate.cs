namespace Llyn.Core;

/// <summary>One pronunciation candidate returned by a lookup source.</summary>
/// <param name="LCandidateSource">The name of the source the candidate came from, as declared by
/// that source's language pack (for example <c>"Cambridge"</c>). Source identity is a config-driven
/// name, never a compile-time enum, so the lookup stays language-agnostic.</param>
/// <param name="LCandidatePhonetic">The bare phonetic form, without enclosing brackets.</param>
public sealed record LCandidate(string LCandidateSource, string LCandidatePhonetic);
