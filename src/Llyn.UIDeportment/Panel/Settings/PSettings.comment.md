# PSettings.cs

## `public partial class PSettings : UserControl`

The settings panel as a control: what it is made of, and the stored choices it opens on.
What each choice costs — a catalog swap, a whole different workspace — lives in the files beside this one.

## `public PSettings()`

Loads the panel's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the panel, so its named parts answer `FindName`.
It names the language box's value path, sets the three icons and subscribes every control's event.
The language box starts empty, since its items wait for the atelier.

## `private TextBox PWinnow`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PSettingsIntroduce(PWindow host)`

Puts the panel to work on the host's atelier by subscribing `CLedgerChanged`, and shows the first card.
It also subscribes the posture's linked switch, so the summary row follows a tick.
The ledger raises the state on every open and after every settings or workspace change.
The ledger rows get their fill through `QLookItemAttach` before the first state arrives.

## `private void PLocalizationRefine(IReadOnlyList<KeyValuePair<string, string>> languages)`

Fills the language box with one item per interface language the ledger lists.
Each item shows the native name and carries its language code as `Tag`.
The markup lists no language, so a new catalog file needs no edit to either side.

## `private void PSettingsRefine(CLedgerState state)`

Paints the whole panel from one ledger state.
The interface texts are applied first, so everything painted after reads in the stored language.
The language box is built once, from the first state.
Setting the language box raises its selection change, and the handler writes the value back.
That write leaves the record equal, so the engine neither saves nor raises a bulletin.
The switches listen on `Click`, which setting `IsChecked` in code does not raise.
So a painted switch writes nothing back at all.
The linked switch is no settings field, so it is read from the posture.

## Inline notes

### `private PWindow _pSettingsHost = null!;`

The window this panel sits in.
Its envoy asks before a workspace change throws typed work away.
It also owns the other panels that change moves onto the new workspace.

### `private CAtelier PSettingsAtelier => _pSettingsHost.PWindowAtelier;`

The host's atelier, whose gates every settings file calls.

### `private QPosture PSettingsPosture => _pSettingsHost.PWindowPosture;`

The host's GUI-only posture, read for the linked switch and cleared by the width reset.
