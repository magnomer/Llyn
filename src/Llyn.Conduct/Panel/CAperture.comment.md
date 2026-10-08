# CAperture.cs
Hash: `7645b95c3ccac75e`

## `public sealed class CAperture`

The list slot of one browsing panel, which holds the vista the panel stands on.
It owns the rows notice, the query, ordering and filter a driver sets, and the tally chip.
It also answers the chosen row id and whether the last rows read came back empty.
The rows a tab lists are the tab's own, so it only announces that they need re-reading.
The mode and the opening of a row stay with `CPanel`, which reads the vista through this slot.

## `private readonly LSettingsPort _cApertureSettingsPort;`

The port a failed tally read reads its failure notice through, before the envoy shows it.

## `private readonly LVistaPort _cApertureVistaPort;`

The port the tally of the vista's chosen row is counted through.
The vista holds only view state, so every stored read goes through this port.

## `internal CAperture(CEnvoy envoy, LSettingsPort settings, LVistaPort vistas, string loadKey, string? vacantKey = null, string? unmatchedKey = null)`

`loadKey` words every failed tally read, so a blank one throws.
`vacantKey` and `unmatchedKey` word the empty list notice, and a list without one leaves them out.
The slot holds no vista until `CApertureRestore`, so every read before it answers the empty case.

## `public event Action? CApertureRowsChanged;`

A notice changed the rows, so the driver re-reads the list.
`CApertureRowsResonate` raises it.

## `internal LVista? CApertureVista`

The vista the panel stands on, or null before a restore.
It is internal so the panels' own areas read it while no driver sees engine data.

## `public CCatalogOrder CApertureOrder`

The vista's ordering as a driver marks it in a menu.
The engine answers the ordering before a vista arrives.

## `public CCatalogFilter CApertureFilter`

The vista's language filter as a driver ticks it in a menu.
The engine answers the filter before a vista arrives.

## `public long? CApertureChosen`

The id of the chosen row, or null before a vista arrives or while none is chosen.

## `public bool CApertureFiltered`

Whether the vista's filter hides any language, false before a vista arrives.

## `internal bool CApertureNarrowed`

Whether the query or the filter hides any row, as the vista answers it.
Only `CCorpus` and `CRepertoire` read it, when an arrival lands on a hidden row.

## `public bool CApertureEmpty`

Whether the last rows read found nothing, which the list shows as its empty notice.
It reads true until an owner hands a count through `CApertureCountSet`.

## `public bool CApertureQueried`

Whether the vista's query holds text, as the engine answers it, false before a vista arrives.

## `public string CApertureKey`

The localization key of the empty list notice.
A search with text reads as unmatched and no search as vacant.
It reads empty when the panel gave no keys.

## `public void CApertureQuerySet(string query)`

Hands the typed query to the vista, which refills the rows.
The vista announces only a changed text.

## `public void CApertureOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `public void CApertureFilterSet(CCatalogFilter filter)`

Hides the languages the user unticked in the filter menu.
The vista announces only a changed filter.

## `internal void CApertureCountSet(int count)`

The owner hands the number of rows it last read, so the empty notice follows the rows shown.
Only owners that read their rows as a plain list call it.

## `public string CApertureTallyRead()`

The tally chip's sentence for the chosen row, how many places cite it.
It is one ShellEngine call, `LVistaPort.LEngineTallyRead`, worded by Core's `LCatalogTallyFormat`.
Before a vista arrives it reads empty.
A fresh draft has no chosen row, so it reads as cited nowhere.
A failure shows the panel's load failure key, as its rows read does, and answers an empty chip.
Only the situation, example and reference lists call it, since only their vistas count a tally.

## `public void CApertureRowsResonate()`

A notice changed the rows, so the slot tells its driver to re-read them.

## `internal void CApertureRestore(LVista vista)`

Takes the vista the window started for the tab.
The held query is carried into the fresh vista first, so a workspace switch keeps the search.
The stored split stays as it is, since only a panel's `CPanelVistaRestore` switches it off.

## `public void CApertureObserverAttach(CSubject subject, Action<CBulletin> observer)`

Attaches a driver's observer to the vista's notices on `subject`.
Each notice is copied into a Conduct notice before the observer sees it.
The wrapped delegate is never detached, because the vista lives as long as the tab.

## `public void CApertureChosenAttach(CSubject subject, Action<CBulletin> observer)`

The same as `CApertureObserverAttach`, but only for notices about the chosen row.
