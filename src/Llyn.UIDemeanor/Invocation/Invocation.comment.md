# Invocation

Strict mode: one action from the command line, printed, ending with an exit code.
It never draws a screen and never reads keys, only an answer line on a terminal.
The folder holds the parsed command line, its global options and the envoy that answers Conduct's questions.

## `BInvocation`

One parsed command line, its options kept in the order typed.
The bootstrap parses it before the workspace opens and plays it on the loop once the atelier exists.

## `_bInvocationOptions`

Each option's name without its dash, with its value or null when no colon was typed.
The typed order is the order the run plays the options.

## `BInvocationGlobal`

The global options a question or a print reads, held apart from the verb.
It is a property named after its type, as the naming rule asks.

## `BInvocationParse`

Reads the arguments by the grammar of plan `08` section 2, and every failure is a usage error.
An option is a dash, a name and an optional colon and value.
A name is lowercase ASCII, starts with a letter and goes on in letters and digits.
The value is everything after the first colon, so `-workspace:C:\Notes` keeps its drive colon.
A bare word, a double dash, a slash, an equals sign or a capital letter is not an option.
An abbreviation is an unknown option, since no name is matched by its prefix.
The verbs are help, version, workspace, find, new, delete, import and export.
A second verb is a usage error.
The qualifiers headword, language, pronunciation, note, to and as need a value.
An option given twice is a usage error.
An empty value counts as missing, since no option here takes empty text.
`-version`, `-yes`, `-no` and `-quiet` forbid a value.
`-color` and `-format` are checked against their words.
`-format` takes only the output formats, since the export file format belongs to `-as`.
`-as` takes markup, html, markdown, docx or pdf.
`-new` takes only `entry`.
`-delete` and `-export` take an id, a whole number above zero.
`-help` takes no value or one verb's name.
`-yes` with `-no` is a usage error, since no answer could honour both.
A missing verb gives the `Invocation.Usage` line.
`-export` without `-to` is a usage error.
The first failure in typed order wins, then the yes and no clash, the missing verb, the missing `-to`.
The error speaks the default catalog, since the stored language is unknown before the workspace opens.
The caller prints the returned sentence and exits 2, so a typo never opens or migrates a database.

## `BInvocationRun`

Plays the options in typed order, one handler per option that has work of its own.
It is asynchronous, so the import and export verbs are awaited before the next option.
Global options and qualifiers have no handler, since the envoy and the verbs read them where needed.
The presence seam goes to the strict envoy it builds.
It returns 1 when the envoy recorded a refusal, else 0.

## `BInvocationLeased`

True when the command line holds `-new`, `-delete`, `-import` or `-workspace` with a value.
Those options change a workspace or move onto one, so only they build a leased engine.
`-workspace` without a value only reads the path, so it takes no lease.
The bootstrap reads it before the engine exists.

## `BInvocationValueRead`

The value typed for one option, or null when that option is absent.
The envoy reads the `-workspace` value through it, and `-find` reads `-format`, defaulting to text.

## `BInvocationHelpPrint`

Prints the `Invocation.Help` sentence and one line per option on stdout, or one verb's line for `-help:verb`.
Help reads no gate, so the run hands it the option directly, as a look-only event goes to a refine.
Option names are printed literally, since they are a data contract and never translated.
Names are ASCII, so padding by characters aligns the column by cells.

## `BInvocationVersionObserve`

Calls `CAtelierAboutRead`, the gate the GUI's about box calls, and hands its key on.

## `BInvocationVersionPrint`

Prints the product, the about text and the build on one stdout line.
The build is the assembly version to three parts, as the GUI's about box shows it.

## `BInvocationWorkspaceObserve`

Without a value it reads `CAtelierPathRead`, with a value it moves through `CWorkspaceChange`, as the settings page does.
A refused move shows its failure through the envoy and returns the unchanged path.
A folder the engine does not accept returns the unchanged path with no failure, as in the GUI.

## `BInvocationWorkspacePrint`

Prints the workspace path the gate returned on stdout.

## `BInvocationFindObserve`

Starts the Library through `CLibraryCreate` with the console's observer, as the GUI's library panel does.
The panel is never shown, so its shown seam answers false.
The atelier is detached, so the Library vista starts on the headword order and reads no stored layout.
It sets the query through `CApertureQuerySet` and reads the rows on `CApertureRowsChanged`, the GUI's order.
An empty query is a usage error, so the query always changes the vista and the event always fires.
A failed read shows its failure through the envoy and yields no rows, so the run exits 1.

## `BInvocationFindPrint`

