# CEntry.cs
Hash: `4e445b9e61f7c95c`

## `public sealed class CEntry`

The head fields of the entry an editor's desk holds, with their reads and gates.
They cover the headword, reading, note, language, unit and etymons.
It also announces each draft the desk prepares, so every driver writes its controls from one event.
It hands each field edit to a quill over the held tenure.

## `private readonly CDesk _cEntryDesk;`

The editor's desk, whose held tenure every read and edit goes through.

## `private readonly LDraftPort _cEntryDraftPort;`

The draft port the chip quill reads the cards' link targets through.

## `private readonly LMediaPort _cEntryMediaPort;`

The media port the folio reads each prepared draft's recordings through.

## `internal CEntry(CDesk desk, LDraftPort drafts, LMediaPort media)`

Only the editor builds it, over its own desk and the ports the editor was handed.
It listens to the desk's prepared drafts from the start, so no draft is prepared unheard.

## `public event Action<CEntryDraft>? CEntryDraftChanged;`

The held entry was read again, so a driver writes its controls from it.
It carries the content the desk prepared, never a second read of the draft.
The chip quill resolves the cards' link targets for that same content, so every card paints its links ready.

## `public string CEntryLanguage`

The held draft's language, or empty while no draft is held.

## `private LTenure? CEntryTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.

## `public string CEntryPronunciationRead()`

The primary reading as the field shows it, respelled when the pack respells.

## `public IReadOnlyList<CTranslationTarget> CEntryEtymonRead()`

The etymons of the held entry, as the etymology field lists them.

## `public void CEntryHeadwordSet(string text)`

Hands the typed headword to an entry quill over the held tenure.
A desk that holds no tenure or fills its view takes no edit.

## `public void CEntryPronunciationSet(string text)`

Hands the typed reading to a pronunciation quill over the held tenure.
A desk that holds no tenure or fills its view takes no edit.

## `public void CEntryNoteSet(string text)`

Hands the typed note to an entry quill over the held tenure.
A desk that holds no tenure or fills its view takes no edit.

## `public static bool CEntryNoteCheck(string text, string note)`

Whether the held note already equals what the typed text resolves to, as the entry quill judges.

## `public void CEntryLanguageSet(string language)`

Hands the chosen language to an entry quill over the held tenure.
A desk that holds no tenure or fills its view takes no edit.

## `public void CEntryUnitSet(string key)`

The user picked a unit from the menu.
Picking the unit already held clears it, a rule the entry quill keeps.
The menu itself is read with the part-of-speech field, through `CMarkerUnits`.
A desk that holds no tenure or fills its view takes no edit.
