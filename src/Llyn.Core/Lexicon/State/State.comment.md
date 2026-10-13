# State

State holds the three-state distinction every stored field can carry.
It also holds small records of what the web could not fill, notes, owners and deletions.
A field that shows nothing is not one thing.
Nothing may ever have been recorded there, or the user may have marked it as not known.
So the two are never stored, read or shown as the same thing.

## `LLacuna`

One slot of an entry's paradigm the web was asked for and could not fill.
A found form is stored as an inflection, so only the negative answer needs a record of its own.
It is kept in the workspace, so a reopen never asks the web again for a form no source named.
An entry no source could be reached for stores nothing, since not reached is no answer.
A row without a morphology id remains usable when its cell key identifies the missed slot.
A row with neither identifier is ignored.
A missed multi-value cell is named by its key, since one morphology id cannot tell it from its neighbours.
A hand edit of the entry's inflections deletes its lacunae, since the user's word replaces the source's.

## `LNote`

The single Note an entry owns, free text hanging from the entry with no id and no order.
The owning entry id is the identity, so saving a Note replaces whatever Note the entry had.
The text is Markdown in the dialect `LMarkdown` reads.
The engine normalizes it on write, so a stored Note is always in canonical form.

## `LOwner`

Which kind of stored row an id names when a seam takes the referring side of a reference.
An Example, a Situation and a Tag are shared data several kinds of row may point at.
A name may carry three components after its prefix, so a seam cannot spell the owner into its own name.
One method per owner side would read one component too many.
Naming the side as a value keeps one seam per operation.
It makes the set of sides something the compiler checks rather than a spelling convention.
Not every seam accepts every side, since a Tag hangs from a Meaning or a Collocation and nothing else.
A seam handed a side its entity has no association for refuses the request and never guesses at a table.

## `LState`

The three-state distinction a stored value can carry.
Unspecified means never given, Unknown means deliberately marked not known, and Specified means a real value.
Source metadata such as `LReference` uses it.
So does any field that must tell no value yet from known to be absent.

## `LStateAnchor`

A stated link to another row, holding the state and the id it points at.
`LStateValue` carries text a user typed, while this carries an id and never text.
A link slot has two answers, nothing linked or this row linked.
There is no unknown anchor, because the source is not known is itself a row to link.
A workspace starts with a Reference titled Unknown for exactly that, and the user may delete or recreate it.
The id is filled only when the state is specified, which is why it is nullable.
The unreadable flag marks an anchor whose stored state word the engine could not read.
Such an anchor is shown but refused on write until the user clears it.
`LStateAnchorRead` treats a missing id and a zero id alike as unspecified, since SQLite counts from one.
`LStateAnchorShow` gives zero when the anchor points at nothing, so a caller tests one value.
`LStateAnchorNormalize` gives the unspecified anchor when this one is unreadable.
`LStateAnchorEmpty` reads true for an unreadable anchor too, because its state stands unspecified.

## `LStateMark`

A stated field with no payload beside it, holding the three-way state alone.
The author credits of a source are such a field.
The names are rows of `reference_author` and the state is one column.
A bare `LState` could hold that column but could not say when its stored word was unreadable.
An unreadable mark is shown but refused on write until the user clears it.
`LStateMarkNormalize` gives the unspecified mark when this one is unreadable.
`LStateMarkEmpty` reads true for an unreadable mark too.

## `LStateValue`

A stored value together with what is known about it.
Only the specified state carries text.
Every field that can stand empty for either reason carries this rather than a bare string.
An unknown value keeps no text.
The user chose not to give one.
The state is the whole of what is known.
A fourth case is a diagnosis, not a state.
The store may hold a state word the engine cannot read, which makes the value unreadable.
Its state stands unspecified, it keeps the stored text for showing, and it can never be written back.
Only a store reading produces it, never a form.
`LStateValueRead` of plain text treats blank text as nothing recorded.
Nothing read this way is ever unknown, since only the user can mark a value not known.
`LStateValueRead` of a written field gives the unspecified value where the form sent nothing.
So a request never leaves a null.
The implicit conversion from text lets plain text stand where a value is wanted.
Only the writing side converts.
A value never turns back into a string on its own.
So no reader loses the distinction by accident.
`LStateValueShow` is nothing unless the value states text or is unreadable.
`LStateValuePlain` lets a reader above the engine map it plainly, so no layer above keeps its own empty fallback.
`LStateValueEmpty` is false for an unknown value, since something is there and only its text is beyond reach.
`LStateValueLegible` counts an unreadable value and never an unknown one.
`LStateValueFilled` is false only for a value nothing was recorded for and which the store read cleanly.
`LStateValueSound` fails unreadable values and blank text, though `LStateValueLegible` accepts them.
`LStateValueNormalize` loses the stored text of an unreadable value.
That is why only the engine calls it, and only after the user agreed.

## `LStateWritten`

A form field as the user left it.
It is the text in the box and the mark that says not known.
It carries no state, because the shell never decides what a field means.
The shell sends what was written, the engine resolves it, and the shell shows what the engine holds.
`LStateWrittenResolve` is the one rule for the three states, and it runs on the engine side.
The mark decides first, so a marked field is unknown whatever its box holds.
Every editor empties the box when the mark goes on, so nothing written is lost by this order.
An unmarked blank field stands for nothing recorded, and any other text is the value it states.

## `LTombstone`

The record that an Entry was deleted, with which Entry, under which revision, and when.
A tombstone outlives the Entry it names.
Its entry id is recorded text rather than a reference to a live row.
So the history stays readable after the Entry is gone.
One deleted Entry leaves exactly one tombstone.

## Stamp

Commit: `c2f335c331fe`
- `LLacuna.cs` `3e82ebcd61329561`
- `LNote.cs` `4e2d219a289a896f`
- `LOwner.cs` `0903a6d1268d4a0f`
- `LState.cs` `459365b435331ed3`
- `LStateAnchor.cs` `c1b737c1a1e8275d`
- `LStateMark.cs` `f3767a2996426dbe`
- `LStateValue.cs` `26c31c3a21cdded9`
- `LStateWritten.cs` `be3675f813991acd`
- `LTombstone.cs` `4fc6cedac3c38688`
