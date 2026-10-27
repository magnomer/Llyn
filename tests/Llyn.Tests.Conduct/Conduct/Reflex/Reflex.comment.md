# Reflex

This folder holds the reflex row rules and the respelling helpers' reflex scan.
It also holds the More readings opening.

## `TAtelierRespelling`

Covers the reflex rules the respelling helpers own, over a fake reflex port.
A reflex stands between slashes whenever its language is phonemic, and bare otherwise.
A reflex row answers ready to show, its text and fold mapped from the same engine call.
Rows of two languages each lead their own run.

## `TReflexOpening`

Covers the "More readings" opening of the reading view and the editor on a real workspace.
The engine stores it per entry, so most cases read it back through the engine relay.
The reading view writes through `CDisplaySound.CDisplayReflexToggle`, the editor through `CKindred.CKindredSpread`.
Each entry keeps its own opening, and an absent entry reads closed.
Both gates store the opening and answer true, and the reading view hears the fold bulletin once.
An editor on the same entry then reads the opening from the store.
With no entry shown or held, the gate answers false and stores nothing.
A port that faults the write shows `Reflex.SpreadFailed` once, and the gate answers false.

## `TReflexRow`

Covers the reflex row rules, namely the label keys, the lead, the hidden row and the typed copy.
A blank name keys `Reflex.` alone.
The first row of each run of one language leads, and a later run of the same language leads again.
Languages compare as stored, so a trailing space opens a run of its own.
Only a folded row under a closed fold is hidden.
A typed cell changes only that cell, and the label key follows a typed language.
The held row stays as it was, and the lead is kept.

## Stamp

Commit: `9377e657b395`
- `TAtelierRespelling.cs` `060c6d6e0be58f03`
- `TReflexOpening.cs` `817d257866410ca8`
- `TReflexRow.cs` `c78dd20b873ac79d`
