# TInterfaceMention.cs
Hash: `c9db23b5f991af6c`

## `internal static class TInterfaceMention`

The relay that builds the sentence gates over a draft port whose chip line read fails.
It also runs the shared chip read over a failing draft port.
It runs the shared Meaning read over a scripted draft port.
It runs the corpus rows read over a failing entry port.
It runs the favorites rows read over a failing entry port.
It runs the quotation rows read over a failing entry port.
It runs the occurrence rows read over a failing entry port.
It maps a click result and adds a Mention to an Example, so a test builds no engine record itself.
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

## `internal static CMentionResult TMentionResultRead(int offset, LMention? stored) =>`

Maps a click result holding only `stored`, so a test builds no engine record itself.

## `internal static LExample TExampleMentionAdd(this LExample example, LMention mention) =>`

The Example with `mention` as its only Mention, so a test builds an Example that links.

## `internal static CMentionOffer? TDisplayMentionFind(LEngine engine, CAtelier atelier, CEnvoy envoy, long shown)`

Finds a word through a display whose entry port's word find throws.
The display is attached to the atelier's navigation and mention area, as the composition does.
It shows the stored entry `shown`, since a display showing nothing answers before it asks the engine.

## `internal static IReadOnlyList<CMentionLabel> TMentionFailRead(LEngine engine, CDesk desk, CEnvoy envoy)`

Runs the shared chip line read over a draft port whose resolve throws.

## `internal static IReadOnlyList<CVistaRow> TQuotationFailRead(LEngine engine, CEnvoy envoy)`

Runs the quotation rows read over an entry port whose entry find throws.

## `internal static IReadOnlyList<CVistaRow> TOccurrenceFailRead(LEngine engine, CEnvoy envoy)`

Runs the occurrence rows read over an entry port whose find throws.

## `internal static IReadOnlyList<CVistaRow> TFavoriteFailRead(LEngine engine, CEnvoy envoy)`

Runs the favorites rows read over an entry port whose favorite find throws, through a real atelier.

## `internal static CMentionSense? TMentionSenseRead(LEngine engine, CDesk desk, CEnvoy envoy, Func<object?[]?, object?> sense)`

Runs the shared sense menu read on a desk with a live draft.
The draft port's sense read answers through `sense`.
A test hands rows to check the map and the key, or a throw to check the failure policy.
