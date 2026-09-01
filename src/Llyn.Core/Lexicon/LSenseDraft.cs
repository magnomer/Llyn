namespace Llyn.Core;

/// <summary>
/// One sense card of the input form, captured as an immutable value. A draft is what the shell hands
/// the engine before anything is persisted: it carries the typed text exactly as it stands on screen
/// and nothing else — no identity, no state, no database concern. Empty fields stay empty strings
/// rather than <c>null</c>, because "nothing was typed" is what the form means.
/// </summary>
/// <param name="LSenseDraftPosition">One-based position of the card in the sense list.</param>
/// <param name="LSenseDraftDefinition">Definition text from the card's Definition field.</param>
/// <param name="LSenseDraftExample">Example text from the card's Example field.</param>
/// <param name="LSenseDraftSituation">Situation text from the card's Situation field.</param>
/// <param name="LSenseDraftSynonym">Synonym text from the card's Synonym field.</param>
/// <param name="LSenseDraftTag">Tag text from the card's Tags field.</param>
public sealed record LSenseDraft(
    int LSenseDraftPosition,
    string LSenseDraftDefinition,
    string LSenseDraftExample,
    string LSenseDraftSituation,
    string LSenseDraftSynonym,
    string LSenseDraftTag);
