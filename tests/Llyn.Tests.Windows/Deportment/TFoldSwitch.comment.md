# TFoldSwitch.cs
Hash: `c18e0b18d51088ee`

## `public sealed class TFoldSwitch`

Covers the open switches of the editor's rime-book and script boxes.
Each click goes through `QCadence`, which holds the fold and hears both switches.
It also covers the reading view's switches, which `QLecternSound` hears over the display's own fold area.
Each case builds both boxes on one STA thread through `TWindow.TWindowRun`, as WPF requires.
A click is the switch's own state change followed by its click event, as a user's press is.

## `public void FoldSwitchClick_NoStoredEntryHeld_PutsBothSwitchesBackAndStoresNothing()`

Both switches start closed, and with no stored entry held each gate refuses the write.
So both switches go back to closed, the state before the click, and the store holds nothing.

## `public void FoldSwitchClick_OpenSwitchNoStoredEntryHeld_PutsBothSwitchesBackOpen()`

Both switches start open, and each click closes one while no stored entry is held.
Each gate refuses the write, so both switches return to open, the state before the click.
The store still holds nothing for either box.

## `public void FoldSwitchClick_HeldEntry_KeepsTheSwitchAndStoresTheBox()`

With a stored entry held the script gate stores the click, so the switch stays open.
Only the script box is stored, and the rime-book box stays folded.

## `public void LecternBoxRefine_StoredEntryShown_ShowsBothSwitchesWithTheStoredState()`

The reading view's boxes show their head with the switch.
Each switch shows the shown entry's stored state, open for the rime book and closed for the script.

## `public void LecternSwitchClick_NoEntryShown_PutsBothSwitchesBackAndStoresNothing()`

With nothing shown each display fold gate refuses the write.
So both switches go back to closed, the state before the click, and the store holds nothing.

## `public void LecternSwitchClick_ShownEntry_KeepsTheSwitchAndStoresTheBox()`

With an entry shown the display's rime-book gate stores the click, so the switch stays open.
Only the rime-book box is stored, and the script box stays folded.

## `private static StackPanel TFoldSwitchCreate(TEditorFixture editor, CAtelier atelier)`

Builds a surface holding the rime-book box first and the script box second, under their markup names.
It introduces the editor's facets to `QCadence` over that surface.
The atelier only lends its ledger, which a script failure would show through.

## `private static StackPanel TFoldViewCreate(CWing wing, CAtelier atelier)`

Builds a surface holding the reading view's rime-book box first and script box second, both in folded layout.
The reading line and paradigm box follow, since the sound section pulls them by contract ID.
It builds `QLecternSound` over the wing's display and paints the stored openings once.

## `private static void TFoldSwitchToggle(ToggleButton hinge, bool opened)`

Sets the switch to `opened` and raises its click, as a user's press does.

## `private static long TFoldSwitchPrepare(LEngine engine)`

Stores one English entry and answers its id.

## `private static void TFoldIconPrepare()`

Points the icon root at the test assembly's own icons, so a box can build its switch chevron.
The override keeps switch icon lookup within the test assembly.
