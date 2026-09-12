# TLookup.cs

## `public sealed class TLookup`

Covers the lookup fanning one source's readings out into candidates.
Every reading of a source becomes one candidate sharing the source's order and keeping its variety.
An untagged reading in a language with varieties becomes one candidate per declared variety, in pack order.
Without varieties the untagged reading stays untagged, so a pack like Spanish is unchanged.
A variety the answer already tags is not fanned out again.
A tag the pack does not declare passes through first.
An empty answer stays one untagged null candidate even when the pack declares varieties.
An empty answer still yields one candidate without a phonetic, so the shell can say why.
A dotted or spaced source form reaches the candidate normalized, with no switch involved.
A pack cleanup group applies to every candidate the same way, and a scoped one only to its fanned-out variety.
The returned set saved into a trove holds cleaned but un-respelled text.
Replaying that set through the respelling wrapper recasts what the receiver sees while the trove is unchanged.
The receiver hears the finish exactly once, and never when the lookup was cancelled.
