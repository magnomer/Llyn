# LLecternCard.cs

## `public sealed class LLecternCard`

The reading view's card deportment, standing between the veneer and [LDisplay](../../Llyn.Conduct/Display/LDisplay.comment.md).
It draws the Meaning and Collocation cards, their links and bylines, the incoming rows and the etymology.
It answers every chip, row, source link and word clicked on them.
The veneer hands over its converters and its navigation as seams, so no veneer type is named here.

## `public void LLecternCardAttach(LWindow window, ResourceDictionary resources, ItemsControl meaning, UIElement meaningSection, ItemsControl collocation, UIElement collocationSection, ScrollViewer contents, LCompass compass)`

Holds the window for the pack's typography and the view's resources the example templates read.
Holds the two card lists with their sections, the scroll viewer and the compass a scroll measures by.

## `public void LLecternLinkAttach(Action<IReadOnlyList<LTranslationTarget>> translationSeam, Action<IReadOnlyDictionary<long, string>> citationSeam, Action<LSentenceOrder> orderSeam)`

Holds the seams that fill the converters the card templates bind through.
`translationSeam` names the linked headwords, `citationSeam` the bylines, `orderSeam` the sentence order.

## `public void LLecternIncomingAttach(ItemsControl incoming, UIElement section)`

Binds the incoming list to its rows and holds the section that collapses when no entry links here.

## `public void LLecternEtymologyAttach(UIElement etymology, UIElement section, Action<string, string, IReadOnlyList<LTranslationTarget>> etymologySeam)`

Holds the etymology field, its section and the seam that hands the field its language, narrative and links.

## `public void LLecternRouteAttach(Func<long, bool> entrySeam, Func<long, bool> situationSeam, Func<long, bool> registerSeam, Func<long, bool> tagSeam, Action<string, Exception> failSeam)`

Holds the seams a click opens a record through, one per panel.
`failSeam` is subscribed to `LDisplayFailed`, so a refused word lookup reaches the window's notice.

## `public void LLecternCardShow()`

Draws the shown draft's cards, incoming rows and etymology.
The converters are filled before the card lists are handed their items.
A converter holding a dictionary says nothing when that dictionary changes.
So the lists are emptied and handed their items again once the words behind the ids are known.
Example lines are drawn inside templates, so the pack's example typography goes into the view's resources.
Each incoming row names its referring Meaning or Collocation in the reader's words.
The etymology field shows only with a narrative or a link, and its section only for a derived entry.
With no draft shown it clears instead.

## `private void LLecternCardDraw(LEntryDraft draft)`

Fills the converters and the example typography, then hands the card lists their items.

## `private void LLecternIncomingShow(IReadOnlyList<LUsage> usages)`

Lists one incoming row per usage, its owner named in the reader's words.
The section collapses when no entry links here, because an empty relationship does not occupy the page.

## `private void LLecternEtymologyShow(string language, string text, IReadOnlyList<LTranslationTarget> etymons, bool derived)`

Hands the field its language, narrative and links, and sets the field's and the section's visibility.

## `public void LLecternCardClear()`

Empties the converters, the card lists and the incoming rows, and collapses their sections.

## `public void LLecternCardHandle(RoutedEventArgs e)`

Every chip on a card names a record kept in some other panel, and the record decides the panel.
A situation opens the repertoire, a register the tenor, a translation the library, a tag the taxonomy.
The chip is read off what was clicked, since a click leaving a template is re-sourced to its presenter.
A chip naming a record the card never saved leads nowhere and leaves the click unhandled.

## `public void LLecternIncomingHandle(object sender)`

Opens the entry that carries the clicked row, because that is where such a link is edited.

## `public void LLecternEtymonHandle(object parameter)`

Opens the entry a source chip names, as a translation link does.

## `public void LLecternMentionFind<LLecternAnchor>(`

Forwards a clicked word to the display, which asks the engine what stands there.
The display hands a found answer to `show` with `anchor`, and a failed lookup calls nothing.

## `public void LLecternCardScroll(long id)`

Scrolls the card with the given id into view and plays the spotlight on it.
The card containers exist one dispatcher turn after the entry is shown, so the scroll waits for the layout pass.
The compass scrolls it, so the card is led by the same distance as a row.
A card the entry no longer has is left unfound, and nothing moves.

## `private static FrameworkElement? LLecternCardFind(ItemsControl cards, long id)`

The container drawn for the card with `id`, or null when the list holds no such card.
