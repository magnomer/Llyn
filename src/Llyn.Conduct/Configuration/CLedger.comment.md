# CLedger.cs

## `public sealed class CLedger`

The settings ledger: the workspace settings, their saves, and the pages a settings view lists.
A view attaches once and is handed one ready state on attach and after every settings or workspace change.
It stands on the atelier's settings port, and `CAtelier` hands one out on each read.

## `private const int CLedgerWeb = 2;`

How many online lookups the Web page counts against, the frequency and the morphology lookup.

## `private static readonly (string, string[])[] CLedgerPages =`

The settings pages in the order a view lists them, each with the keys its search also reads.

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

## `public IReadOnlyList<string> CLedgerFind(string text)`

The pages whose title, helper or own keys read `text`, trimmed and ignoring case.
A blank text answers every page.
Labels are read through the engine's texts, so a console searches the same words.

## `public string CLedgerMetaRead(string child, bool linked)`

The one-line summary one page shows under its title.
The Layout page's summary follows `linked`, which the driver hands in since the linked switch is GUI-only state.
Every other page reads the stored settings.

## `private CLedgerState LLedgerRead()`

Builds the state a view shows: the settings, folder, language, texts, language list and pages.
The texts are loaded first, so every title and summary reads in the stored language.
The Layout page's summary is left empty, since only the driver knows the linked switch.

## `private CSettings CLedgerSettingsRead()`

Copies the engine's settings, with its count of online lookups, into the Conduct shape.

## `private string CLedgerMetaRead(string child, CSettings settings)`

The summary of one settings-backed page, from `settings` already read.
An unknown page reads empty.

## `private string CLedgerLanguageRead(string localization)`

The native name of an interface language the program ships, or the code itself for one it does not.
