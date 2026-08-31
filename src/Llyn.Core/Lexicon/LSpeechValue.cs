namespace Llyn.Core;

/// <summary>
/// One entry in the language-controlled part-of-speech display vocabulary: it maps a stable POS id to
/// the display name shown for a given language. Entries store only the id (<see cref="LSpeech"/>); the
/// name lives here and is resolved by <c>(language, value_id)</c>, never copied onto the entry's rows.
/// </summary>
/// <param name="LSpeechValueLanguage">Language the display name is governed by.</param>
/// <param name="LSpeechValueId">Stable POS id (for example <c>noun</c>).</param>
/// <param name="LSpeechValueName">Display name for the language (for example <c>Noun</c>).</param>
/// <param name="LSpeechValuePosition">Display order within the language's vocabulary.</param>
public sealed record LSpeechValue(
    string LSpeechValueLanguage,
    string LSpeechValueId,
    string LSpeechValueName,
    int LSpeechValuePosition);
