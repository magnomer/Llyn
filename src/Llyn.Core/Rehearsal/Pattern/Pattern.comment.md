# Pattern

This folder holds the records and pure rules of deck stencils: the stencil, its slots, fields, body and tokens.
A deck is studied on a phone flashcard app, and Llyn writes the deck file that app reads.
Every record is positional and immutable, so a change replaces the record whole.
Every rule here reads only its arguments, so nothing here reads the clock.

## `LStencilField`

The lexical values a template slot can name.
A slot body names them as text keys, so a stored body never depends on these numbers.

## `LStencil`

A side template.
A saved template is a reusable copy, and every other template belongs to one deck.

## `LStencilSlot`

One column of a template, written as a body of literal text and field tokens.

## `LStencilDefault`

The slots a new deck's own template starts with, chosen by the deck's grain.
Side 1 holds what the grain asks, and side 2 holds its answer.
- Entry asks the headword and answers every definition.
- Meaning asks the headword and answers its definition.
- Collocation asks the expression and answers its paraphrase.
- Example asks the sentence and answers its gloss.
- Pair asks the meaning's headword and answers the target headword.

Picture and Sound sit on side 2, with the answer.
So the side shown first carries no hint of the answer.
A free grain has no default, since a free cue's cells are its own.

## `LStencilToken`

One piece of a slot body.
A literal token carries text and no field.
A field token carries the field, plus either an index or the all mark.
A field token with neither means the field's first value.

## `LStencilTokenParse`

Splits a slot body into literal runs and field tokens.
A field token is written `{key}`, `{key:n}` or `{key:all}`.
Keys ignore case and surrounding space.
The index counts from one.
An unknown key, a bad suffix or an unclosed brace stays literal text.
So a typo shows on the card instead of failing the export.
Literal pieces next to each other join into one token.

## `LStencilTokenFormat`

Writes a token back as slot body text.
A field token with an unknown field throws, since no key names it.

## `LStencilBody`

Edits a slot body as a list of field chips, so the chip editor never splices text itself.
A place counts field tokens from zero and skips literals.
A place with no field token throws.

## `LStencilBodyAppend`

Adds a plain field token on a new line.
An empty body, or one already ending in a line break, takes the token with no extra break.

## `LStencilBodyRemove`

Drops the field token at a place.
It also drops the line break beside the token, the one before it first.
So removing a chip leaves no blank line.

## `LStencilBodySet`

Sets the field token at a place to its first value or to all values.
Any index on the token is dropped.

## `LStencilBodyCheck`

Tells whether a body is simple enough for chips.
Every token must be a field with no index, and the only literals are line breaks.

## Stamp

Commit: `2f9a06b8eb95`
- `LStencil.cs` `03806030470bf085`
- `LStencilBody.cs` `3da5cde79b844449`
- `LStencilDefault.cs` `782134305c234e34`
- `LStencilField.cs` `9559807691aa972b`
- `LStencilSlot.cs` `da6dcf2601148958`
- `LStencilToken.cs` `4d142f9e9c694627`
