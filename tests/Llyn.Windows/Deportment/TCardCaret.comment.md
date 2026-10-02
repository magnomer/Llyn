# TCardCaret.cs
Hash: `718e7b57cfb578b3`

## `public sealed class TCardCaret`

Covers the caret of a card's Tag field, which is anchored to a chip and is not one of them.
The card is built on its own STA thread, since its fields are WPF collections.

## `public void CardLabelMove_StepsTheCaretAndLeavesTheTagsInOrder()`

A shown field puts the caret after its last Tag.
A step moves only the caret's index and never the Tags.
A step past either end is refused and leaves the caret where it stood.
The Tag found one step back or forward is the one beside the caret.

## `public void CardLabelShow_KeepsTheCaretBeforeTheTagThatFollowedIt()`

A Tag the engine adds at the caret lands before it.
So the caret stays before the Tag that followed it.
When no Tag that followed it is left, the caret falls back to the end of the field.
An emptied field puts the caret at its start.

## `public void CardLabelShow_PutsTheCaretBeforeTheNextTagWhenItsTagIsGone()`

Erasing the Tag right after the caret keeps the caret in place.
So the caret then stands before the next Tag that still follows it.

## `private static void TCardCaretRun(Action<object> act)`

Builds a blank card on an STA thread and runs the case against it.
