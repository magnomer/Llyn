using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// One card of the input form, captured as an immutable value. A Meaning card and a Collocation card
/// are the same card — Title, a meaning text, Example, Situation, Synonym and Tags — so they are the
/// same value here, and the Collocation's own Expression field is the single member a Meaning card
/// leaves empty. A field added to the form is therefore added once.
/// <para>
/// A draft is what the shell hands the engine before anything is persisted: it carries the typed text
/// exactly as it stands on screen and nothing else — no identity, no state, no database concern. Empty
/// fields stay empty strings rather than <c>null</c>, because "nothing was typed" is what the form
/// means. The card's position is not carried: it is the order of the list the card sits in, which the
/// save reads off that order and the load writes back into it.
/// </para>
/// <para>
/// Example, Situation and Tag are ordered sets, because the store models them as many-per-card: a card
/// may reference any number of each, and the order of the list is the order they are stored in and read
/// back. A field the form leaves empty is an empty list, never a list holding an empty string. Turning
/// what a control holds into such a list is the shell's job — the engine takes the list as given and
/// never splits text into rows itself. Two cards holding equal lists are not equal values, because the
/// generated equality compares those lists by reference; a comparison that means "the same card" walks
/// the lists itself.
/// </para>
/// </summary>
/// <param name="LCardDraftTitle">Title text from the card's Title field.</param>
/// <param name="LCardDraftExpression">
/// Expression text from a Collocation card's Expression field; always empty for a Meaning card, whose
/// template has no Expression control.
/// </param>
/// <param name="LCardDraftMeaning">
/// The card's meaning text: the Definition field of a Meaning card, the Meaning field of a Collocation
/// card. One field, labelled differently on the two templates.
/// </param>
/// <param name="LCardDraftExample">Example texts the card references, in the order they are shown.</param>
/// <param name="LCardDraftSituation">Situation titles the card references, in the order they are shown.</param>
/// <param name="LCardDraftSynonym">Synonym text from the card's Synonym field.</param>
/// <param name="LCardDraftTag">Tag texts the card references, in the order they are shown.</param>
public sealed record LCardDraft(
    string LCardDraftTitle,
    string LCardDraftExpression,
    string LCardDraftMeaning,
    IReadOnlyList<string> LCardDraftExample,
    IReadOnlyList<string> LCardDraftSituation,
    string LCardDraftSynonym,
    IReadOnlyList<string> LCardDraftTag);
