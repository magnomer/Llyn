# Customs

This folder holds the import declaration, its modes, the stored entry it joins and what it left behind.

## `LMarkupOutcome`

What one markup import left behind.
The whole import runs as one revision, so it stores every entry or none.

## `LMarkupMode`

The three ways a parsed entry may enter a workspace.
New stores the entry under the file's headword with a fresh id.
Merge appends the parsed rows to a chosen stored entry, whose headword and existing rows are kept.
Replace keeps the chosen stored entry's id and swaps its body for the parsed one.

## `LMarkupIntake`

The reader's choice for one parsed entry, meaning how it enters the workspace.
The import window builds one per entry and the engine imports under them.
The target is zero for a new entry.
`LMarkupIntakeCreate` drops an earlier target pick to zero under New, since a window may keep a pick across mode changes.

## `LMarkupTarget`

A stored entry a parsed entry may join, sharing its headword and language.
It carries what a replacement would drop, so the import window needs no second read.
Nested cards are counted in both card counts.

## Stamp

Commit: `c2f335c331fe`
- `LMarkupIntake.cs` `9128577e5f2320fc`
- `LMarkupMode.cs` `5d039e8db33a0bac`
- `LMarkupOutcome.cs` `71fcd432309ccb52`
- `LMarkupTarget.cs` `05bdbd40d0434f3b`
