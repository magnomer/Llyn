# PVideoFrame.cs

## `public sealed class PVideoFrame : Decorator`

The element that wraps a reading view's film row into the row the Screen plays.
A reading view binds a ready card's film rows straight.
A row arriving as the element's data is wrapped here with the engine the view handed down.
The wrapper becomes the element's data, so the Screen below binds as it does in the editor.
A row the engine calls empty collapses the element.

It is public because the Veneer's display markup names it.
