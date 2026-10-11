# CSLensState.cs
Hash: `05c2a8a8c597edd3`

## `public sealed record CSLensState(double CSLensStateFactor, CSLensMode CSLensStateMode, string CSLensStateText)`

The zoom answer every `CSLens` gate returns, ready for the driver to paint.
The driver scales the sheets by the factor, sets the fit radios from the mode and shows the text.
It also derives the decoded picture width from the factor, under the same cap the lens uses.

**Parameters**

- `CSLensStateFactor`: the scale of the shown page, between 0.1 and 4.
- `CSLensStateMode`: the fit mode the factor came from.
- `CSLensStateText`: the factor as a whole percent in the current culture, for the zoom box.
