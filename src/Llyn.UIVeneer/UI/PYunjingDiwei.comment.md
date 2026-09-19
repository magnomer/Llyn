# PYunjingDiwei.cs

## `public partial class PYunjing`

The category page of the rime table panel.
Shown when an onset or a rime is chosen, hidden when an entry is opened.

## `private LDiwei? _pDiweiShown;`

The category the page shows, or `null` while the reading view stands.

## `private void PDiweiAttach()`

Hands the page the engine, routes its character chips to the window and its switch to the tally choice.

## `private void PTallyChange(bool respelled)`

Persists the switch's choice through the engine and fills the page again, so the tally lines change set.

## `private LDiwei? PDiweiFind(string kind, string key)`

The category of the panel's language by kind and key.
`null` when the store has none or the read fails.

## `private void PDiweiShow(LDiwei diwei)`

Clears the entry shown, then puts the page in the reading view's place and fills it.

## `private void PDiweiHide()`

Drops the page and gives the reading view its place back, unless the editor holds it.

## `private void PDiweiLoad()`

Reads the category's placements, the hypothesis and the tallies, and hands the page the grouped rows.
The switch shows only while respelling is on for the language, and the IPA set prints while it is hidden.
The set printed is read from the persisted settings, so the choice is remembered across pages and runs.
Called again on every panel load, so a fetch that lands refreshes the page.

## `private void PDiweiEntryShow(string character)`

Opens the entry of a clicked character in the library, after confirming an unsaved draft may be left.
