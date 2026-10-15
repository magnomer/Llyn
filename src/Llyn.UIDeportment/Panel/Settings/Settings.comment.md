# Settings

The settings panel holds a catalog of setting groups on the left.
The card of the chosen group sits on the right.
It stands apart because the choice drivers live in `Panel/Choice`, and this folder only steers the catalog and the dial.
Each named part of the markup is pulled by its contract ID, which keeps the markup's names.

## `QSettings`

The settings panel's driver.
It owns the group catalog and the stored choices it opens on.
Showing the chosen group's page is the job of its `QDial`.
So is driving the card controls that no choice driver owns.
Most choices are driven by their own driver in `Panel/Choice`, which hears and paints that choice's controls.
The listing switch is driven by the dial, and the linked-panels switch by the window's `QLayout`.
What each choice costs, such as a catalog swap or a whole different workspace, lives in those drivers.
The constructor takes the veneer's page, which the window pulls by contract ID.
It builds one choice driver per concern over the surface, and each wires its own controls.
It builds the page dial over the same surface.
The linked-panels switch is an exception, since it is a setting on the window's one `QLayout`.
That keeper is handed in, and it wires the switch when the panel introduces it.
The window's GUI-only posture is handed in too, read for the linked summary.
The dial gets the same posture for the width reset.
Nothing else is wired before the window introduces it.
`QSettingsIntroduce` keeps the atelier, whose gates every settings file calls, and keeps no field for the envoy.
It sets the search hint and subscribes the search field.
It introduces the dial and each choice driver to only the Conduct area it calls.
Each of them also receives the window's envoy, for a failed save.
This happens before `CLedgerChanged` is subscribed, so the first state finds them ready.
It also subscribes the posture's linked switch, so the summary row follows a tick.
The ledger raises the state on every open and after every settings or workspace change.
The ledger rows get their fill through `QLookItemAttach` before the first state arrives.
It then shows the first card.
`QSettingsRefine` paints the whole panel from one ledger state.
The interface texts are applied first, so everything painted after reads in the stored language.
Each choice driver and the dial are handed their own values from the state.
The linked switch reads the posture.
The custom analysis driver reads its value from the ledger, since the state does not carry it.
`QSettingsDialRefine` marks one group's row chosen, and has the dial show that group's card.
`QSettingsLedgerRefine` paints the catalog from one ledger state.
That covers every page's row, the narrowing the state carries, and the Layout summary.
The narrowing rides on the state, so a settings change keeps a narrowed catalog narrowed.
`QSettingsPageRefine` writes one page's title and summary onto the row paired with it by the page's name.
A page with no row yet gets one, appended in the ledger's page order.
`QSettingsMetaRefine` writes the Layout row as `CLedgerMetaRead` words it for the linked flag the posture holds.
The linked switch calls it too, since that flag is GUI-only state no ledger state carries.
`QSettingsFindRefine` lists the rows of the pages named shown, and shows the empty text when none is shown.
Hidden rows leave the items source rather than collapsing, so the frame keeps its sibling spacing.
The chosen row may leave the catalog while its card stays, since the card is the dial's to swap.
A click on a row shows the card of that row's group.
Marking the row chosen is left to the card showing, so the first card marks its row the same way.
`QSettingsItemRefine` fills one ledger row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
A new title or summary on the item refills the row the same way.
The search field's text goes raw to `CLedgerFind`, which answers the pages it shows.
Conduct keeps the text and matches it, so a console searches the same way.

## `QDial`

The settings panel's card dial, which shows the page of one group and hides the others.
It owns the table that pairs each group's name with the page the markup draws for it.
It also drives the card controls no choice driver owns.
Those are the listing switch, the notebook and depot fields, the depot picker, and the folder and width buttons.
Marking the chosen row in the catalog stays with `QSettings`, which calls the dial.
The constructor takes the veneer's page and the window's GUI-only posture, which the width reset clears.
`QDialIntroduce` keeps the ledger those controls call and the window's envoy, for a failed save.
It sets the folder, width and depot picker icons and subscribes the controls' events.
`QDialRefine` shows the page of the named group and collapses every other page.
A name the table lacks collapses every page.
`QDialTableRead` pairs the names with the pages in the catalog's order.
The Sync group draws the page named `PDialOutpost`, since `Sync` is a verb and names no page.
`QDialSettingsRefine` paints the listing switch, the notebook title and the depot folder from the stored settings.
The switches listen on `Click`, which setting `IsChecked` in code does not raise.
So a painted switch writes nothing back at all.
Ticking the listing switch makes the ledger save the raw switch.
The window's envoy goes with it, so a failed save is shown.
Enter in the notebook field hands the typed title to the ledger and shows the title it stored.
Every other key is left to the field.
Leaving the notebook field saves the typed title the same way Enter does.
A title that did not change raises no bulletin, so leaving an untouched field costs nothing.
`QDialNotebookRefine` shows the stored title, trimmed and with a blank one already turned into `Llyn`.
A failed save answers null and leaves the field alone, since the ledger has already repainted it.
Enter in the depot field, or leaving it, saves the typed folder through `CLedgerDepotSave` and shows the folder it stored.
The depot picker button makes the ledger ask for a folder through `CLedgerDepotChange`, and `QDialDepotRefine` shows the folder it answers.
A null answer, a refusal or a cancelled dialog leaves the field alone.
The folder button makes the ledger open the workspace folder.
The width button makes the posture drop the stored panel widths.

## `QLedgerItem`

Presentation item for one setting group row in `PLedger`.
It carries the group title the row shows and the child name the card is found by.
The child name is identity and never displayed.
The title is read from the localization when the ledger is built and again when the language changes.
`QLedgerItemTitle` notifies, so a language change rewrites the row in place rather than rebuilding the catalog.
`QLedgerItemMeta` is the one-line summary of the values the group holds, shown under the title.
It changes whenever a control of the group is changed, so it notifies.
The ledger writes it instead of refilling the list, so the catalog keeps its rows in place.
`QLedgerItemChosen` marks the group shown on the right, painted as an accent edge.
The panel sets it on every row when a card is shown, so exactly one row carries it.

## Inline notes

### QSettings.cs: `private readonly List<QLedgerItem> _qSettingsList = [];`

One row item per settings page, kept across states so a row keeps its chosen mark.

### QDial.cs: `private CEnvoy _qDialEnvoy = null!;`

The window's envoy, which reports a failed epithet save or folder open.

## Stamp

Commit: `2f9a06b8eb95`
- `QDial.cs` `14cbb17d73ece180`
- `QLedgerItem.cs` `24868b3c638df739`
- `QSettings.cs` `4fbb19ea101a923b`
