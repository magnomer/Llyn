# Settings

This folder holds the settings panel's ledger and notices.
It also holds the Joplin courier set up there.

## `TCourier`

Covers whether the courier shows Llyn as connected to Joplin, over a real workspace engine.
The token is written into the workspace's settings file before the engine starts, as a past connect left it.
A stored token reads as connected once the workspace opens, and not before.
A workspace whose settings hold no token reads as unconnected after a switch, since each workspace keeps its own.
A workspace with no settings file at all would inherit the token, so the second one gets an empty one.
A push whose token cannot be restored shows `Courier.SendFailed` and drops the token.
The courier then reads unconnected while the push still runs, and ends idle with no line.
The push facts run the engine against `TOutpostFake`, which records every cargo it is handed.
A stored token reaches the outpost as one cargo.
The cargo holds the default port, the restored token, the notebook title, a 32-character realm and the stored entries.
The entry's lookup answers its own headword.
A push with no stored token refuses with the missing refusal and hands the outpost nothing.
Attaching with a raw token answers the hidden form of the token the outpost was asked to pair.
What the batch writes to Joplin is covered with the embassy tests, not here.

## `TCourierEngineStart`

Starts an engine whose rig talks to `outpost`, with a token the fake warrant restores.
It stores one entry, `瀧`, so every push has an entry to carry.

## `TCourierSend`

Pushes through the engine with its Livery page read and an empty language read.

## `TCourierWarrantSave`

Writes a settings file holding only the language and `warrant`.
The fake warrant restores only a value marked `fake:`, so any other value reads as refused.

## `TLedger`

Covers the ledger's search and shown state over the fake rig, a fake settings port, or a workspace engine.
A blank search lists every page in order, and a switch label finds only its page.
A search nothing reads answers empty, and a later state keeps the narrowing the search left.
The Layout read words the page for the linked flag handed in.
An attached view is shown every page at once, with its titles.
The Web summary counts out of two, and the Layout summary is left to the driver.
A saved notebook title answers trimmed, and the Sync page's summary names it.
A folder chosen inside the workspace is stored as its path from the workspace root.
A folder outside the workspace answers null and shows the engine's `Refusal.PathRooted` under `Settings.SaveFailed`.
A saved epithet shows the view again with Listing on, and a detached view is shown nothing more.
The fake port's normalised language and online count reach the state unchanged.
A scanned language reads its native name, an unscanned one its code, and the epithet saves reach the port.
The custom analysis gate saves through a workspace engine and its read answers the saved state.
The saved change repaints the inflection box once in the editor and once in its reading view.
The ledger's notices and failures are covered in `TLedgerNotice`.
The view reader and the fake settings port come from `TInterfaceConduct`, shared with `TLedgerNotice`.

## `TLedgerNotice`

Covers the ledger's notices over the fake rig, a fake settings port, or a workspace engine where bulletins must flow.
The folder gate opens the folder and asks nothing, and a shell failure shows `Settings.FolderFailed`.
A user act that fails twice shows two notices, both through the folder gate and through the failure policy.
A repaint read that fails twice shows one notice, and another key still shows.
Another atelier keeps its own repaint memory, and a settings save lets the key show again.
Each of the six settings gates, custom analysis among them, shows a refused save as `Settings.SaveFailed` through the envoy.
Each refusal raises the ledger again with the settings unchanged, so the driver repaints.
A faulting settings store under a real engine shows the same notice and records the fault once.
A wrapped refusal answers its reason alone, and a bare fault answers the unexpected key with its audit file.

## Stamp

Commit: `1f3467bbe5c6`
- `TCourier.cs` `ff1c6cea2caa8869`
- `TLedger.cs` `57deaa336b8c1d12`
- `TLedgerNotice.cs` `8dfd175cb77edb8a`
