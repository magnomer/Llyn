# TReflexHinge.cs
Hash: `13ca0cdecbc6066c`

## `public sealed class TReflexHinge`

Covers the two "More readings" hinges, `PDisplayReflexHinge` in the reading view and `PReflexHinge` in the editor.
Each section and its named parts live on their own STA thread, since a WPF control demands one.
A click is the hinge's own state change followed by its click event, as a user's press is.

## `public void LecternHingeClick_NoEntryShown_PutsTheHingeBackAndStoresNothing()`

With nothing shown the reading view's gate refuses the write, so the hinge goes back to closed.

## `public void LecternFoldRefine_StoredOpenedEntry_ChecksTheRenamedHinge()`

A shown entry stored opened paints the renamed hinge checked from the stored state.
After repainting, the hinge and persisted opening both remain true.

## `public void ReflexHingeClick_NoHeldEntry_PutsTheRenamedHingeBackAndStoresNothing()`

With an empty desk the editor's gate refuses the write, so the renamed hinge goes back to closed.

## `private static StackPanel TReflexLecternCreate()`

The reading view's reflex parts by contract ID, the hinge among them.

## `private static ToggleButton TReflexHingeFind(Panel surface, string name)`

The hinge named `name` among the surface's children.

## `private static void TReflexHingeToggle(ToggleButton hinge, bool opened)`

Sets the hinge to `opened`, then raises its click, as a press does.

## `private static long TReflexHingePrepare(LEngine engine)`

Stores a bare English entry and answers its id.

## `private static void TReflexHingeRun(Action body)`

Runs `body` on an STA thread and fails the test with any exception it threw.
