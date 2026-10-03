# LEngineStaff.cs
Hash: `f77a72effef4352b`

## `internal sealed record LEngineStaff(...)`

The record keeps every clerk and engine helper that belongs to the current rig.
The engine swaps one record when the workspace changes.
Each facade reads its concern from this record.

**Parameters**

- `LEngineStaffDraft` manages held drafts.
- `LEngineStaffChronicle` manages draft history.
- `LEngineStaffCourt` manages links from a held draft to a target not yet stored.
- `LEngineStaffClaim` manages held claims.
- `LEngineStaffTag` manages tags.
- `LEngineStaffRegister` manages registers.
- `LEngineStaffTranslation` manages translation links.
- `LEngineStaffReference` manages references.
- `LEngineStaffExample` manages examples.
- `LEngineStaffSituation` manages situations.
- `LEngineStaffMeaning` manages meaning operations.
- `LEngineStaffMention` manages text mentions.
- `LEngineStaffUsage` manages usage records.
- `LEngineStaffVocabulary` manages vocabulary rows.
- `LEngineStaffParadigm` manages paradigms.
- `LEngineStaffPronunciation` manages pronunciations.
- `LEngineStaffTrail` manages workspace trails.
- `LEngineStaffLanguage` manages language data.
- `LEngineStaffRecording` manages recordings.
- `LEngineStaffTranscription` manages transcriptions.
- `LEngineStaffReflex` manages reflexes.
- `LEngineStaffEntry` manages entries.
- `LEngineStaffLacuna` fetches missing inflected forms and records the lacunae left.
- `LEngineStaffFrequency` manages frequency data.
- `LEngineStaffOutcome` runs the commit round of an entry draft.
- `LEngineStaffAuthor` manages authors.
- `LEngineStaffFavorite` manages favorites.
- `LEngineStaffCitation` manages citations.
- `LEngineStaffFanqie` manages fanqie data.
- `LEngineStaffShengfu` manages a character's phonetic series, its 聲符.
- `LEngineStaffStem` manages stems.
- `LEngineStaffDiwei` manages 音韻地位 placements.
- `LEngineStaffScript` manages script data.
- `LEngineStaffWorkspace` manages workspace state.
- `LEngineStaffMarkup` translates an entry to and from markup.
- `LEngineStaffIntake` imports markup.
- `LEngineStaffPortrait` builds portraits.
- `LEngineStaffEnsign` manages language flags.

## `internal static LEngineStaff LEngineStaffBuild(...)`

Builds the staff in dependency order over one rig.
Fetch clerks receive the gate and the bulletin raiser supplied by the engine.
The frequency and lacuna fetches also receive the settings reader.
The identity issuer receives the stale ids, so the new workspace never issues one a tenure still holds.
