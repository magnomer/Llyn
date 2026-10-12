# CPortrait.cs
Hash: `37a9199673348538`

## `internal static class CPortrait`

What every panel's export and print gate shares: the formats offered, the words, and the failure policy.
Each map copies or maps by name, and the engine builds every record it takes.
Only the offered formats carry a rule of their own.
An enum member a map does not know throws, so a reordered enum cannot shift a meaning.
Only the panel gates and `CSPress` call it, so every member is an `L` helper.

## `private static readonly string[] LPortraitLabelKeys`

The wording keys of an entry page, in the order of the engine label's words.
The collocation keys are `Display.CollocationSingle`, `Display.CollocationOther`, `Display.CollocationStub` and `Display.CollocationHeading`.
The three unit keys and the two freedom keys close the list, in the order the label maps them.

## `internal static IReadOnlyList<CPortraitChoice> LPortraitChoiceRead()`

The export formats the file question offers, read from the engine's static format list.
It hands that list to the overload, which holds every rule.

## `internal static IReadOnlyList<CPortraitChoice> LPortraitChoiceRead(IReadOnlyList<(LPortraitMedium, string, bool)> rows)`

The export formats of `rows`, in their order, each with its wording key.
It takes the rows as a parameter, so a test can feed rows the static source never gives.
The engine names the formats, their suffixes and the default one.
Conduct strips every `|` from a suffix, since a dialog filter splits on it.
Exactly one format comes back chosen, the engine's first chosen one or else the first.
So the dialog always opens on a real format.
An unknown engine format is a programming mismatch, so the read throws rather than offer it.
An empty engine list still offers one format, so the reader can always export.
That one is the program's own markup, under no suffix, so its filter shows every file.

## `internal static CPortraitChoice LPortraitChoiceCreate(LPortraitMedium medium, string suffix, bool chosen)`

One engine format as a ready row, mapped by name, with the key its name is worded by.
`CSPress` builds its PDF-only save choice through it, so each format key has one owner.

## `internal static async Task LPortraitFileExport(CEnvoy envoy, LSettingsPort settings, Func<string> fileRead, Func<string, LPortraitMedium, Task> export)`

Reads the offered name through `fileRead`, then asks the reader through `envoy` for a path and a format.
The name is read here, so a failed read shows `Export.NameFailed` and asks nothing.
Its own key tells a failed name read apart from a failed export.
Then it runs the panel's export.
The question needs the gate's offered name and formats, so the envoy asks it rather than the driver beforehand.
No path means the reader cancelled, which exports nothing and is no failure.
A failed export shows `Export.Failed` through the panel's envoy, with the notice `settings` reads.
The format read and the question run inside that same try.
So an engine throw or an unknown format is shown, and the export stops cleanly.
The task never faults, since its drivers await it from event handlers.

## `internal static async Task LPortraitPrint(CEnvoy envoy, LSettingsPort settings, Func<Task<LGalleyRelay?>> open)`

Opens a print session, puts the print window up over its gate, and disposes the session on close.
The window's galley is disposed first, so a pending wait is cancelled before the session goes.
A null relay means nothing is printable, so no window opens and no failure shows.
A failed open shows `Print.Failed` through the panel's envoy, with the notice `settings` reads.
Failures inside the window answer wording keys to the window instead, since it stays open.
The window gets `settings` too, so a printer listing failure shows through the ledger.

## `internal static LPortraitMedium LPortraitMediumRead(CPortraitMedium medium)`

The engine format a driver's export choice stands for, mapped member for member by name.

## `internal static LPortraitLabel LPortraitLabelRead(LSettingsPort settings)`

The engine label an entry page is worded with.
Conduct chooses every key, and the engine words each in the workspace's language.
A printed or exported page is not the screen, so the driver's lookup takes no part.

## `internal static LPortraitLegend LPortraitLegendRead(LSettingsPort settings, string realm)`

The engine legend a catalog page of `realm` is worded with: Example, Source or Situation.
The realm picks the untitled fallback and the usage tally wording, which differ by realm.
Each Source kind is worded by its stored word, which is also its key.
The engine names the kinds, so Conduct names no kind of its own.
