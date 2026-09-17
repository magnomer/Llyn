# TEngineVista.cs

## `public sealed class TEngineVista`

The vista's promises, one fact per thing a browse panel relies on.
Each fact starts a vista on a fresh workspace and drives it through the relays alone.

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

## `public void PronunciationFind_VistaOrder_SortsRows()`

The pronunciation rows come in the vista's ordering, here headwords reversed.

## `public void DiweiFind_VistaUsageOrder_CountsFirst()`

Under the usage ordering the onset anchored by more entries lists first.
Under the name ordering it lists second.

## `public void TagFind_VistaQuery_MatchesName()`

Only the tags whose text matches the vista's query are listed.

## `public void VistaOrderSet_ChangedOrder_RaisesVistaBulletin()`

A changed ordering raises one vista bulletin carrying the vista's own id.

## `public void FavoriteFind_VistaTwins_NumbersRows()`

The favorites vista find answers vista rows, twins numbered by entry id as the library does.

## `public void PronunciationFind_VistaTwins_NumbersRows()`

The phonology vista find fills each row's twin name, so the panel numbers nothing itself.

## `public void VistaOrderSet_SameOrder_RaisesNothing()`

Setting any of the four to what it already is raises no bulletin, so a repeated click re-lists nothing.

## `private static LEntry TVistaEntryCreate(LEngine engine, string headword, string language)`

Stores one entry with one meaning in the given language, the least an entry needs to be listed.

## `private static void TVistaDiweiPlace(LEngine engine, TWorkspace workspace, string language, string character, string initial)`

Stores one entry for the character, places the character under one onset, and anchors the entry's reflex to it.
The anchor is what the onset's entry count is tallied over, so a placement alone counts nothing.

## `private sealed class TVistaObserver : LObserver`

Collects the ids of the vista bulletins raised, ignoring every other subject.
