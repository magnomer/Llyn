# Study

This folder holds the study gates: the study tab and the practice sitting it runs.

## `CStudy`

The study tab's gates, one sealed class owning its own chosen deck and the running sitting.
  Its deck choice stands apart from `CDeck`'s, so studying one deck never moves the deck editor.
It registers the `Study` tab as `CDeck` registers `Deck`: the station is the chosen deck and the arrival chooses it.
  The leave question always answers yes, because a sitting stores nothing.
A workspace open clears the chosen deck and the sitting, because each workspace keeps its own decks.
`CStudyChanged` tells the driver to read the roll again.
  It also follows every deck bulletin, so a write in any panel reaches the shelf without a restart.
  The bulletin comes from the engine write itself, so no panel announces another's change and nothing polls.
`CStudyRead` makes the one engine call `LEngineActiveRead` and answers a `CStudyRoll`.
  Its folders reuse `CDeckFolder`.
  Each `CStudyRow` carries the deck's active card count, 0 when the engine names none.
  A read failure shows `Deck.LoadFailed` and answers an empty roll with `Study.Empty`.
  The roll also carries the running sitting's state, or none, so one read answers the whole page.
The roll's empty-state key is chosen here, since it is interaction.
  `Study.Empty` means no deck exists, and `Study.Unchosen` means none is chosen.
  `Study.Inactive` means the chosen deck has no active card, and otherwise the roll names no key.
`CStudyEmptyKey` names `Study.Empty` for the driver's first paint, before any workspace is open.
`CStudySelect` adds a station, chooses the deck and ends any sitting, since a sitting belongs to its deck.
`CStudyStart` makes the one engine call `LEngineFaceRead` for the chosen deck with the shown labels.
  It does nothing with no chosen deck or no active card, so the roll's empty-state key stays the answer.
  A read failure shows `Study.LoadFailed`.
  It keeps only each card's front and back, by name, and drops the stored cue.
  Each engine side becomes a `CSittingSide`, its picture files resolved to addresses and its first sound kept.
`CStudySoundPlay` plays the shown front's or turned back's sound through `LEngineRecordingPlay`, as the reading view does.
  With no running card, an unturned back or a silent side it does nothing.
  A play failure shows `Sound.PlayFailed`.
`CStudyFinish` ends the running sitting.
  With none it does nothing and raises nothing.
`CStudySitting` is the running sitting or null, the driver's one check.
The constructor takes the random draw, `Func<int, int>`, from the driver.
  Conduct is a pure ring, so a random arrives from outside and never as `System.Random`.

## `CSitting`

One practice run over one deck's faces, owning their order, the place and the turned state.
Practice only: nothing studied is stored, so no gate calls the engine.
Each gate returns the ready `CSittingState`, which `CSittingRead` also answers.
  The state carries the place text `3 / 20`, the front and the back.
  It also says whether the card is turned, the run is at its end, and the order is shuffled.
  The view computes none of these.
`CSittingSideToggle` turns the card, and at the end it does nothing.
`CSittingAdvance` moves to the next card unturned, and past the last card to the end.
  At the end the front and back are empty and the place reads the last card.
`CSittingRetreat` moves back one card unturned, and from the end back to the last card.
  At the first card it does nothing.
`CSittingShuffleToggle` switches between deck order and a shuffled order, and starts the run again at the first card unturned.
  The shuffle is a Fisher-Yates walk over the driver's draw, so a seeded draw gives a repeatable order.
A turned back with no text, picture or sound names `Study.BackEmpty` as the state's blank key.

## `CSittingSide`

One side of a card as studied: its text and its picture drafts, ready for the view.
The sound file stays internal, so the view learns only `CSittingSideAudible` and asks `CStudy` to play it.

## Stamp

Commit: `121691f60459`
- `CSitting.cs` `ae88bc924a34add2`
- `CSittingSide.cs` `520a1bcf143a00de`
- `CSittingState.cs` `51da44b47c6a8c72`
- `CStudy.cs` `8466b3b72f5e50ee`
- `CStudyRoll.cs` `bf01ab3d7688aa0f`
- `CStudyRow.cs` `7a13b422ce9d4392`
