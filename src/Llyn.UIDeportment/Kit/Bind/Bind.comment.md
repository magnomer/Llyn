# Bind

This folder holds baseline conversion, event value readers and wording property helpers.
The helpers keep framework extraction and localized wording application apart from individual view handlers.
They own no per-view state.

## `QFontConverter`

Baseline alignment needs both faces and sizes because equal top positions do not align different font baselines.
A negative drop is clamped to zero, so this converter never lifts the text.
Malformed input yields no margin, while reverse conversion is unsupported.

## `QSender`

Typed extraction answers null when a sender, original source or command parameter does not carry the requested value.
The generic readers accept any reference type, rather than requiring a veneer value.
Reading the original source keeps a bubbled click's item separate from the list that receives it.
The focus reader uses the currently focused element rather than the event sender.
Only Enter, Escape, Down and Up cross the key reader as words.

## `QStateConverter`

Conduct supplies the wording choice, so the helper only resolves its key or uses its literal text.
The hint uses an empty string when no hint key is supplied, clearing any previous placeholder.

## Stamp

Commit: `c2f335c331fe`
- `QFontConverter.cs` `6cc60d44961149aa`
- `QSender.cs` `052f89559a071f0d`
- `QStateConverter.cs` `1a6bf847c5428ade`
