# Sound

This folder holds the editor's sound sheet and the stored open state of its rime-book and script boxes.
It also holds the shared maps for reflex rows, respelling marks, pack fonts, frequency chips and varieties.
It stands apart from its neighbours because every shape here is ready to paint.
Conduct resolves keys, marks, fonts and verdicts, so a driver only looks keys up and decides nothing.
Engine rules stay in Core, and this folder maps their answers without adding a rule.

## `CRespelling`

The reflex rows as a driver prints them, each mapped from its engine guise.
It holds no state, so it is static and the atelier builds nothing for it.
A found reading's mark and text come ready from the errand's search instead.

`LRespellingReflexScan` over a port makes one engine read that answers every row's switch, phonemic mark, fold and shown text.
The editor and the reading view's sound area both read their rows here, so the rule has one owner.
A series member has guises the engine already read.
It builds its rows through the overload over guises, so they match the entry page's rows.

`LRespellingReflexRead` puts a phonemic language between slashes whatever the switch shows, and any other reading stands bare.
A blank region becomes null.
The text is the guise's shown text as the engine answers it, so no respelling rule lives here.

## `CRespellingMark`

How one language's readings print.
A driver compares two marks to know whether a row must be rebuilt.
A slash in the opener marks a phonemic reading, so two marks differ whenever phonemic status does.

## `CSounding`

The sound sheet of the entry the editor holds.
It covers the rime-book groups, the script rows and the paradigm slots.
It also holds the maps from fanqie, script and paradigm rows to the shapes the drivers show.
Stored-entry reads and mark gates take their entry from the desk, so drivers pass no entry id.
A fresh draft has no stored rows and cannot mark them.
Rime-cell navigation uses the draft language and does not require a stored entry.
The open state of its boxes is a separate concern, so `CFold` reads and stores it.

Ledger-backed reads have fallbacks, so a box can still draw after a fetch failure.
Their refusals show through the envoy under each read's notice key.
Mapping and font failures are not caught by those read wrappers.
A read runs on every repaint, so it shows through the atelier's repaint memory, once until the user acts.
Every mark gate announces the change, or shows the refusal under its own notice key.
A send answers a user act, so its refusal shows every time.

`LSoundingObserverAttach` is not filtered to the held entry, since fanqie rows can be shared.
A fanqie changed elsewhere refreshes the editor as a fanqie set here does.

`CSoundingFanqieRead` fetches the groups first when missing, and a refusal answers them empty.
A rebuild is offered only for a stored entry whose pack has a rime book.
A refused pack check offers none.
The glyph font is the draft language's.

`CSoundingFanqieSet` hands the held rank and the raise flag down, and the fanqie clerk resolves the new rank.
`CSoundingDiweiOpen` lets the engine name the cell kind from the initial flag, and a blank key opens nothing.

`CSoundingScriptRead` offers a rebuild only for a stored entry whose pack has script styles.

`CSoundingParadigmRead` folds the waiting check and the morphology verdict into each slot's ready text and tip.
The driver never reads them.
The headword font is that of the paradigm's own language.
The box is read through the ledger under `Display.ParadigmReadFailed`.
A refused box read shows that notice and answers a null view.
A fresh draft has no entry, so its box is null.

`LSoundingListRead` is empty for a fresh draft or a refusal.
The reading view names its rows with the same keys.

The static fanqie and script maps are shared with the lectern, so both tables draw the same shape.
The row map copies every cell the fanqie line shows, derived cells included.
The script map keeps an epoch key only when `settings` finds a wording for it.
Otherwise the epoch is empty, so a driver never looks up a key that has no text.
Epoch codes come from pack data, so they form an open set and stay a string.

The paradigm map takes each row's text and tip from `paradigms` in one call, so the rule keeps one owner.
The held flag is true for the editor, whose missing forms are held by the draft.
The status rule and its wording are Core's, applied below Conduct.

## `CFold`

Rime-book and script box states belong to one entry, not to settings or this area's memory.
Each read asks the engine for that entry's stored state.
The editor builds one over its held entry, and the reading view builds one over its shown entry.
Both modes share one rule and one owner, so the area takes an entry source, never a mode flag.
The entry source is asked on every read and write, so the area never holds a stale id.

Both boxes default to closed without a stored entry or after a refused storage read.
`LFoldCheck` makes no engine read without an entry.
Storage failures use `Box.SpreadReadFailed` through repaint memory and answer closed.

`CFoldFanqieSpread` answers true when the port returned normally, not when a row changed.
The engine's fold bulletin lets the editor and the reading view repaint from storage.
So a fold in either mode folds both.
Without a stored entry, false is returned without a write.
A write failure shows `Box.SpreadFailed` and returns false, allowing the driver to restore the switch.
`CFoldScriptSpread` has the same verdict, absent-entry refusal and failure notice.

## `CFont`, `CFontRole`, `CFontSlant`

`CFontRead` is the one font rule, shared by every area that reads a pack's typography.
The engine record drops a blank family and an unusable size, so a driver sets only usable values.
A blank language passes on unread, since the engine answers it a font with nothing set.
A refused read is not caught here and reaches the caller.

The slant maps from the engine's style name, which the engine already lowers.
Any other name keeps the theme's slant, so a driver switches on a closed set.
`CFontRole` carries the engine's names, because the map switches name by name.
An unknown role throws, so a role added on one side alone fails loudly.

## `CFrequency`, `CFrequencyTier`

The frequency of an entry, as the frequency chip shows it.
A missing frequency is a null shape, so the chip hides.
Conduct raises a negative band or spare count to zero, since a chip cannot draw fewer than no stars.
The tier is unknown at band zero or for a name Conduct does not know.
So the chip always finds its brush and its name.
Every tier has a localization key, so the chip never shows a missing name.

## `CVariety`

One variety of a reading, as a driver labels it or flags it.
Conduct chose both keys, so a driver only looks them up.
The label key is `Variety.` and the name, and the driver falls back to the name.
The flag key comes from the engine, so the ensign's key format has one owner.

## Stamp

Commit: `c2f335c331fe`
- `CFold.cs` `1d4f1209ad356aac`
- `CFont.cs` `505e7e889f713f67`
- `CFontRole.cs` `4f4b9a9bc58621ca`
- `CFontSlant.cs` `1d5f688fcbe0df05`
- `CFrequency.cs` `fc6d2a52dde7ba9e`
- `CFrequencyTier.cs` `817eb221e8ae0c18`
- `CRespelling.cs` `ece0c93bdb9fa88a`
- `CRespellingMark.cs` `0e039bb6dba672d3`
- `CSounding.cs` `e59be986f3b94b4f`
- `CVariety.cs` `16ce6347974fd167`
