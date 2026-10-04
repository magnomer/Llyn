# TInterfaceConductPortrait.cs
Hash: `bce5987f96fc49d2`

## `internal static class TInterfaceConductPortrait`

The relays for Conduct's portrait maps and its shared export and print cores.
They are apart from `TInterfacePortrait`, which relays the engine's side of a likeness.
Each relay is transparent and carries no test logic of its own.

## `internal static LPortraitMedium TPortraitMediumRead(CPortraitMedium medium) =>`

Relays the internal export format map.

## `internal static LPressTicket TPortraitTicketRead(CPressTicket ticket) =>`

Relays the internal print ticket map.

## `internal static LPortraitLabel TPortraitLabelRead(LSettingsPort settings) =>`

Relays the label wording, so a fact reads each key through a fake settings port.

## `internal static LPortraitLegend TPortraitLegendRead(LSettingsPort settings, string realm) =>`

Relays the legend wording of `realm`.

## `internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead() =>`

Relays the export formats the file question offers.

## `internal static Task TPortraitFileExport(CEnvoy envoy, string file, Func<string, LPortraitMedium, Task> export) =>`

Relays the shared export core with its question and failure policy.
The fixed `file` is handed on as the core's name read.
Its settings port is the shared fake from `TInterfaceConduct`.

## `internal static Task TPortraitFileExport(CEnvoy envoy, LSettingsPort settings, Func<string> fileRead, Func<string, LPortraitMedium, Task> export) =>`

Relays the shared export core with a given settings port and name read.
A fact can thus fail the name read or pass no settings.

## `internal static Task TPortraitTicketPrint(CEnvoy envoy, Func<LPressTicket, Task> print) =>`

Relays the shared print core with its question and failure policy.
