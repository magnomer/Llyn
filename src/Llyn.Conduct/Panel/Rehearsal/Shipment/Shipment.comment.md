# Shipment

This folder holds the deck export gate and the state it shows.

## `CDepot`

The deck export gate, copying `CCourier`.
It takes the deck by id, so it needs no chosen deck and stands apart from `CDeck`.
The busy value is read from the engine's workspace latch at each `CDepotRead`.
  The gate keeps no busy flag, so a run another driver started reads as busy too.
  A second run is refused by the engine with `Refusal.DepotBusy`, and the gate shows it.
The receipt line is the only state the gate owns, and `CDepotExport` is its one writer.
`CDepotChanged` tells the driver to read the state again, as `CCourierChanged` does.

## `CDepot` gates

`CDepotExport` reads the portrait label, then makes its one engine call, `LEngineDeckExport`.
It raises once the export task exists, so the driver reads the latch already started.
Every failure and refusal shows under `Deck.ExportFailed`, as `Courier.SendFailed` does.
  A failed run clears the receipt line, because it has no receipt.
The line formats `Deck.ExportReceipt`, with up to ten failed names under `Deck.Failed`.
  A translation with broken placeholders falls back to four plain numbers.
The line reads `Deck.Exporting` while a run is under way, as `CCourier` reads `Courier.Sending`.
  The text is read before the export starts, so a failed read starts no run.

## `CDepotState`

The export state as the driver shows it, with the busy value and the last receipt line.
The deck menu greys its export item while the last state was busy.

## Stamp

Commit: `2f9a06b8eb95`
- `CDepot.cs` `7a04f84913df94b2`
- `CDepotState.cs` `c9c1510f8ba2c3b4`
