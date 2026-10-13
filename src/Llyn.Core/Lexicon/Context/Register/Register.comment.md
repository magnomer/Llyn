# Register

This folder holds the Register mark in its stored, draft, offer and row shapes.

## `LRegister`

The formality or politeness level a Meaning or Collocation is marked with.
A built-in Register is one a language pack names, and a written Register is one the user typed.
A Register belongs to no language.
Every pack naming the same name names the same Register, and a card in any language marks it.

## `LRegisterDraft`

The id of the Register the row edits and its name.
The draft never owns the name, because the id is the reference.
An empty id means the row has not been stored as a Register yet.
A Register a language pack ships is never renamed by a card.
The card marks it and reads its name back, so the pack stays the authority on what it is called.
Whether the row is one a pack ships is read from the stored Register.
The draft carries no flag for it.

## `LRegisterOffer`

What a register field keeps after a gate, and the stored Registers it offers for the kept text.
The kept text is what remains once the completed names went onto the card.
The offered rows are at most eight, match the trimmed text, and leave out those the card links.
It is shown only when the text is not blank and some stored Register is offered.

## `LRegisterRow`

One stored Register a register field offers, its name already split around the typed word.
A pick links it by id.
The lead is the whole name when the word is not found in it, and the mark is then empty.

## Stamp

Commit: `c2f335c331fe`
- `LRegister.cs` `a8e0ff8840957442`
- `LRegisterDraft.cs` `e261fb66771499c2`
- `LRegisterOffer.cs` `02633798ed7727e1`
- `LRegisterRow.cs` `2151ecf0889e96df`
