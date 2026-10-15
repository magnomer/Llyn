# Gloss

Gloss holds the Gloss rows and the language picker.
A Gloss is a rendering in another language, and its row serves both the card editor and the corpus panel.
The row is shared so one template and one flag resolver serve every place a Gloss shows.

## `QRendition`

This is one Gloss row: the language it is written in and its flag.
The text is the engine's value, state and all, and the converter reads its mark.
The row raises a notice for a language pick and lets its owner hand it to the gate.
What is typed into its text field leaves through the owner's own handler.
So the row holds no copy of it.
The language is set only by the draft, so a pick shows once the gate's change comes back.
A negative id marks a row the store has not written.

### `QRenditionShow`

A field already reading what the engine holds is left alone.

### `QRenditionRowApply`

One fill serves the card row, the field, the display and the corpus line.
So a part the template lacks is skipped.
The flag shows the language's flag, and the ring stands in while there is none.
The picker selects by language name, a path set here so the markup holds none.
Selection handlers are off while the fill sets the selection, so only a pick raises the notice.

### `QRenditionTextApply`

The keys are read once per fill, so they do not follow a later language switch.

### `QRenditionPickRaise`

A null selection, which the list shows while the catalog reloads, raises nothing.
The editor hears the notice through the sentence row, and the corpus hears the list itself.

### `QRenditionListRefine`

A pick shuts the picker, but the shut waits for the dispatcher.
It refills the row and would reset the selection, so every handler of the pick reads the picked language first.

## `QGlossCommand`

The routed removal command is bound by whichever surface hosts the row.
The card editor and the corpus panel bind their own handlers, so one template serves both.

## `QGlossConverter`

The converter turns the Glosses a read-only card carries into rows it can show.
A card stores each Gloss as language and text alone, and the display needs the flag beside it.
So each Gloss is wrapped in the same row the editor uses, which resolves its own flag.
One resolver for both modes keeps a Gloss identical whichever mode shows it.
A value that is not a Gloss list shows no rows.

### Inline notes

### QGlossConverter.cs: `private static readonly ObservableCollection<QLanguageItem> QGlossConverterCatalog = []`

The row asks for a catalog because the editor lets a Gloss change language.
The display never opens that picker, so every row shares one empty catalog.

## `QLanguageItem`

This is one row of the `QSpeaker` dropdown, with the language name and its flag image.
The flag is null when the pack declares none, and the row then shows no flag.
It is resolved once when the list is built, so the menu paints without a per-row download.
`PLanguageNameRead` answers empty when the sender carries no row.

## `QGloss`

This class hands the Gloss gestures of a card's sentence row, raw, to one sentence gate.
It has no control of its own to wire.
It holds the sentence driver, which finds the card of a pressed sentence row.
The engine chooses a new Gloss's language and place.

## `QSpeaker`

This is the editor's language pill and its menu of loaded languages.
The pill shows the draft's language, and a pick hands the language to the editor's gate.
It is drawn as the reading view draws it, so one entry reads the same in both.
It keeps its arrow and menu, because here the language is chosen rather than reported.
The rows are the editor's one language list, which every card's Gloss picker also shows.
The menu opens and closes with the pill, where a binding followed its check.

### `QSpeakerLanguageRefine`

The languages are the workspace's, so it answers each opened workspace.
It repaints the pill's flag and the card links' flags once their images are in.
A failed load is reported by the catalog and answers no languages, so the menu empties.

### `QSpeakerEnsignRefine`

It paints from the drawings already in the store, so it asks for no second fill.

### `QSpeakerChoiceRefine`

It unchecks the pill, so the menu closes once a language is picked.

## Stamp

Commit: `2f9a06b8eb95`
- `QGloss.cs` `29e3b6ba402afc04`
- `QGlossCommand.cs` `f1f508e8d22a6c14`
- `QGlossConverter.cs` `807ab4503b57cfbf`
- `QLanguageItem.cs` `d39f5081980f1ecd`
- `QRendition.cs` `6d261d1d53c0aac0`
- `QSpeaker.cs` `51169bdf8215ad55`
