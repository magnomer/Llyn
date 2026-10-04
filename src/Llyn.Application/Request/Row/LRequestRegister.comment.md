# LRequestRegister.cs
Hash: `89dc8d8bedcdd41f`

The register requests, shaped like the situation requests.
A register is a shared row, so its name is edited by row id and not by card.

## `public sealed record LRequestRegisterAddition(long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue, int LRequestPosition)`

Adds a new register with the typed name at `LRequestPosition`.
The engine mints its id, and commit creates the row in the entry's language.

## `public sealed record LRequestRegisterPick(long LRequestDraftId, long LRequestCardId, long LRequestRegisterId, int LRequestPosition)`

Links an existing register to the card at `LRequestPosition`, copying the stored row under its positive id.

## `public sealed record LRequestRegisterRemoval(long LRequestDraftId, long LRequestCardId, long LRequestRegisterId)`

Unlinks one register from the card.

## `public sealed record LRequestRegisterShift(long LRequestDraftId, long LRequestCardId, long LRequestRegisterId, int LRequestPosition)`

Moves one register chip to `LRequestPosition` inside its card.

## `public sealed record LRequestRegisterName(long LRequestDraftId, long LRequestRegisterId, LStateWritten LRequestValue)`

Replaces the name of the register, wherever the draft holds it.