Prints one stdout line per row: id, headword, language and reading.
The language is the stored entry language, so no format translates anything.
No rows print nothing in every format, so a script tests for an empty stdout.
`text` pads each column but the last to its widest value in cells, then two spaces.
Cells come from `BCanvasWidthResolve` with ambiguous characters narrow, so a CJK headword keeps the columns straight.
Trailing spaces are trimmed, so an empty reading leaves no padding.
`tsv` joins the four values by tabs, so an empty reading still ends the line with a tab.
`json` prints one array on one line, with the keys `id`, `headword`, `language` and `epithet`.
The id is a number there, and the reading sits under `epithet`, the row's own name for it.
Characters outside ASCII stay as written, since the output is UTF-8 and never HTML.
The id is formatted with the invariant culture in every format.

## `BInvocationNewObserve`

Starts the Library as `-find` does, then calls `CLibraryEntryCreate` with the four qualifiers.
A qualifier not typed is null, so its field stays as a new entry has it.
The entry is saved through the editor's gate, as the GUI saves it.
A false answer with no refusal recorded shows `Invocation.NewEmpty`, so the run exits 1.

## `BInvocationNewAttach`

Hooks the editor desk's finished event, so the saved id reaches the printer.

## `BInvocationNewPrint`

Prints the new entry's id on stdout, formatted with the invariant culture.

## `BInvocationDeleteObserve`

Starts the Library, then calls `CPanelBinDelete` of its panel with the id.
The gate selects the row and asks the envoy's delete question, so `-yes` or `-no` answers it.
A refused or failed delete prints nothing on stdout.
A false answer with no refusal recorded shows `Invocation.EntryMissing`, so a wrong id exits 1.

## `BInvocationImportObserve`

Starts the Library, then awaits `CLibraryMarkupImport`, which reads its file from the envoy.
A count prints on stdout, and no count prints nothing.
No count means the envoy recorded a refusal or a failure, so the run exits 1.

## `BInvocationImportPrint`

Prints the number of entries imported on stdout, formatted with the invariant culture.

## `BInvocationExportObserve`

Starts the Library, then awaits `CLibraryEntryExport` with the id.
A path prints on stdout.
No path with no refusal recorded shows `Invocation.EntryMissing`, so a wrong id exits 1.

## `BInvocationExportPrint`

Prints the path of the exported file on stdout.

## `BEnvoyStrict`

Implements `CEnvoy` for strict mode, answering every question without a screen.
Conduct passes each key, and the envoy only looks up its text, so no question is worded here.
A question with no answering option and no terminal is answered no, so a script never hangs.
It takes the presence seam, so a test can say which files exist.

## `BEnvoyStrictRefusal`

Set when a failure was shown or a question was refused, so the run exits 1.
A question that strict mode cannot answer yet also refuses, so a skipped action never reports success.

## `CEnvoyConfirm`

A yes or no question, answered by `BEnvoyStrictConsult`.
The tally form joins the question, the tally text and the number on one line.

## `CEnvoyUnionConfirm`

The merge question, with the dropped and the kept name joined by an arrow, as the GUI shows them.

## `CEnvoyFailureShow`

Prints the sentence after `llyn: ` on stderr and records the refusal.
The notice form adds the notice sentence, and the label and path when both exist.
Failures print under `-quiet` too, since quiet keeps stderr to failures.

## `CEnvoyLeaveConfirm`

`-yes` saves and `-no` discards the unsaved work.
With neither, the workspace stays, which refuses.

## `CEnvoyOmissionShow`

One stderr line per omission, line number then text, unless `-quiet` is given.

## `CEnvoyWorkspaceRead`

The `-workspace` value, or a refusal when none was typed.

## `CEnvoyMarkupRead`

The `-import` value, or a refusal when none was typed.

## `CEnvoyCustomsRead`

Lists each entry to import with its name, language, mode and target on stderr.
Then it asks `Invocation.CustomsConfirm`.
The list is skipped only under `-quiet` with `-yes` or `-no`, since a bare question needs its list.
A declined question refuses, and `-no` declines it.

## `CEnvoyFileRead`

The `-to` file and the `-as` medium of an export.
Without `-as` the medium is the one the portrait chose by default.
A file the presence seam finds asks `Invocation.ReplaceConfirm`, and a refusal there cancels the export.
Without `-to` it refuses.
The parser already requires `-to` with `-export`, so that path is a guard.

## `CEnvoyPressRead`

Refuses, since no option answers it.

## `CEnvoyFolderRead`

Refuses, since no option answers it.
The same holds for `CEnvoyCoinageRead` and `CEnvoyMediaRead`.

## `BEnvoyStrictConsult`

`-yes` or `-no` answers first.
Otherwise a terminal gets the question and `[y/N]` on stderr, and one line read from stdin.
Only `y` or `yes`, in any case and trimmed, answers yes.
Without a terminal the answer is no, unasked.
The `[y/N]` mark is a fixed affordance like `llyn: `, so no catalog carries it.

## Stamp

Commit: `52c21d68b4c4`
- `BEnvoyStrict.cs` `96e35e37f7f80fbe`
- `BInvocation.cs` `90acd4da8f2cdaac`
- `BInvocationGlobal.cs` `af318fc2fd36320e`
