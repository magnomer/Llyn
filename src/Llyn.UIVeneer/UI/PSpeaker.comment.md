# PSpeaker.cs

## `public partial class PEditor`

The language an entry is written in, as the editor chooses it.
That is the dropdown of installed packs and the pill that shows the chosen one.
The flag beside it belongs here too.
The choice is what lookup, audio download, and the saved entry are all carried out in.
The choice itself lives in the held draft, and every reader here asks the tenure for it.

## Inline notes

### `private string PSpeakerLanguageRead()`

The held draft's language, or empty when no tenure is held.
The editor keeps no copy, so nothing here can drift from what the engine holds.
A fresh draft already carries the first listed pack, which the engine chose when it started.

### `internal async void PSpeakerLoad()`

Builds the language menu and shows the draft's language in the pill.
Each row reads its flag from `PEnsign`, which every other tab reads too.
The Translation chips are redrawn afterwards, because they were built before any flag existed.
The flags may await a first-time fetch, hence async.

### `await PEnsign.PEnsignLoad(_lEngine);`

The menu waits for the shared flags rather than fetching its own.
This tab once resolved every SVG again on its own, so a flag was parsed twice.
The rows are added afterwards, in the order the packs were read.

### `internal void PSpeakerHandle(object sender, RoutedEventArgs e)`

A pack picked from the menu is written to the pill and sent to the draft, then painted.
No comparison with the old choice is made, since the engine answers a repeat with the same draft.

### `PEditorLanguageSend();`

A language chosen by hand belongs in the held draft like anything else typed.
It is chosen whole, so its request goes at once rather than after a pause.
It is not counted as unsaved work, because the tongue on offer is the panel's state.
The engine drops every recording on this request, since each is audio in the old language.

### `PCategoryLoad();`

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

### `if (chosen != PSpeakerLanguageRead())`

The draft's language may have changed while the flags loaded.
Only paint the still-current one.

### `PHeadwordFontApply(language);`

The headword is drawn in the typography of the language it is written in.
A language chosen by hand changes that typography at once.
