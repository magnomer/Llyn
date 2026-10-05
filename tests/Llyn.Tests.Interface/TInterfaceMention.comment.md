# TInterfaceMention.cs
Hash: `8ac88a45e7d8825b`

## `internal static class TInterfaceMention`

The relay that builds the sentence gates over a draft port whose chip line read fails.
It also runs the shared chip read over a failing draft port.
It builds sentence gates whose chip line read answers any lines a fact hands, and one chip.
It runs the shared Meaning read over a scripted draft port.
It runs the corpus rows read over a failing entry port.
It runs the favorites rows read over a failing entry port.
It runs the quotation rows read over a failing entry port.
It runs the occurrence rows read over a failing entry port.
It runs the display word find over a failing or recording entry port.
It maps a click result and adds a Mention to an Example, so a test builds no engine record itself.
It runs the unit read of a piece and the shared open of a find answer.
It is transparent and carries no test logic of its own.

## `internal static CSentence TSentenceFailCreate(LEngine engine, CDesk desk, CEnvoy envoy)`

Builds sentence gates whose chip line read throws, over a real desk and a fresh repaint memory.

## `internal static CSentence TSentenceLineCreate(LEngine engine, CDesk desk, CEnvoy envoy, IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>>? lines)`

Builds sentence gates whose chip line read answers `lines` as it stands, over a real desk.
A fact hands rows the draft lacks, rows it misses or null, so the read meets the engine's worst answer.
Its repaint memory is fresh, and a failed read reaches `envoy` through a real settings outlet.

## `internal static LMentionLabel TMentionLabelCreate(long id, string word, long entry)`

Builds one chip of `word`, named `word`, with no sense, so a fact builds no engine record itself.
It is linked whenever `entry` is not zero.

## `internal static IReadOnlyList<CCatalogExample>? TAnthologyFailRead(LEngine engine, CAtelier atelier, CEnvoy envoy)`

Runs the corpus rows read over an entry port whose Example find throws, on a real vista.

## `internal static CMentionOffer? TAnthologyMentionFind(LEngine engine, CAtelier atelier, CEnvoy envoy, long? chosen, IReadOnlyList<long>? found)`

Finds the word clicked at unit 3 of the chosen Example's text, with `chosen` on a real vista.
The fake word find answers the word at offset 2 with the `found` Entries, and throws when `found` is null.
The atelier's navigation opens what the find opens at once.

## `internal static CMentionOffer TMentionResultOpen(CAtelier atelier, CMentionResult result, string text) =>`

Runs the shared open of a find answer over the atelier's mention area.
The whole shown `text` places the offer.

## `internal static int? TMentionUnitRead(CAtelier atelier, string text, int start, int offset) =>`

Runs the atelier's unit read of one piece, which only the result open calls in the product.

## `internal static CMentionResult TMentionResultRead(int offset, LMention? stored) =>`

Maps a click result holding only `stored`, so a test builds no engine record itself.

## `internal static LExample TExampleMentionAdd(this LExample example, LMention mention) =>`

The Example with `mention` as its only Mention, so a test builds an Example that links.

## `internal static CMentionOffer? TDisplayMentionFind(LEngine engine, CAtelier atelier, CEnvoy envoy, long shown)`

Finds a word through a display whose entry port's word find throws.

## `internal static int? TDisplayOffsetRead(LEngine engine, CAtelier atelier, long shown, string text, int unit)`

Clicks a word through the display find gate and answers the offset the engine find was handed.
The fake word find records that offset and then throws, so nothing opens.
Null means the gate never reached the engine.

## `private static CDisplay TDisplayCreate(LEngine engine, CAtelier atelier, CEnvoy envoy, LEntryPort entries, long shown)`

Builds a display over the scripted `entries`, for both display finds above.
The display is attached to the atelier's navigation and mention area, as the composition does.
It holds a fresh repaint memory, since the lookup is a user act that never goes through it.
It shows the stored entry `shown`, since a display showing nothing answers before it asks the engine.

## `internal static IReadOnlyList<CMentionLabel> TMentionFailRead(LEngine engine, CDesk desk, CEnvoy envoy)`

Runs the shared chip line read over a draft port whose resolve throws.

## `internal static IReadOnlyList<CVistaRow> TQuotationFailRead(LEngine engine, CEnvoy envoy)`

Runs the quotation rows read over an entry port whose entry find throws.

## `internal static IReadOnlyList<CVistaRow> TOccurrenceFailRead(LEngine engine, CEnvoy envoy)`

Runs the occurrence rows read over an entry port whose find throws.

## `internal static IReadOnlyList<CVistaRow> TFavoriteFailRead(LEngine engine, CEnvoy envoy)`

Runs the favorites rows read over an entry port whose favorite find throws, through an atelier built over that port.

## `internal static CMentionSense? TMentionSenseRead(LEngine engine, CDesk desk, CEnvoy envoy, Func<object?[]?, object?> sense)`

Runs the shared sense menu read on a desk with a live draft.
The draft port's sense read answers through `sense`.
A test hands rows to check the map and the key, or a throw to check the failure policy.
