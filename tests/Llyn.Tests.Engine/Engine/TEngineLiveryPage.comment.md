# TEngineLiveryPage.cs
Hash: `8fea4145e9530e58`

## `public sealed class TEngineLiveryPage`

Calls `LEngineLiveryRead` through the `TLiveryRead` relay, one fact per part group.
The folded-language format fact lives here too, since it shares the folding pack.

## `private const string TEngineLiveryReflex`

The pack the two folded-reflex facts load, with Jin folded and Wu open.

## `private const string TEngineLiveryFanqie`

The pack `LiveryRead_StoredFanqie_KeepsGroups` loads, naming one fanqie book.

## `private const string TEngineLiveryScript`

The pack `LiveryRead_StoredScript_KeepsStyleGroups` loads, naming one script style.

## `private static readonly TimeSpan TEngineLiveryPatience`

How long `LiveryRead_StoredScript_KeepsStyleGroups` waits for the script fetch it starts.

## `private static readonly Dictionary<string, string> TEngineLiveryPages`

The pages the fake client serves to that script fetch.

## `public void LiveryRead_StoredEntry_KeepsHeaderAndStamps()`

Saves an entry, marks it favorite and rates it through the engine relays.
Reads the page and checks the draft, favorite, grasp and both stamps.

## `public void LiveryRead_MissingEntry_ReturnsNull()`

Reads an id with no stored entry and finds null.

## `public void LiveryRead_FoldedReflex_KeepsFoldedLanguage()`

Saves an entry with two reflexes under a pack that folds one.
Reads the page and checks the folded list, the reflex order and the guise folded flags.

## `public void LiveryRead_StoredFanqie_KeepsGroups()`

Saves one fanqie row through `TFanqieSave`, then reads the page.
Checks the one group `LEngineFanqieDivide` answers.

## `public async Task LiveryRead_StoredScript_KeepsStyleGroups()`

Reads the page before any fetch and finds no script.
Starts the script fetch through `TEngineScriptStart`, waits on `TEngineScriptCheck`, and reads again.
Checks the one style group `LEngineScriptDivide` answers.

## `public void LiveryRead_CardWithStoredTarget_KeepsTarget()`

Saves an entry whose meaning links a stored entry, then reads the page.
Checks the target `LEngineTranslationRead` answers under the card id.

## `public void LiveryRead_LinkedEntry_KeepsIncomingUsage()`

Saves an entry whose meaning links a stored entry, then reads the linked entry's page.
Checks the usage `LEngineIncomingRead` answers.

## `public void LiveryFormat_FoldedLanguage_PutsRowsInsideDetails()`

Saves an entry with a folded and a shown reflex language, then writes its body through `TLiveryFormat`.
Checks the folded reading sits inside `<details>` and the shown one before it.
Checks the summary carries the `Reflex.More` key.
