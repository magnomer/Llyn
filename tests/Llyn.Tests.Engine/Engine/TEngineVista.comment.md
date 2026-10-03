# TEngineVista.cs
Hash: `edc90f30c4ba2d58`

## `public sealed class TEngineVista`

The vista's promises, one fact per thing a browse panel relies on.
Each fact starts a vista on a fresh workspace and changes it through the vista itself.
A fact that must find an ordering or filter again starts its vistas through the posture, which stores them.
This class covers starting a vista and listing the library's entries through it.
The other panels' finds live in `TEngineVistaList`.
The raised bulletins live in `TEngineVistaBulletin`.
The reads and filter relays live in `TEngineVistaRelay`.

## `public void EntryFind_NoEpithetHeld_CarriesAnEmptyEpithet()`

A listed entry with no epithet carries an empty one, so no reader of the row supplies a fallback.

## `public void VistaOrderSet_WorkspaceReopened_KeepsOrder()`

An ordering set on the vista is found again by a vista started on the reopened workspace.

## `public void VistaFilterSet_WorkspaceReopened_KeepsFilter()`

A filter set on the vista is found again by a vista started on the reopened workspace.

## `public void VistaStart_NoStoredOrder_UsesFallback()`

A tab with nothing stored lists by the fallback, hides nothing, queries nothing and chooses nothing.

## `public void EntryFind_VistaFilter_HidesLanguage()`

A hidden language's entries are left out of the rows.

## `public void EntryFind_VistaQuery_MatchesHeadword()`

Only the entries whose headword matches the query are listed.

## `public void EntryFind_VistaBlank_AnswersNothing()`

A blank vista with nothing typed lists no rows while a plain one lists every entry.
Typing a query makes the blank vista list its matches like any other.

## `public void VistaOrderSet_LeftTab_KeepsRightApart()`

An ordering set on the left tab is found again there and leaves the right tab on its fallback.

## `public void EntryFind_VistaTwins_NumbersByEntryId()`

Two entries sharing a headword are numbered by entry id, so the older is `(1)` even when listed second.
An unshared headword is listed unnumbered.

## `public void EntryFind_VistaChosen_MarksRow()`

The row of the selected entry is the one row marked chosen.

## `internal static LEntry TVistaEntryCreate(LEngine engine, string headword, string language)`

Stores one entry with one meaning in the given language, the least an entry needs to be listed.
The sibling classes build their entries through it.
