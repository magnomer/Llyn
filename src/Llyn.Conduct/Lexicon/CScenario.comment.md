# CScenario.cs
Hash: `4034ea22cef6f454`

## `public sealed record CScenario(CSituationDraft CScenarioDraft)`

The repertoire's edit area, ready to fill from the held Situation.
It answers a ready line for each of the Situation's three fields.
A field's line carries its text and its hint key, so the driver only looks keys up and paints.

**Parameters**

- `CScenarioDraft`: the held Situation, whose picture and video rows the area draws.

## `public CScenarioLine CScenarioTitle`

The title's line, asking for a title while it is empty.

## `public CScenarioLine CScenarioKind`

The kind's line, asking for a kind while it is empty.

## `public CScenarioLine CScenarioDescription`

The description's line, asking for a description while it is empty.

## `internal static CScenarioLine LScenarioTitleRead(string text, bool uncertain)`

The title's line for a text, from the draft or as typed.
The title asks with the untitled text the catalog uses, so an empty title reads the same in both places.

## `internal static CScenarioLine LScenarioKindRead(string text, bool uncertain)`

The kind's line for a text, from the draft or as typed.

## `internal static CScenarioLine LScenarioDescriptionRead(string text, bool uncertain)`

The description's line for a text, from the draft or as typed.

## `internal static CSituationDraft LScenarioBlankRead()`

The empty Situation the area shows while no draft is held.
The repertoire raises it instead of null, so the driver keeps no default of its own.

## `private static CScenarioLine LScenarioLineRead(string text, bool uncertain, string key)`

An unknown field asks with the unknown mark, and any other field asks with its own key.
