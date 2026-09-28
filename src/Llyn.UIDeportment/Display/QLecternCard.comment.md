# QLecternCard.cs

## `public sealed class QLecternCard`

The reading view's card driver, standing between the veneer and the display's area, [CDisplay](../../Llyn.Conduct/Display/CDisplay.comment.md).
It draws the Meaning and Collocation cards, their links and bylines, the incoming rows and the etymology.
It hears every chip, row, source link and word clicked on them and hands each to one gate.
The veneer hands over its converters as seams, so no veneer type is named here.

## `public QLecternCard(LDisplay display)`

Builds the half over the display, whose area answers every read and gate.
The card lists still take the shown draft's cards, since the card templates draw engine cards until job66.

## `public void QLecternCardIntroduce(CAtelier atelier, ResourceDictionary resources, ItemsControl meaning, UIElement meaningSection, ItemsControl collocation, UIElement collocationSection, ScrollViewer contents, QCompass compass)`

Holds the window for the pack's typography and the view's resources the example templates read.
Holds the two card lists with their sections, the scroll viewer and the compass a scroll measures by.

## `public void QLecternLinkIntroduce(Action<IReadOnlyList<CTranslationTarget>> translationSeam, Action<IReadOnlyDictionary<long, string>> citationSeam, Action<CSentenceOrder> orderSeam)`

Holds the seams that fill the converters the card templates bind through.
`translationSeam` names the linked headwords, `citationSeam` the bylines, `orderSeam` the sentence order.

## `public void QLecternIncomingIntroduce(ItemsControl incoming, UIElement section)`

Binds the incoming list to its rows and holds the section that collapses when no entry links here.

## `public void QLecternEtymologyIntroduce(UIElement etymology, UIElement section, Action<string, string, IReadOnlyList<CTranslationTarget>> etymologySeam)`

Holds the etymology field, its section and the seam that hands the field its language, narrative and links.

## `internal void QLecternRouteIntroduce(PWindow host)`

Holds the window, whose tabs a clicked chip, row or source link opens its record in.

## `public void QLecternCardRefine()`

Answers an entry opening by filling the converters from `CDisplayCardRead`.
The converters are filled before the card lists are handed their items.
A converter holding a dictionary says nothing when that dictionary changes.
Example lines are drawn inside templates, so the pack's example typography goes into the view's resources.

## `public void QLecternLeafRefine()`

Answers an entry opening by handing the card lists the shown draft's cards again.
The lists are emptied first, so the templates redraw with the converters just filled.
The cards are engine records, which job66's card state replaces.

## `public void QLecternIncomingRefine()`

Answers an entry opening by listing the usages `CDisplayIncomingRead` answers.

## `public void QLecternEtymologyRefine()`

Answers an entry opening by drawing the etymology `CDisplayEtymologyRead` answers.

## `public void QLecternBlankRefine()`

Answers an entry closing: empties the converters, the card lists and the incoming rows, and collapses their sections.

## `private void QLecternCardRefine(CLecternCard card)`

Fills the converters, the example typography and the two sections' visibility from `card`.

## `private void QLecternIncomingRefine(IReadOnlyList<CUsage> usages)`

Lists one incoming row per usage in its Conduct shape, its owner named under the key Conduct chose.
The section collapses when no entry links here, because an empty relationship does not occupy the page.

## `private void QLecternEtymologyRefine(CLecternEtymology etymology)`

Hands the field its language, narrative and links, and sets the field's and the section's visibility.

## `public void QLecternChipObserve(RoutedEventArgs e)`

Hears a click on a card chip and hands `CDisplayChipOpen` what the chip carries, with the window's tabs.
The chip is read off what was clicked, since a click leaving a template is re-sourced to its presenter.
A link chip also hands the entry id it names.
The click is marked handled only when a tab was asked to open.

## `public void QLecternIncomingObserve(RoutedEventArgs e)`

Hears a click on an incoming row and hands its usage's own gate, `CUsageOpen`, the window's tabs.

## `public void QLecternEtymonObserve(object parameter)`

Hears a click on a source link and hands `CDisplayChipOpen` the entry id it carries.

## `public void QLecternMentionObserve<QLecternAnchor>(`

Hears a clicked word and hands it to `CDisplayMentionFind`.
The answer goes to `show` with `anchor`, and a failed lookup shows nothing.

## `private static void QLecternMentionRefine<QLecternAnchor>(QLecternAnchor anchor, CMentionResult? result, Action<QLecternAnchor, CMentionResult> show)`

Hands a found answer to `show`, and nothing when the lookup found no answer.

## `public void QLecternSpotlightRefine(long id)`

Scrolls the card with `id` into view and plays the spotlight on it.
The card containers exist one dispatcher turn after the entry is shown, so the scroll waits for the layout pass.

## `private void QLecternSpotlightRefine((CCompassPart, int)? place)`

Finds the container at the place `CDisplayCardFind` answered and lets the compass scroll it.
The compass scrolls it, so the card is led by the same distance as a row.
A card the entry no longer has is left unfound, and nothing moves.
