namespace Llyn.Core;

/// <summary>
/// One part-of-speech assignment on an entry, ordered within it. Identity is
/// <c>(entry_id, position)</c>. The entry stores only the stable POS id
/// (<see cref="LSpeechValueId"/>), never the display name — the display name is resolved from the
/// language-controlled vocabulary (<see cref="LSpeechValue"/>).
/// </summary>
/// <param name="LSpeechEntryId">Parent entry id.</param>
/// <param name="LSpeechPosition">Order within the parent entry.</param>
/// <param name="LSpeechValueId">Stable, language-controlled POS id (for example <c>noun</c>).</param>
public sealed record LSpeech(
    string LSpeechEntryId,
    int LSpeechPosition,
    string LSpeechValueId);
