# QSettings.cs
Hash: `e2eb3799d7e2da76`

## `internal sealed class QSettings`

The settings panel's driver.
It owns the group catalog, the chosen group's page, and the stored choices it opens on.
Each choice is driven by its own driver in `Panel/Choice`, which hears and paints that choice's controls.
What each choice costs, such as a catalog swap or a whole different workspace, lives in those drivers.

## `internal QSettings(FrameworkElement surface, QLayout layout)`

Takes the veneer's page as its surface, which the window pulls by contract ID.
It builds one choice driver per concern over the surface, and each wires its own controls.
The linked-panels switch is the exception, since it is a setting on the window's one `QLayout`.
That keeper is handed in, and it wires the switch when the panel introduces it.
Nothing else is wired before the window introduces it.

## `private TextBox QSettingsWinnow`

Each named part of the markup is pulled by its contract ID, which keeps the markup's names.

## `internal void QSettingsIntroduce(QWindow host)`

Sets the search hint and the two page icons, and subscribes the panel's own events.
It introduces each choice driver to only the Conduct area it calls, or to the posture.
The Joplin driver gets the ledger for its port and the courier for its two actions.
Each settings switch also receives the window's envoy, for a failed save.
This happens before `CLedgerChanged` is subscribed, so the first state finds them ready.
It also subscribes the posture's linked switch, so the summary row follows a tick.
It subscribes `CCourierChanged` through the surface's dispatcher, though the gates already resume on the UI thread.
So a gate called off the UI thread would still paint safely.
The ledger raises the state on every open and after every settings or workspace change.
The ledger rows get their fill through `QLookItemAttach` before the first state arrives.
It then shows the first card.

## `private void QSettingsRefine(CLedgerState state)`

Paints the whole panel from one ledger state.
The interface texts are applied first, so everything painted after reads in the stored language.
Each choice driver is handed its own value from the state, and the linked switch reads the posture.
The Joplin port arrives as ready text, so a rejected entry is painted over by the stored port.
The switches listen on `Click`, which setting `IsChecked` in code does not raise.
So a painted switch writes nothing back at all.

## `private void QSettingsEpithetObserve(object sender, RoutedEventArgs e)`

The listing switch was ticked, so the ledger saves the raw switch.
The window's envoy goes with it, so a failed save is shown.

## `private (string QSettingsChild, StackPanel QSettingsPage)[] QSettingsTableRead()`

Pairs each group's name with the page the markup draws for it.

## `private void QSettingsDialRefine(string child)`

Shows the card of one group and hides the others, and marks that group's row chosen.

## `private void QSettingsFolderObserve(object sender, RoutedEventArgs e)`

The folder button was clicked, so the ledger opens the workspace folder.

## `private void QSettingsWidthObserve(object sender, RoutedEventArgs e)`

The width button was clicked, so the posture drops the stored panel widths.

## `private void QSettingsLedgerRefine(CLedgerState state)`

Paints the catalog from one ledger state.
That covers every page's row, the narrowing the state carries, and the Layout summary.
The narrowing rides on the state, so a settings change keeps a narrowed catalog narrowed.

## `private void QSettingsPageRefine(CLedgerPage page)`

Writes one page's title and summary onto the row paired with it by the page's name.
A page with no row yet gets one, appended in the ledger's page order.

## `private void QSettingsMetaRefine()`

Writes the Layout row as `CLedgerMetaRead` words it for the linked flag the posture holds.
The linked switch calls it too, since that flag is GUI-only state no ledger state carries.

## `private void QSettingsFindRefine(CLedgerShown shown)`

Lists the rows of the pages `shown` names, and shows the empty text when it says none is shown.
Hidden rows leave the items source rather than collapsing, so the frame keeps its sibling spacing.
The chosen row may leave the catalog while its card stays, since the card is the dial's to swap.

## `private void QSettingsRowRefine(object sender, RoutedEventArgs e)`

A click on a row shows the card of that row's group.
Marking the row chosen is left to the card showing, so the first card marks its row the same way.

## `private void QSettingsItemRefine(FrameworkElement container, object item, string? _)`

Fills one ledger row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
A new title or summary on the item refills the row the same way.

## `private void QSettingsWinnowObserve(object sender, TextChangedEventArgs e)`

The search field's text changed, so `CLedgerFind` hears the raw text and answers the pages it shows.
Conduct keeps the text and matches it, so a console searches the same way.

## Inline notes

### `private readonly List<QLedgerItem> _qSettingsList = [];`

One row item per settings page, kept across states so a row keeps its chosen mark.

### `private QWindow _qSettingsHost = null!;`

The window this panel sits in.
Its envoy asks before a workspace change throws typed work away.
It also owns the other panels that change moves onto the new workspace.

### `private CAtelier QSettingsAtelier => _qSettingsHost.QWindowAtelier;`

The host's atelier, whose gates every settings file calls.

### `private QPosture QSettingsPosture => _qSettingsHost.QWindowPosture;`

The host's GUI-only posture, read for the ledger's linked summary and cleared by the width reset.
