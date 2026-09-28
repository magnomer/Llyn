# LPanel.cs

## `public sealed class LPanel`

The state one browsing panel carries, shared by every browse tab through its own deportment.
Its mode is the vista's editing flag and its chosen row is the vista's choice, so it copies neither.
Every verdict the veneer writes into a control is read off the vista at the moment it is asked.
Every request the veneer forwards ends in a notice, and the veneer writes its controls only on notice.
The questions a panel asks the user leave through seams the veneer hands in, so no dialog is known here.
The rows a tab lists are the tab's own, so this class only announces that they need re-reading.

It is sealed: its public members name only .NET types and Conduct shapes.
It maps each Conduct shape to the engine's type inside, so a driver never holds engine data.
The vista and the loaded draft pass only between controllers, through internal members.
That keeps the class ready to sink into Conduct, where the console driver can share it.

## `private readonly string _lPanelDeleteKey;`

The localization key a refused delete is shown under, named by the deportment so the message names its record.
A panel that never deletes still names the key of the record it lists.

## `private readonly Func<bool> _lPanelShownSeam;`

Whether the panel's tab is the one in front, read from the veneer because only a control knows it.
It is consulted when a stored entry is announced, so a hidden tab in edit mode does not adopt it.

## `public event Action<long>? LPanelEdited;`

The editor must show this stored entry, raised only while the panel is in edit mode.

## `public event Action<string, Exception>? LPanelFailed;`

A load or delete the engine refused, with the localization key the window shows it under.

## `internal event Action<LDraft>? LPanelDraftChanged;`

The draft just loaded, for the controllers that show it.
It is internal because a draft is engine data, which a driver never receives.
A controller that owes its view something from the draft raises its own event with that part.

## `internal void LPanelVistaRestore(LVista vista)`

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

## `public bool LPanelRowShow(long? id)`

Selects and loads a row without the leave question, for a deportment that already asked.
It answers whether the panel now holds the row.
The vista restores its previous choice when the load throws, so a failed click changes nothing.

## `private bool LPanelDraftShow(Func<LDraft?> load)`

Loads a row through the given load, after a click or a return to the display.
A throw is announced and answers false, and the vista has already restored its prior choice.
The vista drops a choice that no longer loads, so the chosen verdict decides the clear.
No engine answer is branched on, and the loaded draft travels to the veneer inside the notice.

## `public void LPanelDraftUpdate()`

The chosen entry changed under the panel, so it is reloaded quietly and a vanished one clears the panel.

## `public void LPanelObserverAttach(CSubject subject, Action<CBulletin> observer)`

Attaches a view's observer to the vista's notices on `subject`.
Each notice is copied into a Conduct notice inside the wrapper, before the observer sees it.
The wrapped delegate is never detached, because the vista lives as long as the tab.

## `public void LPanelChosenAttach(CSubject subject, Action<CBulletin> observer)`

The same as `LPanelObserverAttach`, but only for notices about the chosen row.

## `public CCatalogOrder LPanelOrder`

The vista's ordering as a driver marks it in a menu, headword order before a vista arrives.

## `public CCatalogFilter LPanelFilter`

The vista's language filter as a driver ticks it in a menu, hiding nothing before a vista arrives.

## `public void LPanelEntryHandle(CBulletin bulletin)`

Any stored entry re-lists the rows and repaints the mode.
A tab in front, in edit mode and with no chosen row adopts the stored entry as its choice.
That is how a fresh entry becomes the shown one, while an entry under edit keeps its row.

## `public void LPanelEntrySelect(CBulletin bulletin)`

The adoption alone, for a two-list deportment whose rows already re-list on the same bulletin.
It raises nothing, because the adopted entry's own bulletin reloads and repaints the panel next.

## `public long LPanelVoyageRead()`

The record the panel stands on, as the station the window's trail records.
Zero says the panel stands on none, so there is no place to come back to.

## `internal static CCatalogOrder LPanelOrderRead(LCatalogOrder order)`

The Conduct mirror of an engine ordering, member for member.
Every controller that shows an ordering maps it here, so the mirror has one home.
It casts, since the mirror declares its members in the engine's order.
A branch per member would decide nothing, and a test holds the order.

## `internal static LCatalogOrder? LPanelOrderRead(CCatalogOrder? order)`

The engine ordering a driver's choice stands for, or null when the driver chose none.
A setter keeps the vista's ordering on null, so the view's handler holds no branch.

## `internal static CCatalogFilter LPanelFilterRead(LCatalogFilter filter)`

The Conduct copy of an engine filter, carrying its hidden languages.

## `internal static LCatalogFilter LPanelFilterRead(CCatalogFilter filter)`

The engine filter a driver's ticks stand for.
The engine builds it, so a filter hiding nothing is its shared empty filter.

## `internal static LSubject LPanelSubjectRead(CSubject subject)`

The engine subject a driver's subject names, cast member for member like the ordering.
It is internal so the test that holds the member order can reach it.
