# Inflection

This folder holds the tests of the inflection fetch, including an entry an editor holds, and the regularity analysis.
They also cover the paradigm and paradigm view the engine answers.

## `TEngineInflectionCell.cs`

It covers the inflection fetch over a fixture pack whose verb paradigm states two-value cells.
The fixture's morphology source names each reading by the cell key, as the Spanish pack does.
The second cell is listed out of code order, so its key is read sorted.
Each fact runs the fetch in a first engine and reads the outcome in a reopened one.
Both cells found store every value of the cell, and nothing is recorded as missed.
When only the first cell is found, only the second reads unknown.
Its lacuna row names the cell by its key.
A missed cell survives a reopen, so a new start asks the web nothing.

## `TInflectionCellRun`

The run saves a fixture verb, runs one fetch and disposes the engine, so every fact shares one setup.
The frequency fill is off, so the stub client only sees the morphology request.
The wait ends once the handler was asked and the fetch no longer stands.
The client leaves the handler alive on dispose, since the reopened engine sends through the same one.

## `TInflectionPackCreate`

The pack is written before the workspace opens, so the engine imports its values.

## `TEngineInflectionFetch.cs`

It covers the inflection fetch over the shipped English pack against a stub client.
The stub body is shaped as the Wiktionary REST HTML the pack's morphology source reads.
The entry is a bare `Verb`, since the pack lists no verb subtypes.
A body naming both verb forms leaves both slots specified and raises the inflection bulletin once.
A body naming only the past leaves the past participle unknown rather than unspecified.
A body naming no form marks every slot unknown and still raises the bulletin, so the marks show.
A source that cannot be reached leaves both slots unspecified and stores nothing.
That miss is asked only once per session.
A hand set then forgets that miss, so the next start asks again.
A second start while a fetch is pending sends no second request.
An old workspace row with no morphology does not stop a fetch, and the next successful fetch removes it.
With the setting off, a start asks nothing and writes nothing.
A held draft with an unsaved headword asks nothing, and the start asks once that draft is gone.
A headword change while a fetch is pending discards the forms instead of storing them under the new headword.
A hand set of the inflections forgets the unknown mark, so the slot reads unspecified again.
An unknown slot survives a reopen of the same workspace, so it does not ask the web again.
An entry no source could be reached for is asked again after a reopen, since nothing was stored.
A hand set after the reopen deletes the lacuna rows, so the web may be asked again.

## `InflectionStart_BothFormsFound_MarksSlotsSpecifiedRaisesOnce`

The frequency fill is switched off first, so the stub client only sees the morphology request.
The short wait after the bulletin lets a second bulletin land if the fetch wrongly raised twice.

## `InflectionStart_EntryHeldInDraft_AsksNothing`

The draft gets an unsaved headword, since a draft still matching the stored entry no longer blocks a fetch.

## `InflectionStart_SettingOff_AsksNothing`

The setting is written to the workspace before the engine starts, since the engine reads it on construction.

## `EntryUpdate_HeadwordChangedMidFlight_DiscardsForms`

The client holds its answer behind a gate, so the headword changes under the pending fetch.
Nothing observable follows, so a short wait stands in for the fetch settling.

## `TInflectionMissRun`

It runs one fetch that misses in a first engine and disposes it.
The first client leaves the handler alive on dispose, since the reopened engine sends through the same one.

## `TInflectionCountCheck`

It polls the handler until it has seen the count of requests or the patience runs out.
The caller then reads the settled state.

## `TEngineInflectionHeld.cs`

It covers the fetch of an entry an editor holds unchanged right after its save.
The body is the Spanish conjugation table cut from the real Wiktionary REST page for `vivir`.
Its long lines are wrapped only where the shipped readings allow white space.
The shipped Spanish pack reads all 64 cells from it.
The client holds its answer behind a gate, so the save's fetch is still pending when a draft is held.
That draft is the editor reopening on the saved entry, so it matches the stored entry.
The reading view then starts the entry's fills, as opening it does.
The expanded view shows 64 forms, and no cell carries a stand-in tip.
The store keeps 64 forms, and the held draft carries them too.

## `TEngineInflectionRegular.cs`

