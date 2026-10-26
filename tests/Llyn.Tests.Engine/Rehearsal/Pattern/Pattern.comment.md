# Pattern

These tests pin the rules and workflows of deck stencils.
Each Core rule test drives one rule through a relay, with no workspace.
The stencil clerk tests save entries in a workspace, since a face reads stored rows.

## `TStencilBody`

Covers the field chip edits of a slot body, with no workspace.
An append writes a token, and puts it on a new line after any text.
A remove keeps the other tokens, and removing the last token leaves an empty body.
A join set writes the all mark or drops it.
The check accepts fields and line breaks only, and refuses literal text or an index.

## `TStencilClerk`

The list read answers saved templates only, never a deck's owned one.
Each field edit saves the changed body into the deck's owned template.
The media move puts a template's picture slot on another side with its body kept.
Covers the build of one card face from a one-slot template.
The join rule joins every value with a bar, and an index picks that value.
A meaning's image fills the picture, and an entry's recording fills the sound.
Both come back as rooted paths.
A pair writes its target while the link stands.
A pair whose link is gone writes an empty target and stays unorphaned.
A video alone fills no picture.
The hand-built meaning and pair cards carry no entry id, as the schema allows only an entry card one.
So they reach their entry through the bound row.
One test per bound grain stores the cue through a new deck and resolves it against that deck's template.
So each proves the holder lookup over a real database and the default template together.

## `TStencilDefault`

Pins the default template of each grain, prompt on side 1 and answer on side 2.
A free grain gets no slots.

## `TStencilToken`

Covers the parse of a slot body.
Literal text and field tokens split apart, and an index or the all mark is read.
An unknown key, a zero index, two suffixes or a stray brace stay literal.

## Stamp

Commit: `c37ca1837ded`
- `TStencilBody.cs` `9acb56c274b8e0e9`
- `TStencilClerk.cs` `5be2930a4634094d`
- `TStencilDefault.cs` `fad4ab50915daad1`
- `TStencilToken.cs` `b8ac3e335ed8a6b0`
