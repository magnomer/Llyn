# Lexicon

These tests pin the engine's lexicon seams, mostly against a real workspace.
They cover an entry's favorite mark, grasp, note, parts of speech, unit and field states.
They also cover References and the Authors credited on them.
They stand apart from the other engine folders because they test stored data and its rules, not rendering.
Most facts save through `LEngine`, read back through the entry's load, and compare what returns.

## `TAuthorAbsorbing`

Covers folding one Author into another.
The credits of the dropped Author move to the kept one, at the place the dropped one stood.
A Source crediting both keeps one credit at the earlier place and loses the other.
An Author folded into itself is refused, and so is an id naming no Author.

## `TFavorite`

Covers the engine's favorite seam.
An entry is marked, read back, searched, and unmarked through `LEngine`.
The mark carries a stamp, which is what the favorites panel orders by.
Marking twice must leave one mark on the earlier stamp.
A typed query returns only the marked entries that carry it.
Unmarking leaves the entry itself in place.
The mark goes down with the entry it stands on.

## `TGrasp`

Covers the grasp value and the engine's grasp seam.
An entry is rated, read back and cleared through `LEngine`.
A save raises a bulletin, which is what the reading view re-reads on.
An entry edit must never drop the rating the user gave.
A bad value reaches neither the store nor a subscriber.
A rating for a missing entry throws.
The favorite catalog carries the rating, which a later ordering may sort by.
The star scan turns a rating into full, half and empty stars for the sheet.

## `TMarkdownNote`

Covers the note dialect, which is what the reader makes of note text and what the engine stores.
Two notes that differ only in line ends or trailing blanks must compare equal, else every pause would write.
Each block shape is read once, in order, with its level and its verbatim code kept.
The styles nest and an escaped marker is text.
A lone asterisk or an underscore inside a word must not vanish from a note.
Both the create path and the update path normalize the note, so no path stores a raw note.
Each run of numbered lines is counted from one, and a block outside a run counts zero.
A later list after plain text must restart, or the reader would show a continued count.
A link span carries an address only when it is a web address, and local file links carry none.
The reader opens only what carries an address, so a note cannot launch a local path.

## `TReference`

Covers the engine's bibliographic seams.
A Reference is created, cited by an Example and credited to Authors in order.
Each side refuses a plain delete while something still points at the row.
A credited Author is not deletable until the delete detaches its credits.
The detaching delete the catalog runs lets go of every pointer and the row together.
It leaves every Example and Author it touched.

Neither a Reference nor an Author is deleted by a citation or a credit going.
Both are deliberate data, not something typed into a card.
Usage reads name every card and Example that cites a Reference or sits under a credited Author.
Usage rows sharing a sentence go by language and then id, never by storage order.
Authors sharing a name go by id.
A Reference whose stored author state word is broken reads as unreadable and refuses commit until swept.

## `TSpeech`

Covers the controlled vocabularies and the inflections that use them.
The engine writes parts of speech and morphology into a workspace from the packs on disk.
So the tables are no longer empty on a fresh workspace.
An entry can carry a part of speech from the first save.
A pack row can be rewritten in place, as a second import rewrites it.
An Entry's inflected forms are appended to, moved and deleted through the inflection save.

The pack's values come first in pack order, and typed values follow alphabetically without case.
The typed values are added out of order, so arrival order cannot pass for the rule.
A pack row whose stored position moves to the end still reads in its pack place.
So the stored position never decides what a view shows.
Morphology order is the order the pack lists the values in, and it is stored, not inferred.
The presets are bare roles, and a retired subtype is no value at all.
Valency belongs to the dependence field, not to the part of speech.
A language declaring no morphology is an ordinary language, not a broken pack.
A second engine over the same workspace writes the packs again, so the pack's wording is what the workspace holds.

An entry stores its part of speech as the id, never as the word.
So renaming the preset renames it on this entry too.
The entry reads back the preset's name, resolved through the language's vocabulary.
A part of speech the pack has never heard of is kept as the user wrote it, trimmed.
It resolves against nothing.
One no pack declares replaces a declared one on the same terms.
Clearing the field leaves the entry with no part of speech, not the last one still standing.
Setting the field records a speech change.
The same update made again leaves the field as it stands and records nothing about it.
An unnamed part of speech stays out of the names a reader reads, though the entry still counts as marked.

## `TSpeechRetirement`

Covers the retirement of a part of speech a pack once published.
Its helpers keep the `TSpeech` base, since a retirement is a fact of the speech store.
Most tests write a small fixture vocabulary under a language no pack ships, so the shipped packs never interfere.
It covers a link moved to the head, a duplicate link dropped, and links dropped.
It covers a unit set on linking entries, and a unit the user chose kept.
A second application finds nothing to do.
The shipped English pack is read for its retired rows.
An import keeps a part the user added and drops the retired ones.

## `TState`

Holds the difference between a field nothing was written in and a field holding something that cannot be read back.
The two look alike on a form and must never be stored, loaded, or dropped alike.

Every card field that can stand empty is written with its state.
An unknown title, sentence or situation title is stored as unknown and keeps no text.
An empty expression is stored as nothing recorded.
Both come back that way, and a plain tag survives beside them.
A row nothing was written in refers to nothing and is not stored.
A row holding an unknown sentence still refers to its Example and is kept.

A source that is not known is the seeded Unknown Reference, cited like any other row.
The example table never writes an unknown wording for its source, and a sentence citing nothing stays unspecified.
The unknown wording an older workspace wrote for a source reads as unreadable, since no anchor answers to it.
Migration rewrites the wording before a read like this happens.

Writing over an unknown field states it.
Clearing one says nothing is written there.
A Situation rewritten this way keeps the id it was stored under.

The one state rule maps every pair a form can hand it.
A marked field is unknown and keeps no text.
An unmarked field holding nothing, or nothing but whitespace, is unspecified.
Any other text states itself.
A whitespace-only field is stored, loaded, and mapped again by the same rule.
What comes back maps to what was sent, so the editor holding it is not dirty.

A stored state word the program does not know loads as an unreadable field that shows its stored word.
Saving that draft back is refused, so a damaged field is never silently overwritten.
A draft holding an unreadable field refuses to commit until the sweep drops that field to unspecified.
After the sweep the commit succeeds and writes the field as unspecified.

## `TUnit`

Covers an entry's lexical unit and freedom, kept apart from its parts of speech.
Both survive save and reload, and a markup round trip.
A retired unit in markup is ignored.
The units and freedoms offered are the same in every language.
A language change leaves both as chosen.
The dropdown's second choice of the held value clears it.
The unit and freedom chips lead the speech chips of a Joplin note.
The print sheet writes them in the same chip row as the speeches, with no separate scope line.

## Stamp

Commit: `c37ca1837ded`
- `TAuthorAbsorbing.cs` `2d18dd4c46364a19`
- `TFavorite.cs` `00998332c4948308`
- `TGrasp.cs` `fc971b3834a60e34`
- `TMarkdownNote.cs` `14407d8ac4c9c600`
- `TReference.cs` `9aff50e3983b04ef`
- `TSpeech.cs` `6f08a573cb6494d2`
- `TSpeechRetirement.cs` `9c83a1639d8c4839`
- `TState.cs` `40a7ada7b5110e57`
- `TUnit.cs` `276a7c295c73568e`
