# CEntry.cs
Hash: `1103d0016f738c87`

## `public sealed class CEntry`

The head fields of the entry an editor's desk holds, with their reads and gates.
They cover the headword, reading, note, language, unit and etymons.
It also announces each draft the desk prepares, so every driver writes its controls from one event.
It announces the held draft again for the held entry's card, reflex-list or editor-box fold bulletin.
It hands each field edit to a quill over the held tenure.

## `private readonly CDesk _cEntryDesk;`

The editor's desk, whose held tenure every read and edit goes through.

## `private readonly LDraftPort _cEntryDraftPort;`

The draft port the chip quill reads the cards' link targets through.

## `private readonly LMediaPort _cEntryMediaPort;`

The media port resolves each prepared draft's image addresses and video screens.

## `private readonly LCardPort _cEntryCardPort;`

The card port the held entry's folded card ids are read through.

## `private readonly LSettingsPort _cEntrySettings;`

The settings port a refused fold read words its notice through.

## `private readonly CEnvoy _cEntryEnvoy;`

The editor's envoy, through which a refused fold read shows its notice.

## `private readonly CLedgerNoticed _cEntryNoticed;`

The atelier's repaint memory, so a lasting fold read fault shows once until the user acts.

## `internal CEntry(CDesk desk, LDraftPort drafts, LCardPort cards, LMediaPort media, LSettingsPort settings, CEnvoy envoy, CLedgerNoticed noticed)`

Only the editor builds it, over its own desk and the ports the editor was handed.
It listens to the desk's prepared drafts from the start, so no draft is prepared unheard.

## `public event Action<CEntryDraft>? CEntryDraftChanged;`

The held entry was read again, so a driver writes its controls from it.
It carries the content the desk prepared, never a second read of the draft.
The chip quill resolves the cards' link targets for that same content, so every card paints its links ready.
Each card also carries its stored fold and whether it can fold.
A fold bulletin on the held entry raises it again with the same draft and fresh folds.

## `public string CEntryLanguage`

The held draft's language, or empty while no draft is held.

## `private LTenure? CEntryTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.

## `internal void LEntryObserverAttach(Action<Action> marshal)`

Hears the fold bulletin of the held entry and announces the held draft again through the editor's marshal.
So a fold made in the reading view or another editor repaints this editor's chevrons.
The vigil keeps the handler and puts it on every tenure the desk starts.
A draft never stored hears nothing, since none of its cards can hold a fold yet.
A fold is not an edit, so the bulletin never touches the tenure or its undo.

## `internal CEntryDraft LEntryDraftRead(LTenure held, LDraft draft)`

Shapes `draft` for the editor view through the folio, with its links and its folds.
The prepared draft and the fold bulletin both shape through here, so the two never drift.
It is internal so the test relay reads the held draft the way the event hands it.

## `private void LEntryDraftShow()`

Announces the held draft again, read off the held tenure, after a fold changed.
A desk holding no draft announces nothing.

## `private IReadOnlySet<long> LEntryFoldRead()`

The ids of every folded card of the held entry, in one read for the whole draft.
A draft never stored answers an empty set.
A refused read shows `Fold.ReadFailed` once until the user acts.
It answers an empty set, so the cards still draw.

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
