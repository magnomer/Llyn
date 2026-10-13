# Fanqie

This folder holds the rime-book block of the stored entry and the fanqie readings of one character in one book.

## `CFanqieGroup`, `CFanqieRow`

The fanqie readings of one character in one book.
The controller copies every derived cell, so the rime tables stay in the engine.

## `CSoundingFanqie`

The rime-book block of the editor for the stored entry.
It carries its groups, a pending flag, a rebuildable flag and the pack's glyph font.

## Stamp

Commit: `c2f335c331fe`
- `CFanqieGroup.cs` `e29afd04aaf88d65`
- `CFanqieRow.cs` `fb5891e650e64f48`
- `CSoundingFanqie.cs` `93987be7b770776b`
