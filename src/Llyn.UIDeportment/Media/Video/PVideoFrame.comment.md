# PVideoFrame.cs

## `public sealed class PVideoFrame : Decorator`

The element that wraps a reading view's film row into the row the Screen plays.
A reading view binds a ready card's film rows straight.
A row arriving as the element's data is wrapped here.
The wrapper becomes the element's data, so the Screen below binds as it does in the editor.
A row the engine calls empty collapses the element.

It is public because the Veneer's display markup names it.

## `public PVideoFrame()`

Refines the element each time its data changes.

## `private void PVideoContextRefine(object sender, DependencyPropertyChangedEventArgs e)`

A reading view lists a ready card's film rows, so a row arriving as the element's data is refined here.
Any other data is left alone.

## `private void PVideoEmptyRefine(CVideoDraft draft)`

A row Conduct calls empty collapses the element, since there is nothing to show for it.

## `private void PVideoRowRefine(CVideoDraft draft)`

The loading row becomes the element's data instead, so the Screen below binds as it does in the editor.
