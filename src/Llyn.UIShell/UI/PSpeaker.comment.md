# PSpeaker.cs

## `public partial class PEditor`

The language an entry is written in, as the editor chooses it.
That is the dropdown of installed packs and the pill that shows the chosen one.
The flag beside it belongs here too.
The choice is what lookup, audio download, and the saved entry are all carried out in.

## Inline notes

### `private const string PWindowLanguage = "English";`

Fallback language used until a pack is chosen, and when no pack folder is present on disk.

### `private bool _pSpeakerEntry;`

Whether the current language came from a loaded entry.
A pack that is no longer on disk is still the language the entry was written in.
So the start-up fallback to the first pack leaves it alone.
Only a form standing on no entry may be moved off its language.

### `internal async void PSpeakerLoad()`

Builds the language menu.
Each row reads its flag from `PEnsign`, which every other tab reads too.
The Translation chips are redrawn afterwards, because they were built before any flag existed.
The flags may await a first-time fetch, hence async.

### `await PEnsign.PEnsignLoad(_lEngine);`

The menu waits for the shared flags rather than fetching its own.
This tab once resolved every SVG again on its own, so a flag was parsed twice.
The rows are added afterwards, in the order the packs were read.

### `if (!_pSpeakerEntry && languages.Count > 0 && !languages.Contains(_pSpeakerChoice))`

A form standing on an entry keeps that entry's language even when no pack answers to it.
The fallback is for a form that stands on nothing.
Such a form would otherwise name a pack that is not installed.
A fallback that moved the choice sends it, because the blank draft was started under the old one.

### `PMarkerLoad();`

The parts of speech on offer are the chosen language's, so a fallback that moved the choice moves them too.

### `_pSpeakerEntry = false;`

Chosen by hand, so the language is no longer the loaded entry's.

### `PEditorLanguageSend();`

A language chosen by hand belongs in the held draft like anything else typed.
It is chosen whole, so its request goes at once rather than after a pause.
It is not counted as unsaved work, because the tongue on offer is the panel's state.
The engine drops every recording on this request, since each is audio in the old language.

### `PMarkerLoad();`

The presets belong to the language, so the dropdown now offers the new one's.
What is already typed in the field is left alone.
It is what the user wrote, and a language change is not a reason to throw it away.

### `private async void PSpeakerFlagUpdate()`

Shows the selected language's flag beside its name.
A pack declares an ISO country code.
The engine downloads and caches the matching flag-icons SVG.
So the first call may await that fetch, and later ones read what is kept.
When the pack declares no flag or the download fails, a neutral globe stands in.
A pack that is gone from disk fails inside `PEnsign` rather than here.

### `if (chosen != _pSpeakerChoice)`

The choice may have changed while the flags loaded.
Only paint the still-current one.

### `PHeadwordFontApply(_pSpeakerChoice);`

The headword is drawn in the typography of the language it is written in.
A language chosen by hand changes that typography at once.
