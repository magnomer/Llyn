# PVideoFrame.cs
Hash: `669bc595be841abb`

## `public sealed class PVideoFrame : Decorator`

The element that unwraps a reading view's film line into the row the Screen plays.
A reading view binds a card's [QLeafVideo](../../Display/Template/QLeafVideo.comment.md) lines straight.
A line arriving as the element's data is unwrapped here.
Its film row becomes the element's data, so the Screen below binds as it does in the editor.
A line Conduct calls empty collapses the element.

It is public because the Veneer's display markup names it.

## `public PVideoFrame()`

Refines the element each time its data changes.

## `private void PVideoContextRefine(object sender, DependencyPropertyChangedEventArgs e)`

A reading view lists a card's film lines, so a line arriving as the element's data is refined here.
Any other data is left alone.

## `private void PVideoEmptyRefine(QLeafVideo video)`

A line Conduct calls empty collapses the element, since there is nothing to show for it.

## `private void PVideoRowRefine(QLeafVideo video)`

The line's film row becomes the element's data instead, so the Screen below binds as it does in the editor.
