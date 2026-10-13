# Repertoire

This folder holds the repertoire panel with its session, its situation list, its occurrence list and the scenario editor.

## `CAtlas`

The repertoire panel's situation list, the panel over the situation vista.
It finds the rows and their usage, while the panel's aperture takes the query, order and language filter.
Its panel loads, edits and deletes the chosen Situation, worded under the Situation scope.
Only `CRepertoire` builds it.
A vista, Situation, reflex or settings notice raises the panel's rows.
A workspace notice runs the answer the area hands in.
A driver builds its ordering menu from the orderings once.
The rows read answers null on failure and shows the load failure through the ledger.
Drivers read rows through `CRepertoireRowsRead`, which closes a stale selection.
The engine's own words name an unknown kind and an untitled situation.

## `LAtlasPortraitPrint`

Opens the print window on the chosen Situation with the Situation realm's legend.
It opens nothing while no Situation is chosen.
Only the repertoire's print gate calls it, which chooses the side that prints.

## `LAtlasRowRead`

The one map for a found situation to its row.
The title is the name the engine gave the row, so the atlas keeps its unknown and untitled words.
The kind and the count arrive worded, so no driver words either.

## `LAtlasDraftRead`

Maps the Situation a draft carries to its shape, and a missing draft or Situation to null.
It is a plain map with no rule, so it stays in Conduct, since ShellEngine cannot name a Conduct shape.
`CPlaywright` hands it the desk's draft unread.

## `LAtlasSituationRead`

Maps the Situation a loaded atlas draft carries to the vignette's ready page, and a missing one to null.
It only chooses keys.
The title words untitled while unwritten, and the kind and description word nothing.
The engine parses the description's text into Markdown blocks, so the vignette never parses.
The engine answers which media rows are filled, so the map picks no row itself.

## `COccurrence`

The repertoire panel's occurrence list, holding the entries that reference the chosen Situation.
The situation vista is its roll, and its own vista sits in the panel's aperture.
The occurrence side never deletes, so its panel has no delete scope.
It takes engine ports, so it stays internal to `CRepertoire`, which builds it.
The empty list reads `Situation.Vacant` when nothing references the Situation, and `Situation.Unmatched` under a search.
The change seam is the entry editor's desk, and the finish seam is the repertoire session's.
An entry notice selects a stored entry and refills the situation rows.
A notice on the chosen entry runs the chosen answer, and a vista notice raises the column's own rows.
The engine matches the query and drops the hidden languages, so the list decides nothing about matching.
The rows are every entry while no Situation is chosen.
Print and export are called only by the repertoire's gates.
Those gates choose the side and check an entry is on display.

## `CPlaywright`

The editor of the Situation scenario, as `CEditor` is the entry side's editor.
It owns the scenario desk, the scenario's typed edits, its picture and video rows and its scenario notices.
The repertoire keeps the panel's two sides and the moves between them, and reaches the desk through it.
It reports failures through the envoy, never through a seam a driver hands up.
The desk starts by subject rather than by a vista, with the `Repertoire` origin.
It keeps no port, and reads the atelier's ports on each use.
The picture and video gates are built fresh over the desk, as the editor builds its own.
The quill is null while no tenure runs or the desk fills.

## `CPlaywrightScenarioChanged`

Raised with the scenario of the Situation the desk holds after a start.
A cancel or a refused start raises the blank scenario, so the driver keeps no default of its own.
`CPlaywrightDraftChanged` only refreshes what an edit changed.
A draft notice with no Situation to show raises nothing, so the driver never redraws from nothing.

## `LPlaywrightDesk`

The scenario desk, which the repertoire's atlas and session run on.
The repertoire cancels it whenever the occurrence side takes the front or the scenario scribe closes.

## `LPlaywrightScenarioRead`

Reads the Situation the desk holds, which persists the tenure first and so can throw.
A throw reports `Situation.HoldFailed` through the envoy and reads as null.
It stays internal because the test relays read the held Situation through it.
`LPlaywrightHeldResonate` answers a failed or empty read with the blank scenario, so the driver always repaints.

## `CPlaywrightTitleSet`

Hands the typed title to the scenario's own edit on the tenure, which defers it.
It answers the typed line, whose hint is the untitled text, since typing ends an unknown mark.
The kind and description gates do the same for their lines.

## `CRepertoire`

The repertoire panel's session over the situation list, the occurrence list, the playwright and the entry editor.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
The routing between the two lists and its mode flags live in `CDiptych`, shared with the corpus.
Every gate holds only the interaction and reaches the engine through a panel, the playwright or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints or exports.
It is built the way `CCorpus` is.
The atlas stands over the playwright's desk, and the occurrence list stands over the editor's desk.
The session defers to the entry editor while the occurrence list edits, and holds only the editor's desk.
So the repertoire that owns the editor closes it, through its own close registered with the workspace.
A stored Situation is shown again outside the scribe through `LRepertoireStoredShow`.
The session's held notice reaches the playwright, which raises the scenario.
The occurrence panel opens the editor on the entry it edits, and a clear cancels the editor's desk.
The atlas's row notices reach the occurrence panel, whose rows follow the chosen Situation.
It restores its vistas last, so a built area already stands on started vistas.
The diptych's create seam is `LRepertoireOccurrenceCreate`.
Store follows the entry editor while the occurrence panel edits, and the playwright's desk otherwise.
The scenario area is enabled while the playwright's desk chronicle runs.
Print takes an entry on display or a chosen Situation on the vignette, and export only an entry on display.

## `LRepertoireSituationOpen`

The navigation's arrival on the repertoire tab.
It shows a Situation that comes from outside the list, keeping the scribe open when either editor was open.
When the query or the filter hides it, the list clears it at once.
Both are then dropped and the Situation is shown again, so an arrival never lands on a blank vignette.
A row that vanished stays cleared, and without a query or filter nothing is dropped.

## `LRepertoireStoredShow`

Shows the stored Situation outside the scribe after the playwright's desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `LRepertoireOccurrenceCreate`

Opens a fresh entry in the editor, already linked to the chosen Situation when there is one.
The engine starts and links it in one call, so the first paint shows the link.
The fresh open clears the occurrence panel, which closes the editor before the blank opens.

## `CRepertoireRowsRead`

A failed read is shown by the list and answers no rows, so nothing is cleared.
When the vignette shows a chosen Situation the rows no longer list, both lists clear before the rows return.
The clear announces fresh rows, which carry the same Situations, so a driver's nested refill is harmless.

## `CRepertoirePortraitPrint`

Prints the entry on display, else the chosen Situation on the vignette, else nothing.
The entry prints with the label, and the Situation list with the Situation realm's legend.
A decline of the printer question prints nothing.

## `LRepertoireVistaRestore`

Starts the `repertoire` vista over Situations by name and the `occurrence` vista over entries by headword.
The atlas takes the first, and the occurrence list takes both, since its rows follow the chosen Situation.
The entry editor takes the occurrence vista, so an entry it stores lands in that list.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.
The list owners then carry their held queries into the fresh vistas and attach their observers.

## Stamp

Commit: `c2f335c331fe`
- `CAtlas.cs` `aad04b9283512371`
- `COccurrence.cs` `63d0cc7626e361d3`
- `CPlaywright.cs` `8241b3343680f28d`
- `CRepertoire.cs` `6214813554895a4d`
