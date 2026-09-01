namespace Llyn.Core;

/// <summary>
/// One collocation card of the input form, captured as an immutable value. It carries <em>both</em>
/// <see cref="LCollocationDraftExpression"/> and <see cref="LCollocationDraftMeaning"/>: the card has
/// an Expression field of its own, and its Meaning field is the definition analogue of a sense card.
/// <para>
/// <see cref="LCollocationDraftSynonym"/> exists because <c>DesignUI-Input.md</c> specifies a
/// collocation synonym, but the collocation card template has no Synonym control, so the reader always
/// leaves it empty. That is expected, not a defect.
/// </para>
/// </summary>
/// <param name="LCollocationDraftPosition">One-based position of the card in the collocation list.</param>
/// <param name="LCollocationDraftExpression">Expression text from the card's Expression field.</param>
/// <param name="LCollocationDraftMeaning">Meaning text from the card's Meaning field.</param>
/// <param name="LCollocationDraftExample">Example text from the card's Example field.</param>
/// <param name="LCollocationDraftSituation">Situation text from the card's Situation field.</param>
/// <param name="LCollocationDraftSynonym">Synonym text; always empty until a control feeds it.</param>
/// <param name="LCollocationDraftTag">Tag text from the card's Tags field.</param>
public sealed record LCollocationDraft(
    int LCollocationDraftPosition,
    string LCollocationDraftExpression,
    string LCollocationDraftMeaning,
    string LCollocationDraftExample,
    string LCollocationDraftSituation,
    string LCollocationDraftSynonym,
    string LCollocationDraftTag);
