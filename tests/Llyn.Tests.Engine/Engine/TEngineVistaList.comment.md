# TEngineVistaList.cs
Hash: `43451cf5f867c510`

## `public sealed class TEngineVistaList`

Covers the vista finds of the favorite, phonology, yunjing, yunmu and taxonomy panels, on a real workspace.
It builds each entry through `TEngineVista.TVistaEntryCreate`.
Each find answers rows the panel shows as they come, with twins numbered and no fallback left to the panel.

## `public void PronunciationFind_VistaOrder_SortsRows()`

The pronunciation rows come in the vista's ordering, here headwords reversed.

## `public void DiweiFind_VistaUsageOrder_CountsFirst()`

Under the usage ordering the onset anchored by more entries lists first.
Under the name ordering it lists second.
The chosen onset names the language both columns list.

## `public void TagFind_VistaQuery_MatchesName()`

Only the tags whose text matches the vista's query are listed.

## `public void FavoriteFind_VistaTwins_NumbersRows()`

The favorites vista find answers vista rows, twins numbered by entry id as the library does.

## `public void PronunciationFind_VistaTwins_NumbersRows()`

The phonology vista find fills each row's twin name, so the panel numbers nothing itself.

## `public void PronunciationFind_NoEpithet_CarriesAnEmptyOne()`

A phonology row with no epithet carries an empty one, so the Conduct map needs no fallback.

## `private static void TVistaDiweiPlace(LEngine engine, TWorkspace workspace, string language, string character, string initial)`

Stores one entry for the character, places the character under one onset, and anchors the entry's reflex to it.
The anchor is what the onset's entry count is tallied over, so a placement alone counts nothing.
