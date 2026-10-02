# QLecternCard.cs

## `public sealed class QLecternCard`

The reading view's card driver, standing between the veneer and the display's area, [CDisplay](../../Llyn.Conduct/Display/CDisplay.comment.md).
It draws the Meaning and Collocation cards, their links and bylines, the incoming rows and the etymology.
It hears every chip, row, source link and word clicked on them and hands each to one gate.

## `public QLecternCard(LDisplay display)`

Builds the half over the display, whose area answers every read and gate.
The card lists take the ready cards the area answers, so no engine record reaches the veneer.

## `public void QLecternCardIntroduce(ResourceDictionary resources, ItemsControl meaning, UIElement meaningSection, ItemsControl collocation, UIElement collocationSection, ScrollViewer contents, QCompass compass)`

Holds the view's resources the example templates read.
The pack's typography comes from the display's sound area, which reads the shown language itself.
Holds the two card lists with their sections, the scroll viewer and the compass a scroll measures by.

## `public void QLecternIncomingIntroduce(ItemsControl incoming, UIElement section)`

Binds the incoming list to its rows and holds the section that collapses when no entry links here.

## `internal void QLecternEtymologyIntroduce(QEtymology etymology, UIElement section)`

Holds the etymology field and its section, which this driver paints itself.

## `internal void QLecternRouteIntroduce(QWindow host)`

Holds the window, whose navigation a clicked incoming row opens its record through.
The window also paints the menu a clicked word's gate answers.

## `public void QLecternCardRefine()`

Answers an entry opening by painting the ready cards `CDisplayCardRead` answers.

## `public void QLecternExampleRefine()`

Answers an entry opening by putting the shown language's example typography into the view's resources.
Example lines are drawn inside templates, so the typography reaches them through resources.
It is subscribed before the cards, so the example lines draw in the pack's faces.

## `public void QLecternGlossRefine()`

Answers an entry opening the same way for the gloss typography under the example lines.

## `public void QLecternIncomingRefine()`

Answers an entry opening by listing the usages `CDisplayIncomingRead` answers.

## `public void QLecternEtymologyRefine()`

Answers an entry opening by drawing the etymology `CDisplayEtymologyRead` answers.

## `public void QLecternBlankRefine()`

Answers an entry closing by emptying the card lists and the incoming rows and collapsing their sections.

## `private void QLecternCardRefine(CLecternCard card)`

Sets the two card lists and the two sections' visibility from `card`.

## `private void QLecternLeafRefine(CLecternCard card)`

Hands the card lists the ready cards of `card`.
The lists are emptied first, so every template redraws even when a list keeps its length.

## `private void QLecternIncomingRefine(IReadOnlyList<CUsage> usages)`

Lists one incoming row per usage in its Conduct shape, its owner named under the key Conduct chose.
The section collapses when no entry links here, because an empty relationship does not occupy the page.

## `private void QLecternEtymologyRefine(CLecternEtymology etymology)`

Sets the field's narrative and source links from the ready `etymology`.
It hands the field the ready verdicts for its read narrative and its row of links.
Then it sets the field's and the section's visibility.

## `public void QLecternChipObserve(RoutedEventArgs e)`

Hears a click on a card chip and hands `CDisplayChipOpen` the ready chip it carries.
The chip is read off what was clicked, since a click leaving a template is re-sourced to its presenter.
A link chip also hands the id of the ready target it paints.
The click is marked handled only when a tab was asked to open.

## `public void QLecternIncomingObserve(RoutedEventArgs e)`

Hears a click on an incoming row and hands its usage to the navigation's gate `CNavigationUsageOpen`.

## `public void QLecternEtymonObserve(object parameter)`

Hears a click on a source link and hands `CDisplayChipOpen` the entry id it carries.

## `public void QLecternMentionObserve(PMentionArgument e)`

Hears a word clicked in an example line and hands its sentence row and offset to `CDisplayMentionFind`.
The gate reads the text itself, finds, opens and reports.
Its offer goes to the window's `QWindowMentionRefine` under the clicked control.

## `public void QLecternEtymologyObserve(PMentionArgument e)`

Hears a word clicked in the etymology prose and hands its offset to `CDisplayEtymologyFind`.
The offer goes to the window's `QWindowMentionRefine` under the prose.

## `public void QLecternSpotlightRefine(long id)`

Scrolls the card with `id` into view and plays the spotlight on it.
The card containers exist one dispatcher turn after the entry is shown, so the scroll waits for the layout pass.

## `private void QLecternSpotlightRefine((CCompassPart, int)? place)`

Finds the container at the place `CDisplayCardFind` answered and lets the compass scroll it.
The compass scrolls it, so the card is led by the same distance as a row.
A card the entry no longer has is left unfound, and nothing moves.
