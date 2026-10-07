# CPortrait.cs
Hash: `4a198a9919f2bbd8`

## `internal static class CPortrait`

What every panel's export and print gate shares: the formats offered, the words, and the failure policy.
Each map copies or maps by name, and the engine builds every record it takes.
Only the offered formats carry a rule of their own.
An enum member a map does not know throws, so a reordered enum cannot shift a meaning.
Only the panel gates call it, so every member is an `L` helper.

## `private static readonly string[] LPortraitLabelKeys`

The wording keys of an entry page, in the order of the engine label's words.
The four unit keys close the list, in the order the label maps them.

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

## `private static CPortraitChoice LPortraitChoiceCreate(LPortraitMedium medium, string suffix, bool chosen)`

One engine format as a ready row, mapped by name, with the key its name is worded by.

## `internal static async Task LPortraitFileExport(CEnvoy envoy, LSettingsPort settings, Func<string> fileRead, Func<string, LPortraitMedium, Task> export)`

Reads the offered name through `fileRead`, then asks the reader through `envoy` for a path and a format.
The name is read here, so a failed read shows `Export.NameFailed` and asks nothing.
Its own key tells a failed name read apart from a failed export.
Then it runs the panel's export.
The question needs the gate's offered name and formats, so the envoy asks it rather than the driver beforehand.
No path means the reader cancelled, which exports nothing and is no failure.
A failed export shows `Export.Failed` through the panel's envoy, with the notice `settings` reads.
The format read and the question run inside that same try, as the print path does.
So an engine throw or an unknown format is shown, and the export stops cleanly.
The task never faults, since its drivers await it from event handlers.

## `internal static async Task LPortraitTicketPrint(CEnvoy envoy, LSettingsPort settings, Func<LPressTicket, Task> print)`

Asks the reader through `envoy` for a printer, then runs the panel's print with the engine's ticket.
No ticket means the reader cancelled, which prints nothing and is no failure.
The question stands inside the guard, because a print dialog raises when the spooler is down.
A failed dialog or print shows `Print.Failed` through the panel's envoy, with the notice `settings` reads.

## `internal static LPortraitMedium LPortraitMediumRead(CPortraitMedium medium)`

The engine format a driver's export choice stands for, mapped member for member by name.

## `internal static LPressTicket LPortraitTicketRead(CPressTicket ticket)`

The engine ticket a driver's print dialog answer stands for.
Side and ink map member for member by name, and the engine picks the sheet when the dialog named none.

## `private static LPressSide LPortraitSideRead(CPressSide side)`

The engine print side a dialog's side names, member by member by name.

## `private static LPressInk LPortraitInkRead(CPressInk ink)`

The engine ink a dialog's ink names, member by member by name.

## `internal static LPortraitLabel LPortraitLabelRead(LSettingsPort settings)`

The engine label an entry page is worded with.
Conduct chooses every key, and the engine words each in the workspace's language.
A printed or exported page is not the screen, so the driver's lookup takes no part.

## `internal static LPortraitLegend LPortraitLegendRead(LSettingsPort settings, string realm)`

The engine legend a catalog page of `realm` is worded with: Example, Source or Situation.
The realm picks the untitled fallback and the usage tally wording, which differ by realm.
Each Source kind is worded by its stored word, which is also its key.
The engine names the kinds, so Conduct names no kind of its own.
