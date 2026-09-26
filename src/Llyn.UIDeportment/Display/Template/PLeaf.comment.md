# PLeaf.cs

## `internal sealed class PLeaf`

The fills of the display's card templates, which the Veneer returned without a binding.
Each fill finds a named part and sets it from the draft, where a binding read the draft before.
The display holds one, since the converters it carries keep state per shown entry.

## `internal PLinkConverter PLeafLink`

Resolves a card's translation ids to chips, from the targets the lectern's card hands it per entry.
`PLeafCitation` holds the source bylines and `PLeafFrame` the sentence order in the same way.

## `internal void PLeafCardApply(FrameworkElement container, object item, string? _)`

Fills one meaning or collocation card: badge number, title or kind, expression, meaning and body.
A text the state reading leaves empty folds its line away, as the data triggers did.
The body's content control is handed the card, so its template is realized before its rows are filled.

## `private void PLeafBodyApply(FrameworkElement body, LCardDraft card, CultureInfo culture)`

Sets the items of every body row and folds each row that has none.
The translation row takes the wider margin when no situation row stands above it.
The picture and video rows fold by the look sheet, and their lines get the shared media fills.

## `private static void PLeafListApply<PLeafRow>(`

Sets one chip row's items, folds it when empty and attaches its chip fill.

## `private void PLeafSentenceApply(FrameworkElement container, object item, string? _)`

Fills one example line: the frame, the sentence, the byline and the Glosses.
The sentence's top margin drops it to the frame's baseline, computed by `PFontConverter`.
The margin is bound to both fonts, so a font the theme changes later moves the baseline too.
The byline binds the sentence's margin, family and size, so both keep one baseline live.
The Glosses reuse the editor's Gloss row fill, since their parts carry the same names.

## `private static void PLeafSituationApply(FrameworkElement container, object item, string? _)`

Writes a situation chip's title through the state reading.

## `private static void PLeafRegisterApply(FrameworkElement container, object item, string? _)`

Writes a register chip's name through the state reading.

## `private static void PLeafTagApply(FrameworkElement container, object item, string? _)`

Writes a tag chip's text.

## `private static void PLeafLinkApply(FrameworkElement container, object item, string? _)`

Writes a translation chip's flag, headword and language.

## `private static string PLeafStateRead(LStateValue value)`

Reads a three-state value as text, with the localized unknown mark for an uncertain one.
