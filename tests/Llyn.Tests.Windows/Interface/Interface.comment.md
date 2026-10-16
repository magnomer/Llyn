# Interface

The relays that let the Windows tests reach Deportment's internals and Core.Windows.
They carry no test logic of their own, so a test keeps its arrange and assert beside them.

## `[assembly: CollectionBehavior(DisableTestParallelization = true)]`

The Windows tests build workspaces from the portable suite's fixture, whose disposal clears every SQLite pool.
The portable suite's own switch covers only its assembly, so this one repeats it here.
Two workspaces alive in parallel let one disposal close a connection the other is opening.

## `TInterfaceCapsule`

These relays for Deportment's Capsule let its file contract be checked without a window.

## `TInterfaceDeportment`

The relays for the deportment classes the Windows tests drive.
It is a class of its own rather than a part of `TInterface`.
Some relays construct bare WPF surfaces to expose production painters and handlers.
Their caller runs them on an STA thread.
The rime-book and script switch relays hand a WPF toggle through.
The index item and caret key relays reach internal statics and make no WPF object.
The scheme relay also reaches an internal choice refresh and makes no WPF object.
The internal drivers and cards are handed back as `object`, so the caller stays out of internal types.
The reflex type relay copies the held row with the typed text, as the gate does.
It then hands the copy to the row's refine.
The etymology item relay shows Ancestor rows into an item list with no relations or tongues.
The lectern etymology relay builds one item from a row and its relation.
It then paints the item's face in a given container.
The card position attach hangs one card in a bare list with only the number box in its row.
It lays the list out so its row exists, then hands the row painter to the real item watcher.
It returns the box the painter finds by its part name.
The card row relay hands transcription rows to the shared row matcher with the sheet's key and painters.
The localization and analysis relays build the settings language choice and the custom analysis switch over a given surface.
Each surface holds a WPF combo box or toggle.

### `TLecternCreate`

The reading reflex section subscribes the hinge itself, so the test wires no observer.

### `TCadenceCreate`

It builds `QCadence` over a surface carrying the rime-book and script boxes under their markup names.
So a switch click reaches the real fold gate through the real driver.

### `TLecternSoundCreate`

It builds the reading view's sound section over the display's sound and fold areas.
So a reading-view box switch click reaches the display's real fold gate through the real driver.

### `TCardFoldCreate`

The card's peek is the definition, as a meaning list picks it.
The draft also names the card's id.

### `TCardHingeAttach`

It builds a card driver and introduces the editor's field, list and caret facets to it.
The field drivers it never reaches are passed as null, since the row holds only a hinge.
It attaches the real card fill to the list, so a hinge click reaches the real fold gate.

### `TLookSettingRead`

It reads the fixed value the look sheet gives a property on a part of a style.
It picks the last row whose cues hold, as the look handler does.
A folded hinge holds the checked cue, and an unfolded one holds only the base cue.
It returns null when no row holds, or when the winner is no fixed `Freezable`.

### `TCardCreate`

The blank draft carries no meaning links.

## `TInterfacePhonograph`

The relays that build Core.Windows's phonograph and play a file on it.
The phonograph is handed back as the `LPhonograph` port, so the caller names no Core.Windows type.
The caller subscribes to `LPhonographFinished` itself, since an event is no production call.

## `TWindow`

The shared runner for test steps that build WPF objects.
A WPF control demands an STA thread, and the xunit thread is not one.

### `TWindowRun`

A failure on the STA thread is rethrown on the test thread with its original stack.
So an assertion or a crash inside the step fails the test instead of vanishing with the thread.
The wait is bounded, so a step that hangs fails the test rather than the whole run.

## Stamp

Commit: `ed7d18d90760`
- `TInterfaceCapsule.cs` `062207ed6f90f438`
- `TInterfaceDeportment.cs` `98defaca17184e1a`
- `TInterfacePhonograph.cs` `888cd40c37da96bb`
- `TWindow.cs` `20d60e5a8413b7cf`
