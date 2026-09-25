# LLectern.cs

## `public sealed class LLectern`

The reading view's deportment, standing between the veneer and [LDisplay](../../Llyn.Conduct/Display/LDisplay.comment.md).
Every member forwards to one `LDisplay` member of the same shape.
The veneer names only this class, so no veneer reaches Conduct.

## `public static bool LLecternNarrativeCheck(bool editable, string text)`

Forwards statically, since the check holds no state.
