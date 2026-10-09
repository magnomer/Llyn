# TInflectionSwitch.cs
Hash: `2bc53daaa347571a`

## `public sealed class TInflectionSwitch`

Covers the custom analysis switch on the Inflection settings page.
The switch runs over a real ledger from the atelier relay and a real workspace's settings.
The toggle lives on its own STA thread through the shared window runner.

## `public void LedgerChanged_Opened_ListsTheInflectionPageAfterListing()`

The opened ledger lists the Inflection page right after Listing, with its localized title.
Its summary reads on, since custom analysis is on by default.
A search for the switch label finds the Inflection page.

## `public void InflectionRefine_StoredSetting_PaintsTheSwitch(bool stored)`

The setting is saved through the gate, and the toggle starts in the other state.
The refine paints the toggle from the ledger's read, so it shows the stored setting.
No save failure is shown.

## `public void InflectionObserve_SwitchClicked_SavesTheRawSwitch()`

The toggle is set off and clicked, and the stored setting turns off.
It is set on and clicked again, and the stored setting turns back on.
So each click hands the raw toggle to the gate, and no failure is shown.
