# CLedger.cs
Hash: `e18ee13b15938b3d`

## `public sealed class CLedger`

The settings ledger holds the workspace settings, their saves and the pages a settings view lists.
A view subscribes once and is handed one ready state on every open and after every settings or workspace change.
It stands on the atelier's settings port, which it reads from `CAtelier` on every call rather than keeping a copy.

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

## `internal CLedgerNoticed LLedgerRepaint { get; } = new();`

The atelier's repaint memory, built once with the ledger, so it lives and ends with the atelier.
The ledger owns it, since it is the failure policy every gate funnels through and it hears the user's acts.
The editors, the sides and the source editor take it at build, so every repaint read shares it.

## `internal void LLedgerAttach()`

Attaches the ledger to the settings and workspace bulletins.
The same bulletins and the vista bulletin first clear the repaint memory, since each follows a user act.
A vista bulletin comes only when the user selects, searches, orders, filters or edits, never on a redraw.
The workspace calls it on its first open only.

## `internal void LLedgerRaise()`

Reads the whole settings state and raises `CLedgerChanged` with it.

## `public void CLedgerLocalizationSave(string language, CEnvoy envoy)`

Stores the interface language.
A failed save is shown through `envoy` as `Settings.SaveFailed`.
The ledger is then raised again, so the driver repaints from the saved settings.

## `public void CLedgerEpithetSave(bool epithet, CEnvoy envoy)`

Stores whether lists show epithets.
A failed save is shown through `envoy` as `Settings.SaveFailed`.
The ledger is then raised again, so the driver repaints from the saved settings.

## `public void CLedgerFrequencySave(bool frequency, CEnvoy envoy)`

Stores whether the frequency lookup is on.
A failed save is shown through `envoy` as `Settings.SaveFailed`.
The ledger is then raised again, so the driver repaints from the saved settings.

## `public void CLedgerMorphologySave(bool morphology, CEnvoy envoy)`

Stores whether the morphology lookup is on.
A failed save is shown through `envoy` as `Settings.SaveFailed`.
The ledger is then raised again, so the driver repaints from the saved settings.

## `public void CLedgerRespellingSave(bool respelled, CEnvoy envoy)`

Stores whether transcriptions show respelled.
A failed save is shown through `envoy` as `Settings.SaveFailed`.
The ledger is then raised again, so the driver repaints from the saved settings.

## `public void CLedgerFailureShow(CEnvoy envoy, string key, Exception exception)`

Shows a failure a driver's own medium raised, under the headline `key` names.
A player's refusal or a picture that will not decode has no gate, so the driver hands it here.
It reads the notice through the atelier's settings port, as every gate's failure policy does.

## `internal static CLedgerNotice LLedgerNoticeRead(LSettingsPort settings, Exception exception)`

The one owner of the notice read, which every gate's failure policy shares.
A refusal shows its own reason, since it is a position the user can act on.
A fault shows `Notice.Unexpected`, and names its audit file under `Notice.Recorded` when one was written.
The engine answers in one call with the two keys this read hands down, and the map holds no rule.

## `internal static void LLedgerFailureShow(CEnvoy envoy, LSettingsPort settings, string key, Exception exception)`

A gate's failure policy.
The gate chose `key`, and the envoy gets the ready notice to show with it.
Every call shows, so a user act that fails again on a retry shows its failure again.
A read that runs on every repaint goes through `LLedgerRepaint` first, which merges its repeats.
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
A shipped code the culture table does not know also answers the code itself.
So a bad code never throws through a settings repaint.
