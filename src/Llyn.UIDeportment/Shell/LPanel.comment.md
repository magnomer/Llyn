# LPanel.cs

## `public sealed class LPanel`

The state one browsing panel carries, shared by every browse tab through its own deportment.
Its mode is the vista's editing flag and its chosen row is the vista's choice, so it copies neither.
Every verdict the veneer writes into a control is read off the vista at the moment it is asked.
Every request the veneer forwards ends in a notice, and the veneer writes its controls only on notice.
The questions a panel asks the user leave through seams the veneer hands in, so no dialog is known here.
The rows a tab lists are the tab's own, so this class only announces that they need re-reading.

## `private readonly Func<bool> _lPanelShownSeam;`

Whether the panel's tab is the one in front, read from the veneer because only a control knows it.
It is consulted when a stored entry is announced, so a hidden tab in edit mode does not adopt it.

## `public event Action<long>? LPanelEdited;`

The editor must show this stored entry, raised only while the panel is in edit mode.

## `public event Action<string, Exception>? LPanelFailed;`

A load or delete the engine refused, with the localization key the window shows it under.

## `public void LPanelVistaRestore(LVista vista)`

Takes the vista the window started for the tab.
A vista opens with the stored split, which no control yet shows, so the split is switched off here.
The navigation restores it again for the tab in front once a row is chosen, as it always did.

## `public bool LPanelChangeCheck()`

Whether the editor holds unstored changes, which can only be so while it is shown.

## `public bool LPanelLeaveConfirm()`

Whether the panel may leave what it edits, asking through the leave seam only when something is unstored.

## `public void LPanelScribeSet(bool editing)`

The mode toggle: a switch to the mode already shown does nothing, so a reloaded editor never drops typing.
Leaving the editor asks first, and a refusal re-shows the editor so the toggle reads right again.
Leaving with a chosen row reloads it into the display, leaving with none clears the panel.
Entering with a chosen row hands it to the editor, entering with none clears the panel.

## `public void LPanelFreshOpen()`

The fresh step without the leave question, for a deportment that already asked for the whole tab.

## `public void LPanelScribeShow(bool editing)`

Sets the mode on the vista and announces the change, asking nothing.
Public so a two-list deportment can carry the mode from one list to the other.

## `public void LPanelRowShow(long? id)`

Selects and loads a row without the leave question, for a deportment that already asked.

## `private void LPanelDraftShow()`

Loads the chosen row after a click or a return to the display.
The vista drops a choice that no longer loads, so the chosen verdict decides the clear.
No engine answer is branched on, and the loaded draft travels to the veneer inside the notice.

## `public void LPanelDraftUpdate()`

The chosen entry changed under the panel, so it is reloaded quietly and a vanished one clears the panel.

## `public void LPanelEntryHandle(LBulletin bulletin)`

Any stored entry re-lists the rows.
A tab in front and in edit mode adopts the stored entry as its choice.
That is how a fresh entry becomes the shown one.

## `public long LPanelVoyageRead()`

The record the panel stands on, as the station the window's trail records.
Zero says the panel stands on none, so there is no place to come back to.
