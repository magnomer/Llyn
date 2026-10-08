# CPanel.cs
Hash: `9a7bfccdec6e4dd6`

## `public sealed class CPanel`

The interaction of one browsing panel, shared by every browse tab and both drivers.
Its mode is the vista's editing flag and its chosen row is the vista's choice, so it copies neither.
Every verdict a driver paints is read off the vista at the moment it is asked.
Every question it puts to the user leaves through `CEnvoy`, so no dialog is known here.
Its list slot is the `CAperture` it composes, and its delete is the `CPanelBin` it composes.
The engine work is one `LVista` member per step, and the data rules stay in the engine.

## `private readonly LSettingsPort _cPanelSettingsPort;`

The port a failed load reads its ready notice through, before the envoy shows it.

## `private readonly LVistaPort _cPanelVistaPort;`

The port the panel loads its vista's chosen row through.
The vista holds only view state, so every stored read goes through this port.

## `private readonly Func<bool> _cPanelChangeSeam;`

Whether the editor the panel feeds holds unstored changes, read from that editor's own desk.

## `private readonly Func<bool, bool> _cPanelFinishSeam;`

Stores the editor's work when the user chose to store on leaving, and answers whether it went through.
A two-list tab hands in its session's finish, so the whole tab stores as one.

## `private readonly Func<bool> _cPanelShownSeam;`

Whether the panel's tab is the one in front, which only the driver's surface knows.
It is consulted when a stored entry is announced, so a hidden tab in edit mode does not adopt it.

## `internal CPanel(CEnvoy envoy, LSettingsPort settings, LVistaPort vistas, string loadKey, string? deleteScope, Func<bool> changeSeam, Func<bool, bool> finishSeam, Func<bool> shownSeam, string? vacantKey = null, string? unmatchedKey = null)`

`loadKey` words every failed load and tally read, so a blank one throws.
It builds the aperture and the bin, handing the delete scope to the bin.
The two empty list keys pass through to the aperture, and only lists with an empty notice give them.
A done delete closes the panel, so the bin's `CPanelBinDeleted` is wired to `CPanelEntryClose`.
The aperture holds no vista until `CPanelVistaRestore`, so every verdict before it reads as no row and no edit.

## `public event Action? CPanelChanged;`

The mode or the loaded draft moved, so a driver reads its verdicts again.

## `public event Action? CPanelCleared;`

The chosen row closed, so the owner drops the draft its editor held.

## `internal event Action<LDraft>? CPanelDraftChanged;`

The draft just loaded, passed on unread for the controllers that show it.
It is internal because a draft is engine data, which no driver receives from Conduct's surface.

## `public event Action<long>? CPanelEdited;`

The editor must show this stored entry, raised as the panel enters edit mode or loads a row in it.

## `public CAperture CPanelAperture { get; }`

The list slot the panel stands on, which holds its vista, rows notice, ordering, filter and tally.
Every verdict below reads the vista through it.

## `public CPanelBin CPanelBin { get; }`

The delete request of the panel, which reads its vista off the aperture.

## `public bool CPanelEditing`

Whether the vista is in edit mode, false before a vista arrives.

## `public bool CPanelBinEnabled`

Live while a row is chosen, so owners read it as the panel's chosen-row fact too.

## `public bool CPanelModeEnabled`

Whether the mode switch is live, which needs a chosen row or the edit mode already on.

## `public bool CPanelViewerChecked`

Whether the viewer side of the mode switch is ticked, which is the opposite of the edit mode.

## `public bool CPanelScribeChecked`

Whether the scribe side of the mode switch is ticked, which is the edit mode itself.

## `public bool CPanelPressAllowed`

Print takes a chosen row only while it is read, since a draft being written is not stored yet.

## `internal void CPanelVistaRestore(LVista vista)`

Takes the vista the window started for the tab and hands it to the aperture.
A vista opens in its stored mode, which no control yet shows, so edit mode is switched off here.
The navigation restores it for the tab in front once a row is chosen.
Only a panel's vista is switched, so the Tag, Register and column vistas keep their stored mode.

## `internal bool LPanelChangeCheck()`

Whether the editor holds unstored changes, which can only be so while the panel is in edit mode.

## `public bool CPanelLeaveConfirm()`

Whether the panel may leave what it edits.
It asks through the envoy only when something is unstored.
Storing runs the finish seam, discarding leaves the dropping to the step that asked, and staying answers false.

## `public void CPanelEntryClose()`

Closes what the panel shows.
Nothing is chosen, the rows re-list and the viewer returns.

## `public void CPanelEntryCreate()`

The fresh request.
After the leave question it opens the scribe on nothing.

## `public void CPanelFreshOpen()`

The fresh step without the leave question, for a two-list tab that already asked for the whole tab.

## `internal void LPanelScribeRestore(bool editing)`

Restores the mode the window closed on, but edit mode only over a chosen row.
The navigation calls it for the tab it restores at startup.

## `public void CPanelScribeToggle(bool editing)`

The mode toggle.
A switch to the mode already shown does nothing, so a reloaded editor never drops typing.
Leaving the editor asks first, and a refusal re-shows the editor so the toggle reads right again.
Leaving with a chosen row reloads it into the display, leaving with none closes the panel.

## `private void LPanelScribeOpen()`

Entering the editor hands the chosen row to it, and with no row the panel closes instead.

## `public void CPanelScribeSet(bool editing)`

Sets the mode on the vista and announces the change, asking nothing.
A two-list tab carries the mode from one list to the other with it.

## `public void CPanelRowSelect(long? id)`

A click on a row.
After the leave question the row is opened.
A panel standing on a tab first records the station the click leaves.

## `internal void LPanelStationAttach(Action record)`

The area whose tab this panel is hands in the navigation's station record.
A sub-panel never gets one, so its clicks record nothing.

## `internal long LPanelChosenRead()`

The record the panel stands on, as the station the navigation's voyage records.
Zero says the panel stands on none, so there is no place to come back to.

## `public bool CPanelRowOpen(long? id)`

Selects and loads a row without the leave question, for a step that already asked.
It answers whether the panel now holds the row.

## `private bool LPanelDraftShow(Func<LDraft?> load)`

Loads a row through the given vista load, after a click or a return to the display.
A refused load is told through the envoy and answers false, and the vista has restored its prior choice.
The vista drops a choice that no longer loads, so the chosen verdict decides the close.

## `public void CPanelDraftResonate()`

The chosen entry changed under the panel, so it is reloaded.
A failed reload shows the panel's load key and repaints nothing.
The close check still runs after a failure, so a panel whose row went away closes.
A vanished entry closes the panel.

## `public void CPanelEntryResonate(CBulletin bulletin)`

A stored entry re-lists the rows and repaints the mode, after the adoption below.

## `public void CPanelEntrySelect(CBulletin bulletin)`

A tab in front, in edit mode and with no chosen row adopts the stored entry as its choice.
That is how a fresh entry becomes the shown one, while an entry under edit keeps its row.
It raises nothing, because the adopted entry's own notice reloads and repaints the panel next.
