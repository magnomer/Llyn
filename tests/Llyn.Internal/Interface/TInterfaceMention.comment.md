# TInterfaceMention.cs

## `internal static class TInterfaceMention`

The relay that builds the sentence gates over a draft port whose chip line read fails.
It also runs the shared chip read over a failing draft port.
It runs the shared Meaning read over a scripted draft port.
It runs the corpus rows read over a failing entry port.
It runs the favorites rows read over a failing entry port.
It runs the occurrence rows read over a failing entry port.
It is transparent and carries no test logic of its own.

## `internal static CSentence TSentenceFailCreate(LEngine engine, CDesk desk, CEnvoy envoy)`

Builds sentence gates whose chip line read throws, over a real desk.

## `internal static IReadOnlyList<CCatalogExample>? TAnthologyFailRead(LEngine engine, CAtelier atelier, CEnvoy envoy)`

Runs the corpus rows read over an entry port whose Example find throws, on a real vista.

## `internal static CMentionOffer? TAnthologyMentionFind(LEngine engine, CAtelier atelier, CEnvoy envoy, long? chosen, IReadOnlyList<long>? found)`

Finds the word at offset 3 of the chosen Example, with `chosen` on a real vista.
The fake word find answers the word at offset 2 with the `found` Entries, and throws when `found` is null.
The atelier's navigation opens what the find opens at once.

## `internal static CMentionOffer TMentionResultOpen(CAtelier atelier, CMentionResult result) =>`

Runs the shared open of a find answer over the atelier's mention area.

## `internal static CMentionOffer? TDisplayMentionFind(LEngine engine, CAtelier atelier, CEnvoy envoy)`

Finds a word through a display whose entry port's word find throws.
The display is attached to the atelier's navigation and mention area, as the composition does.

## `internal static IReadOnlyList<CMentionLabel> TMentionFailRead(LEngine engine, CDesk desk, CEnvoy envoy)`

Runs the shared chip line read over a draft port whose resolve throws.

## `internal static IReadOnlyList<CVistaRow> TOccurrenceFailRead(LEngine engine, CEnvoy envoy)`

Runs the occurrence rows read over an entry port whose find throws.

## `internal static IReadOnlyList<CVistaRow> TFavoriteFailRead(LEngine engine, CEnvoy envoy)`

Runs the favorites rows read over an entry port whose favorite find throws, on a real vista.

## `internal static CMentionSense? TMentionSenseRead(LEngine engine, CDesk desk, CEnvoy envoy, Func<object?[]?, object?> sense)`

Runs the shared sense menu read on a desk with a live draft.
The draft port's sense read answers through `sense`.
A test hands rows to check the map and the key, or a throw to check the failure policy.
