# LRequestRegister.cs

The register requests, shaped like the situation requests.
A register is a shared row, so its name is edited by row id and not by card.

## `public sealed record LRequestRegisterAddition(`

Adds a new register with the typed name at `LRequestPosition`.
The engine mints its id, and commit creates the row in the entry's language.

## `public sealed record LRequestRegisterPick(`

Links an existing register to the card at `LRequestPosition`, copying the stored row under its positive id.

## `public sealed record LRequestRegisterRemoval(long LRequestDraftId, long LRequestCardId, long LRequestRegisterId)`

Unlinks one register from the card.

## `public sealed record LRequestRegisterShift(`

Moves one register chip to `LRequestPosition` inside its card.

## `public sealed record LRequestRegisterName(long LRequestDraftId, long LRequestRegisterId, LStateValue LRequestValue)`

Replaces the name of the register, wherever the draft holds it.