It covers the analysis stored when a form is written, and its renewal when the rule book changes.
Each fact saves one form through the entry save path, with the morphology fetch off.
An exactly predicted Spanish form stores its prediction, empty marks and book stamp, and is regular.
The irregular tuve departs from the predicted teni in two letters.
So it is stored with those marks and is not regular.
French has no rule book, so the paradigm's own rule still sets the flag.
The three analysis columns stay `NULL` for French.
The archive reads a stored prediction, marks and stamp back unchanged.
An empty mark list reads back empty, and `null` values read back as uncovered.
A stored row whose stamp differs from the book's is analysed again when the background check starts.
A stored row with no stamp, as written before the book or imported, is stale and analysed the same way.
Both analyses come from the stored text, and no request reaches the web.
A form whose headword fits no kind of the book is stored uncovered, with the book's stamp and no prediction.
With its stamp cleared, the first check analyses it and the paradigm's own rules set its flag.
The second check finds the book's stamp and leaves the row alone, so a flag set by hand survives.

## `TEngineParadigm.cs`

It covers the engine's answer to which forms an entry is expected to have.
Most tests save an entry against the shipped English pack.
So the paradigms and the morphology rows are the real ones and not a fixture.
A part declaring a paradigm shows its slots in declared order, all unspecified while nothing is stored.
An appended inflection carrying a slot's morphology row makes that slot specified and leaves its neighbour alone.
A retired subtype typed by name is a custom part, so it shows no slots.
The packs list bare roles only, so no paradigm reaches a name the pack no longer declares.
A custom part declares no paradigm, and an entry the workspace does not hold answers nothing.
An entry with several parts lists each part's slots in stored part order.
A pack code resolves to its language's morphology row, and an unknown code or language answers null.
The shipped noun pattern accepts a regular plural regardless of case, and a paradigm stating no pattern answers false.
One form carrying both morphology rows of a verb fills both slots.
The form is held once, and both slots answer the same stored inflection.
A stored regular plural is dropped from the display rows while the read still lists it.
An irregular plural stays a display row, still specified.
The English verb slots keep both stored forms, since they declare no regularity rules.
An unfilled slot is never regular, so it shows.
Turning the morphology switch off while a fetch waits cancels it.
The forms the source later returns are discarded and the slots stay unspecified.
`TParadigmFetchBody` is held behind a gate so the switch can flip mid-flight.
`TParadigmInflectionSave` adds forms through the entry update, which regrades every form it stores.

## `TEngineParadigmView.cs`

It covers the inflection view the engine answers for an entry.
It also covers how the list and the font language change beside it.
Fixtures disable morphology fetching and save entries, with forms added only where needed.
With custom analysis off, a Spanish verb answers both default views from the pack's layout.
The collapsed view has nine lines and no headers.
The expanded view has eleven lines under six header keys.
Labels and headers come through as localization keys.
A stored regular form shows unmarked text with no tip.
An existing slot without a form has the lost tip.
The held rule lives in Core, so the engine test keeps it covered.
With held on, a lost cell carries the held tip with the ellipsis.
The irregular tuve is stored through the write path with its two marked letters.
The view answers one mark over the whole word, since no letter aligns past the predicted root.
A cell with no form during a fetch shows the ellipsis with the pending tip.
The slotless first-singular imperative instead has empty text and no tip.
Custom analysis is on by default.
Both views then follow the custom sheets, which open with the future.
The irregular tengo splits after teng, with the inserted g kept in the root.
The root is marked as a whole, and the ending stays unmarked.
With analysis off, both views open with the indicative, and the subjunctive precedes the imperative in the expanded view.
Collapsed forms and the expanded tuve show no marks, while the stored row keeps its marks.
French has no layout, so there is no view.
The list drops every slot of the layout's part, even an irregular one the display would otherwise show.
With every cell as predicted no display slot remains, yet the font language still names Spanish.

## Stamp

Commit: `c37ca1837ded`
- `TEngineInflectionCell.cs` `b84f4974489b1e07`
- `TEngineInflectionFetch.cs` `005f94240360259f`
- `TEngineInflectionHeld.cs` `30cac3cd224a8078`
- `TEngineInflectionRegular.cs` `30dfcb246d3dbb7a`
- `TEngineParadigm.cs` `16fddf3b493beeab`
- `TEngineParadigmView.cs` `b0d30c06175ac036`
