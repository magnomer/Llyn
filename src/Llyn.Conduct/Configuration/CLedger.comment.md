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

Only the atelier builds it, over its own settings port.

## `public Action CLedgerAttach(Action<CLedgerState> show)`

Hands `show` the whole settings state at once, and again after every settings or workspace bulletin.
So a view makes one call to open and never asks the engine piece by piece.
It answers the detach, which stops both subscriptions.

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

## `public string? CLedgerNoticeRead(Exception exception)`

The reason key of a refusal standing anywhere inside the failure, or nothing for a fault.

## `public string? CLedgerAuditRecord(Exception exception)`

Writes one unexpected fault into the workspace's audit log, and answers the file it went to or nothing.

## `public IReadOnlyList<string> CLedgerFind(string text)`

The pages whose title, helper or own keys read `text`, trimmed and ignoring case.
A blank text answers every page.
Labels are read through the engine's texts, so a console searches the same words.

## `public string CLedgerMetaRead(string child, bool linked)`

The one-line summary one page shows under its title.
The Layout page's summary follows `linked`, which the driver hands in since the linked switch is GUI-only state.
Every other page reads the stored settings.

## `private CLedgerState CLedgerRead()`

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
