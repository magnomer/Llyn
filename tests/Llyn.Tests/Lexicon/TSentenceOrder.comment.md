# TSentenceOrder.cs

## `public sealed class TSentenceOrder`

Covers the one thing a language pack says about an Example's frame: which of its two fields is written first.
The packs on disk are read, not a fixture, because the ordering shipped with a language is the fact under test.

## `public void SentenceOrderLoad_English_WritesTheMarkerFirst()`

English writes the marker before the role, which is the order its pack declares.

## `public void SentenceOrderLoad_Japanese_WritesTheRoleFirst()`

Japanese declares the opposite order and must read back the opposite order.
A language whose pack reverses the two and still draws English's order is the whole reason this is read from the pack.

## `public void SentenceOrderLoad_LanguageWithNoPack_WritesTheMarkerFirst()`

A language nothing ships a pack for still draws its rows.
It falls back to the marker first rather than refusing to lay the row out.
