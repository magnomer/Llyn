# PLangcodeMenu.cs

## `public partial class PEditor`

The language an entry is written in, as the editor chooses it: the dropdown of installed packs, the pill that shows the chosen one, and the flag beside it. The choice is what lookup, audio download, and the saved entry are all carried out in.

## Inline notes

### `private const string PWindowLanguage = "English";`

Fallback language used until a pack is chosen, and when no pack folder is present on disk.

### `private bool _pLangcodeEntry;`

Whether the current language came from a loaded entry. A pack that is no longer on disk is still the language the entry was written in, so the start-up fallback to the first pack leaves it alone; only a form standing on no entry may be moved off its language.

### `internal async void PLangcodeLoad()`

Builds the language menu. Each row carries its own flag, resolved once here so the dropdown paints ready. The flag download may await a first-time fetch, hence async.

### `string?[] paths = await Task.WhenAll(`

Every flag is asked for at once rather than one after the next: each is a first-time download with its own ten-second timeout, and awaiting them in turn made the menu wait for the sum of them. The rows are added afterwards, in the order the packs were read.

### `if (!_pLangcodeEntry && languages.Count > 0 && !languages.Contains(_pLangcodeChoice))`

A form standing on an entry keeps that entry's language even when no pack answers to it; the fallback is for a form that stands on nothing and would otherwise name a pack that is not installed.

### `PSpeechLoad();`

The parts of speech on offer are the chosen language's, so a fallback that moved the choice moves them too.

### `_pLangcodeEntry = false;`

Chosen by hand, so the language is no longer the loaded entry's.

### `PSpeechLoad();`

The presets belong to the language, so the dropdown now offers the new one's. What is already typed in the field is left alone: it is what the user wrote, and a language change is not a reason to throw it away.

### `private async void PLangcodeFlagUpdate()`

Shows the selected language's flag beside its name. A pack declares an ISO country code; the engine downloads and caches the matching flag-icons SVG, so this may await a first-time fetch. When the pack declares no flag or the download fails, a neutral globe stands in.

### `if (chosen != _pLangcodeChoice)`

The choice may have changed while the flag downloaded; only paint the still-current one.
