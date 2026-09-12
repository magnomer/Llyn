# PDisplayMention.cs

## `public partial class PDisplay`

What the display does when a word of a shown sentence is clicked.
The control that drew the sentence reports an offset, and the display asks the engine what stands there.
The engine is asked with the sentence, its Mentions and its language, all read off the control.
A sentence loaded from the store carries its own language, and the entry's language stands in when it carries none.
The window then decides what the answer opens, so every panel answers a word the same way.

## `internal void PDisplayCardScroll(long id)`

Scrolls the card with the given id into view and plays the spotlight on it.
The window calls it after opening an Entry from a word narrowed to one of its cards.
The card containers exist one dispatcher turn after the entry is shown, so the scroll waits for the layout pass.
It measures the card's place the way the compass does, and leads it by the same distance.
A card the entry no longer has is left unfound, and nothing moves.
