# TCardList.cs
Hash: `3b02e182cf96e69f`

## `public sealed class TCardList`

Covers the card list gates over an entry desk on a real workspace, with no delay.
A new meaning or collocation lands last in its own list.
A card is dropped from a list of two, and a list of one keeps its card.
A typed number moves a card to that place, clamped to the list, and an unreadable one moves nothing.
A dragged place moves a card there as it stands.
A card the draft lacks keeps the order rather than being refused.
A dragged place is a move only when it lies in the list and differs from the card's own.
The clerk rules behind the gates are read on the held draft through their relays.

## `private static (CDesk TCardListDesk, CCardList TCardListCard) TCardListPrepare(LEngine engine, int count)`

Starts the card desk, builds its list gates and adds meanings through them until the draft holds `count`.

## `private static IReadOnlyList<long> TCardMeaningRead(CDesk desk)`

The ids of the held draft's meaning cards, in their order.

## `private static IReadOnlyList<long> TCardCollocationRead(CDesk desk)`

The ids of the held draft's collocation cards, in their order.
