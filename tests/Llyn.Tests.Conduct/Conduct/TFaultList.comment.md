# TFaultList.cs
Hash: `e993e6b5885af5a2`

## `internal static class TFaultList`

The list-load half of the task fault sweep, holding one row per panel row load.
The rows join `TFault.TFaultRows` there, so the sweep runs them beside every other gate.

## `internal static IReadOnlyList<TFaultRow> TFaultListRows =>`

A property, not a field, so the sweep's list reads it whatever the type initialisation order.
Panel row loads all await the settings port's flag load before they read their rows.
Their rows therefore fault `LSettingsPort.LEngineEnsignLoad` and expect the panel's own load-failed key.
Footnote, occurrence and quotation lists are reached through the shelf, repertoire and corpus that own them.
Each panel is built on `TFault.TFaultAtelierStart`, so only the stage's member faults.
