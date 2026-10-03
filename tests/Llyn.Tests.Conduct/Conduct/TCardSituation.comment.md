# TCardSituation.cs
Hash: `97020c28ffde233f`

## `public sealed class TCardSituation`

Covers the situation gates of a card over an entry desk on a real workspace, with no delay.
A typed list adds its completed titles once each and answers the rest.
A picked situation lands once under its stored id, and the card's offer then leaves it out.
An erased situation leaves.
A typed word offers the stored Situations split around the word, at most eight.
A blank or unmatched word offers nothing.

## `private static IReadOnlyList<LSituationDraft> TCardSituationRead(CDesk desk, long sheet)`

The situations the held draft's card carries.
