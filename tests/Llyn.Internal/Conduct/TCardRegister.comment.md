# TCardRegister.cs

## `public sealed class TCardRegister`

Covers the register gates of a card over an entry desk on a real workspace, with no delay.
A typed list adds its completed names once each and answers the rest.
A picked register lands once under its stored id, and the card's offer then leaves it out.
A typed word offers the stored Registers split around the word, at most eight.
A blank or unmatched word offers nothing.
The draft's own language seeds the pack's Registers, so the driver passes none.

## `private static IReadOnlyList<LRegisterDraft> TCardRegisterRead(CDesk desk, long sheet)`

The registers the held draft's card carries.
