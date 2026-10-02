# CLedger.cs
Hash: `6d68fc5fdb3d40e7`

## `public sealed class CLedger`

The settings ledger holds the workspace settings, their saves and the pages a settings view lists.
A view attaches once and is handed one ready state on attach and after every settings or workspace change.
It stands on the atelier's settings port, and `CAtelier` hands one out on each read.

## `private const int CLedgerWeb = 2;`

How many online lookups the Web page counts against, the frequency and the morphology lookup.

## `private static readonly (string, string[])[] CLedgerPages =`

The settings pages in the order a view lists them, each with the keys its search also reads.

## `private string? _cLedgerWanted;`

The raw text the settings search last handed in, kept so every later state shows the same narrowing.

## `internal CLedger(CAtelier atelier)`

Only the atelier builds it, once, over its own settings port.

## `public event Action<CLedgerState>? CLedgerChanged;`

The whole settings state, raised on every open and after every settings or workspace bulletin.
A settings view subscribes once and never asks the engine piece by piece.

## `internal void LLedgerAttach()`

Attaches the ledger to the settings and workspace bulletins.
The workspace calls it on its first open only.

## `internal void LLedgerRaise()`

Reads the whole settings state and raises `CLedgerChanged` with it.

## `public void CLedgerLocalizationSave(string language)`

Stores the interface language.

## `public void CLedgerEpithetSave(bool epithet)`

Stores whether lists show epithets.

## `public void CLedgerFrequencySave(bool frequency)`

Stores whether the frequency lookup is on.

## `public void CLedgerMorphologySave(bool morphology)`

Stores whether the morphology lookup is on.

## `public void CLedgerRespellingSave(bool respelled)`

Stores whether transcriptions show respelled.

## `public CLedgerNotice CLedgerNoticeRead(Exception exception)`

The notice a failure shows, ready for a driver to word.
A refusal shows its own reason, since it is a position the user can act on.
A fault shows `Notice.Unexpected`, and names its audit file under `Notice.Recorded` when one was written.
The engine answers in one call with the two keys this read hands down, and the map holds no rule.
A driver catch site that still owns its failure asks it, until its own gate reports through `CEnvoy`.

## `internal static CLedgerNotice LLedgerNoticeRead(LSettingsPort settings, Exception exception)`

The one owner of the notice read, which `CLedgerNoticeRead` and every gate's failure policy share.

## `internal static void LLedgerFailureShow(CEnvoy envoy, LSettingsPort settings, string key, Exception exception)`

A gate's failure policy.
The gate chose `key`, and the envoy gets the ready notice to show with it.
The driver's consult never reads Conduct again, so it only shows what it was handed.

## `public void CLedgerFolderOpen(CEnvoy envoy)`

Opens the workspace folder in use in the shell's file browser.
The engine reads the folder itself, so the driver hands nothing it gathered.
A folder the shell cannot open is reported under `Settings.FolderFailed`, since it may have moved away meanwhile.

## `public CLedgerShown CLedgerFind(string? text)`

Keeps the raw search text and answers the pages it shows.
The engine matches each page's title, helper and own keys by the shared catalog rule.
A null or blank text shows every page.

## `public CLedgerPage CLedgerMetaRead(bool linked)`

The Layout page, its summary following `linked`.
The driver hands `linked` in raw, since the linked switch is GUI-only state kept by its Capsule.

## `private CLedgerShown LLedgerShownRead()`

The pages the kept search text shows, and whether none does, so the empty text shows.
The keys are Conduct's choice, and the match is the engine's in one call.

## `private CLedgerState LLedgerRead()`

Builds the state a view shows.
It holds the settings, folder, language, texts, language list and pages.
The texts are loaded first, so every title and summary reads in the stored language.
The Layout page's summary is left empty, since only the driver knows the linked switch.
The pages the kept search shows ride along, so a settings change keeps the catalog narrowed.

## `private CSettings CLedgerSettingsRead()`

Copies the engine's settings, with its count of online lookups, into the Conduct shape.

## `private string CLedgerMetaRead(string child, CSettings settings)`

The summary of one settings-backed page, from `settings` already read.
An unknown page reads empty.

## `private string CLedgerLanguageRead(string localization)`

The native name of an interface language the program ships, or the code itself for one it does not.
