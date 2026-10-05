# TCardRow.cs
Hash: `618531bdc90512ef`

## `public sealed class TCardRow`

Covers the shared row matcher a card list uses to keep its held rows across a repaint.
It runs on transcription rows with the sheet's own key and painters, through the card row relay.
The rows are plain notifying objects, so the case needs no STA thread.

## `public void CardRowShow_DuplicateIds_KeepsEachRowOnce()`

Two held rows and three drafts all carry the same id.
Each held row is matched once, in order, and takes its own draft's text.
The third draft finds no free held row, so a new row is made for it.
A match that ignored rows already taken handed every draft to the first row and dropped the second.
