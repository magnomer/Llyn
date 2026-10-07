# TEngineFrequencyOrder.cs
Hash: `7b50311f228195d8`

## `public sealed class TEngineFrequencyOrder`

Covers the order an entry's frequency rows are shown in.
Rows stored out of pack order read back in the pack's declared source order.
Sources the pack does not declare follow by name.
A first source refetched last still gives the gauge its band and heads the tooltip.
Undeclared names sort by codepoint, so a supplementary ideograph follows a fullwidth letter.
It stores its rows through `TEngineFrequency.TFrequencyStoredSet` over the pack `TEngineFrequency.TEngineFrequencyPack`.
