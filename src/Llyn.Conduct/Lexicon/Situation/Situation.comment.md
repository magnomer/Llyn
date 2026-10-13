# Situation

This folder holds one Situation as the repertoire's edit area and its read page show it.

## `CScenario`

The repertoire's edit area, ready to fill from the held Situation.
It answers a ready line for each of the Situation's three fields.
Each line carries its text and its hint key, so the driver only looks keys up and paints.
The title asks with the untitled text the catalog uses, so an empty title reads the same in both places.
`LScenarioBlankRead` is the empty Situation the area shows while no draft is held.
The repertoire raises it instead of null, so the driver keeps no default of its own.
`LScenarioLineRead` has an unknown field ask with the unknown mark, and any other field ask with its own key.
The three read functions serve both the draft and the typed text.

## `CScenarioLine`

One field of the repertoire's edit area, as its driver fills the field and its unseen twins.
The text is empty when nothing legible is written.
`CScenarioLineVacant` is true when the field is empty, so its placeholder shows.
`CScenarioLineWording` is the key a measuring twin words instead of the text, while the field is empty.
It is null while the text itself shows.

## `CSituation`

One Situation ready for the repertoire's read page, the vignette.
Every field arrives worded and every row arrives chosen, so the driver only looks keys up and paints.
The title is worded untitled and muted while it was never written.
The kind and description carry no wording while never written, so the chip and the section hide.
The Markdown blocks are drawn while the description has no key.
The picture and video rows are those whose location is filled, in the order the user keeps them.

## Stamp

Commit: `c2f335c331fe`
- `CScenario.cs` `4034ea22cef6f454`
- `CScenarioLine.cs` `e72ee0d291086452`
- `CSituation.cs` `e5d67ef144ab7497`
