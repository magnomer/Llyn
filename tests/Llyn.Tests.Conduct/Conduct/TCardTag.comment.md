# TCardTag.cs
Hash: `b39dd4915b874323`

## `public sealed class TCardTag`

Covers the tag gates of a card over an entry desk on a real workspace, with no delay.
A typed tag lands trimmed and once, a picked tag keeps its stored id, and an erased tag leaves.
A typed tag offers the stored Tags split around the word, at most eight.
A blank or unmatched tag offers nothing.
A typed list adds its completed tags once each and keeps the rest.
A tag the card holds stays out of its own offer, and another card still sees it.
Each test builds its card through `TCard.TCardPrepare`.
The tests that write a tag add its sheet through `TCard.TCardSheetAdd`.

## `private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)`

The tags the held draft's card carries.
