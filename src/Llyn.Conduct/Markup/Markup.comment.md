# Markup

This folder holds the shapes of a markup import.
It stands apart from the engine's import, because a driver and the customs gate receive ready values.

## `CMarkupEntry`

One entry a markup file carries, as the import dialog lists it before anything is stored.
The dialog needs only what it shows, so its targets arrive found.
The targets are the ids of the stored entries it may join, found by the engine before the question.

## `CMarkupOmission`

One thing a markup import left behind, as the import report lists it.
Both fields arrive as ready text, so a driver only shows them.
The line is written in the current culture, and zero when unknown.

## `CMarkupTarget`

One stored entry an imported entry may join, as the customs gate holds it.
No driver reads it, since the gate turns it into a row's ready loss.
Its card counts arrive ready, so the gate words a replacement's loss without reading the engine.
Both counts include nested cards.

## Stamp

Commit: `c2f335c331fe`
- `CMarkupEntry.cs` `67623e63568aa0d1`
- `CMarkupOmission.cs` `54cc1b1f6e97dba1`
- `CMarkupTarget.cs` `48ad98607e66bf96`
