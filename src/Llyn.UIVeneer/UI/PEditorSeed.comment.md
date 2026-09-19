# PEditorSeed.cs

## `public partial class PEditor`

The seam a browsing panel plants its choice into a fresh entry through.
New pressed while a Tag, Register, Situation, Example or Source is chosen means an entry that carries it.
So the panel resets the form and then hands the chosen id here.
Each seam sends a request naming card zero, which the engine reads as the first Meaning card.
The engine makes that card when the draft has none, and the first sentence row the same way.
The form neither reads the draft back nor picks the card, because the engine holds the truth.

## `internal string PEditorLanguageRead()`

The language the form is standing on, which a row made beside a new entry is given.

## `internal void PEditorTagAdd(long tagId)`

Puts the Tag `tagId` names first on the first Meaning card.

## `internal void PEditorRegisterAdd(long registerId)`

Marks the first Meaning card with the Register `registerId` names.

## `internal void PEditorSituationAdd(long situationId)`

Puts the Situation `situationId` names first on the first Meaning card.

## `internal void PEditorExampleAdd(long exampleId)`

Binds the first sentence row of the first Meaning card to the stored Example `exampleId` names.

## `internal void PEditorReferenceAdd(long referenceId)`

Cites the Source `referenceId` names on the first sentence row of the first Meaning card.
