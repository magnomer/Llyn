# CStateValue.cs

## `public sealed record CStateValue(string CStateValueText, bool CStateValueUncertain)`

A written value, as a driver shows it with its state.
It carries the engine's verdicts, so a driver never judges the state itself.

**Parameters**

- `CStateValueText`: the text to show, empty when nothing legible is written.
- `CStateValueUncertain`: whether the value is marked unknown.

## `public static CStateValue CStateValueEmpty`

The value of a field nothing is written in.

## `public string? CStateValueShown`

The text, or null when it is empty.
