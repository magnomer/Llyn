# Ledger

This folder holds the settings ledger with the workspace settings and their saves.
It also holds the reader of the pages a settings view lists, and the repaint memory of shown failures.

## `CLedger`

The settings ledger holds the saves of the workspace settings and the failure policy every gate shares.
A view is handed one ready state on every open and after every settings or workspace change.
It reads the settings port from `CAtelier` on every call rather than keeping a copy.
Only the atelier builds it, once.
It builds its `CLedgerReader` from the same atelier, and the reader never calls back.
`CLedgerChanged` carries the whole state, which the reader reads.

`LLedgerRepaint` is the atelier's repaint memory, built with the ledger and ending with the atelier.
The ledger owns it, since it is the failure policy every gate funnels through and it hears the user's acts.
`CEditor`, `CWing` and `CShelf` take it at build, so every repaint read shares it.
`LLedgerAttach` hooks the settings, workspace and vista bulletins and runs on the first open only.
Each such bulletin first clears the repaint memory, since each follows a user act.
A vista bulletin comes only when the user selects, searches, orders, filters or edits, never on a redraw.

Each save gate stores one setting and then raises the ledger again.
So the driver repaints from the saved settings rather than what was typed.
A failed save shows `Settings.SaveFailed`.
`CLedgerNotebookSave` answers the title the engine stored, which trims it and turns a blank one into `Llyn`.
It answers null on a failed save.
`CLedgerDepotSave` answers the flashcard folder the engine stored, or null on a failure or a refusal.
The engine refuses an absolute path, and the gate only shows the refusal.
`CLedgerDepotChange` answers what `CLedgerDepotSave` answered for the chosen folder, or null on a failure or a cancelled dialog.
`CLedgerDepotChange` asks for the folder through `CEnvoyFolderRead`, starting at the workspace root.
The folder sits under that root, so the dialog opens there.
A decline saves nothing, and a dialog that fails shows `Settings.SaveFailed`.
A saved analysis change makes the reading view and the editor repaint their inflection box.

`CLedgerFailureShow` shows a failure a driver's own medium raised, such as a player's refusal.
Such a failure has no gate, so the driver hands it here.

`LLedgerNoticeRead` is the one owner of the notice read, which every gate's failure policy shares.
A refusal shows its own reason, since the user can act on it.
A fault shows `Notice.Unexpected` and names its audit file under `Notice.Recorded` when one was written.
The engine answers in one call with the two keys, so the map holds no rule.
The static `LLedgerFailureShow` shows the notice with the gate's chosen key.
Every call shows, so a user act that fails again on a retry shows its failure again.
A read that runs on every repaint goes through `LLedgerRepaint` first, which merges its repeats.
The driver's consult never reads Conduct again, so it only shows what it was handed.

`CLedgerFolderOpen` reports a folder the shell cannot open under `Settings.FolderFailed`, since it may have moved away.
`CLedgerFind`, `CLedgerAnalysisRead` and `CLedgerMetaRead` relay to the reader, so drivers keep one door.

## `CLedgerReader`

Reads the ledger's state for display and owns the search over its pages.
Reading for display and changing settings are two roles, so the reads left `CLedger`.
The search text moved with its only reader, so it keeps one writer.
It asks only the atelier's settings port.

`CLedgerPages` fixes the page order, each with the keys its search also reads.
`CLedgerWeb` counts the online lookups, the frequency and the morphology lookup.
`_cLedgerWanted` keeps the last search text, so every later state shows the same narrowing.
`CLedgerReaderFind` alone writes it.
`CLedgerReaderFind` matches each page's title, helper and own keys by the shared catalog rule, and blank text shows every page.
`CLedgerMetaRead` takes `linked` raw, since the linked switch is GUI-only state kept by its Capsule.
`CLedgerReaderRead` loads the texts first, so every title and summary reads in the stored language.
The Layout summary stays empty, since only the driver knows the linked switch.
The Inflection summary reads custom analysis from the engine, and the Sync summary reads the notebook title.
An unknown page reads empty.
`CLedgerLanguageRead` answers the code itself for a shipped code the culture table does not know.
So a bad code never throws through a settings repaint.

## `CLedgerNoticed`

The repaint memory: the failure keys a repaint read has already shown.
A read that runs on every repaint would otherwise open one box per repaint while a fault lasts.
The atelier's ledger holds one, so its lifetime is the atelier's and no state is static.
A user action never routes here, so its failure shows on every retry.

`LLedgerRepaintShow` shows the first failure of a key through `CLedger.LLedgerFailureShow`.
A key already shown is merged.
Nothing is read, recorded or shown again until the memory clears.
A lock guards the set, since a background read may fail while the dispatcher shows another failure.
`LLedgerRepaintRead` is the one try, report and fallback rule of a repaint read.
`CSounding`, `CDisplaySound` and `CSentence` read through it, each binding its own envoy and settings port.
`LDisplaySound` holds no envoy and no memory, so it raises its failures to `LDisplay`.
`LLedgerNoticedClear` forgets every key, so the next failure under any key shows again.

## Plain records

- `CLedgerNotice` carries the reason key, and the audit file's caption key and path only when a file was written.
- `CLedgerPage` carries the page name a driver maps to the part it shows.
  The summary is empty for Layout.
- `CLedgerShown` lists the shown page names in page order, and whether none is shown.
- `CLedgerState` is everything a settings view shows, read in one call.
  The localization is normalised by the engine, and the languages come in scan order.
- `CSettings` copies the engine's online lookup count, so the rule stays there.

## Stamp

Commit: `2f9a06b8eb95`
- `CLedger.cs` `3e917fabd77b5d07`
- `CLedgerNotice.cs` `34a2b3923db6163d`
- `CLedgerNoticed.cs` `2f38e70b00bec262`
- `CLedgerPage.cs` `45f2b5ab3f092932`
- `CLedgerReader.cs` `17f075d6901963fc`
- `CLedgerShown.cs` `8b61a9eaaf6811db`
- `CLedgerState.cs` `a50b903fdb52a7de`
- `CSettings.cs` `b95f0474a6f0e2c1`
