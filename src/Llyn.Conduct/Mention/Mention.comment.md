# Mention

This folder holds the mention gates.
It stands apart from the engine, because a driver hands raw click values and receives ready pieces, chips and menus.

## `CMention`

The gates for how a text divides into Mention pieces, and how its chips are named.
It converts click units to code points for the find gates, and a found word's start back to a unit.
It stands on the atelier's ports, and `CAtelier` builds the one instance every driver shares.
It holds no state of its own.
Its click gate opens what a found word names through the atelier's navigation.

## `CMentionSenseChosen`

Raised with a stored Mention's sense once its entry has opened.
The driver then brings that sense card into view.

## `LMentionResultOpen`

Decides what a click on a text opens next, for every find gate alike.
A stored Mention opens its linked entry through the navigation's entry open.
A link to nothing opens nothing.
Its sense is raised only when the open went through, so a declined leave spotlights nothing.
A single matching entry opens at once.
Several matching entries open nothing and come back as candidates for the driver's menu.
Every other answer offers nothing.
The offer carries the menu's title key `Mention.Title` and the place the menu stands on.
That place is the found word's start, so the menu hangs under the whole word and not under the click.
It is a UTF-16 unit in the whole shown text, ready for the control.
So a word that starts in an earlier piece than the click still places the menu on its start.

## `CMentionDivide`

The whole text as pieces, so a driver draws every character and no gap splits into words.
A linked piece answers true, a silent piece false, and a gap nothing.
The corpus excerpt divides its text while it maps it.

## `LMentionPieceRead`

Maps each engine piece to its text and its link state, leaving its offset behind.
The text division maps its pieces here.
The reading card maps the pieces its line read answers.

## `LMentionProspectRead`

The mention picker's interaction, shared by the editor and the corpus transcript.
Each area passes its own desk, so no driver chooses which draft is searched.
A desk without a live draft offers nothing.
A failed search shows `Mention.FindFailed` and offers nothing, so no picker opens.

## `LMentionSenseRead`

The sense menu for a selection, shared by the editor and the corpus transcript.
It is titled by the key `Mention.Sense`.
Its first row picks the whole Entry, worded by `Mention.Whole`, and the Meanings follow.
A selection inside no linked Mention, or a desk without a live draft, answers nothing.
A failed read shows `Mention.FindFailed` and answers nothing, so no menu opens.

## `LMentionChipRead`

The chip line of one Example of a desk's draft, ready to paint, keyed by card and sentence.
A desk without a live draft, or an Example with no Mentions, answers no chips.
A failed read shows `Mention.FindFailed` and answers no chips.

## `LMentionUnitRead`

The UTF-16 unit inside one piece where a code-point offset starts, for placing a popup.
An offset outside the piece answers null.
Only the result open calls it, with the whole text as one piece at zero.
So the offer reaches the driver with its unit ready.

## `LMentionOffsetRead`

The code-point offset of a click in the whole shown text.
It is the inverse of the unit read at a start of zero.
Only the find gates call it, so a driver hands raw click values and composes no offset.

## `CMentionSpanCheck`

Whether a field's selection spans any code point, so a link command may run.
A driver asks this rather than measuring the span itself.

## `CMentionRead`

The links of stored Mentions, for the found Mention a click result carries.
It stays internal, since it names engine types.

## `CMentionResultRead`

Maps what a click on a text found to its shape, with a stored Mention under the click.
The corpus excerpt and the lectern both hand the window this shape.

## `LMentionLineRead`

The chip lines of the draft's sentence rows, keyed by line key, as the sentence area answers them.
The engine mints each key through the draft port, as the position among the entry's sentences.
The shown entry gives the keys, and the draft's content gives the labels, matched by card and sentence row.
It keys one entry for every sentence of the meaning and collocation cards.
A sentence the engine sent no line for gets an empty list, so a missing key never means stale.
A line for a key matching no sentence is dropped, since no row shows it.

## `LMentionLabelRead`

Copies the engine's labels field for field, with no rule of its own.
The label's own key names an unlinked chip, so no silent word is written here.

## `CMentionLabel`

One chip of a mention line, labelled by the engine.
A linked chip shows its headword as the engine named it.
`CMentionLabelKey` is the key a driver looks up in place of the name.
It is chosen for a Mention that stands for nothing.

## `CMentionMark`

The link of the stored Mention a click found, which the mention area opens.
It keeps only the entry and the sense, since the engine already settled the span.
The entry is zero when unlinked, and the sense is zero when none.

## `CMentionOffer`

What a word click leaves for the menu, once the gate has opened what it opens at once.
It carries the menu's place as a ready surface value, so the control converts no offset.
The unit is the UTF-16 unit in the whole shown text where the found word starts.
The menu hangs under it, whichever run of the control holds that unit.
It is null when the found start lies outside the text, so the menu hangs at the control's bottom left.
The entries are empty when the click opened one or found none.
An empty offer still closes an open menu.

## `CMentionPiece`

One run of a mention text, cut by the engine at the Mentions.
The linked flag is null for plain text.

## `CMentionResult`

What a click on a text finds at the clicked place.
The stored Mention is null when none is there.
The entries are those whose headword matches the clicked word.
`CMentionResultSingle` means exactly one entry matches, so the click opens it at once.
`CMentionResultMany` means several match, so the click offers a menu.
`CMentionResultFirst` is the one matching entry, zero unless exactly one matches.

## `CMentionSense`

The sense menu of a linked Mention, ready to show.
The rows come in the order shown, the whole Entry first with id zero.
The Meanings follow in reading order.

## Stamp

Commit: `44d0e95bcfcc`
- `CMention.cs` `3978a6853539422a`
- `CMentionLabel.cs` `f76e9d4f43ab824e`
- `CMentionMark.cs` `e508634d8bf3ccb9`
- `CMentionOffer.cs` `1cc7318417f1a177`
- `CMentionPiece.cs` `6094ea24cbc39f8d`
- `CMentionResult.cs` `b6fa25e7cd5757cf`
- `CMentionSense.cs` `e8041d6157173d41`
