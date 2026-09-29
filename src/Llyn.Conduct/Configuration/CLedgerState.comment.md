# CLedgerState.cs

## `public sealed record CLedgerState(`

Everything a settings view shows, read in one call.

**Parameters**

- `CLedgerStateSettings`: the stored settings.
- `CLedgerStatePath`: the workspace folder in use.
- `CLedgerStateLocalization`: the interface language, normalised by the engine.
- `CLedgerStateTexts`: the interface texts of that language, keyed by localization key.
- `CLedgerStateLanguages`: each shipped interface language's code with its native name, in scan order.
- `CLedgerStatePages`: the settings pages with their titles and summaries.
- `CLedgerStateShown`: the pages the kept search shows, and whether none does.
