# Rehearsal

The facades for the reads the rehearsal panels show, study and statistics.
They sit apart from the tool facades, since each serves one panel rather than a tool.
Each takes the hearth, takes the gate, and relays one clerk read.
`LDeckPort` is their face, through `LDeckOutlet`, beside `LDeckFacade`.
None writes, so none raises a bulletin.

## `LStudyFacade`

The facade for the reads study practises from.
Each member takes the gate and relays the cue and stencil clerks.
It stands apart from `LDeckFacade` because study is its own panel, and that facade is at its member limit.
`LDeckPort` is still its face, through `LDeckOutlet`, so Conduct keeps one deck port.

`LEngineActiveRead()` reads the folders, the decks and the active counts under one gate.
The column then comes from one moment, as `LEngineDeckRead()` does for the deck list.
`LEngineFaceRead` answers each active card with its front and back as `LCueSide` text, pictures and sounds.
It drops the face cells, which only the export needs.

## `LScoreboardFacade`

The facade for the reads the statistics tab shows.
It stands apart from `LStudyFacade` because the statistics tab is its own panel.
`LDeckPort` is still its face, through `LDeckOutlet`, so Conduct keeps one deck port.

`LEngineCensusRead` takes the gate and relays the cue clerk's census read.
A null deck id reads the census of all decks.
It hands the clerk the local time zone, so weeks start on the user's Monday.
The zone is chosen here, at the engine edge, so Core and Application read no machine setting.

## Stamp

Commit: `121691f60459`
- `LScoreboardFacade.cs` `f75e7c75f1774e32`
- `LStudyFacade.cs` `6105a67372370bc2`
