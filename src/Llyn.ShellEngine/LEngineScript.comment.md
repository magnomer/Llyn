# LEngineScript.cs

## `public sealed partial class LEngine`

The script side of the engine: the glyph pictures of the characters an entry is written with.
A character earns its pictures once, fetched from the databases its language pack lists.
They are kept in the workspace database.
The fetch starts on first display and runs in the background, announcing itself when the pictures are stored.
The pictures belong to the character, so a second entry sharing it fetches nothing.

## `private readonly SemaphoreSlim _lEngineScriptGate`

At most two characters are fetched at once, since each character posts one form per style and fetches every picture.

## `private readonly Dictionary<string, CancellationTokenSource> _lEngineScriptPending`

The characters being fetched, keyed by language and character, each with the token that cancels it.

## `private readonly HashSet<string> _lEngineScriptMissed`

The characters every database answered for yet none drew, so they are not asked again this session.

## `public IReadOnlyList<LScriptStyle> LEngineStyleRead(string language)`

The styles the language's pack lists, or none for a blank language or a pack without a `script` list.

## `public IReadOnlyList<LScriptImage> LEngineScriptRead(long entryId)`

The stored pictures of every Han character of the entry's headword, character by character.
A character with nothing stored starts a fetch and contributes nothing yet.
An entry whose language lists no styles reads empty and starts nothing.

## `public bool LEngineScriptCheck(long entryId)`

Whether a fetch is running for any character of the entry's headword.

## `public async Task<IReadOnlyList<LScriptImage>> LEngineScriptFind(string character, string language, CancellationToken cancellation)`

Asks every style of the language for the character and returns what they drew, in pack order.

## `private async Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LEngineScriptScan(string character, string language, CancellationToken cancellation)`

The lookup behind the find, one style after another in pack order, keeping every picture.
The second value says whether any database answered.

## `private static string LScriptKeyFormat(string language, string character)`

The pending and missed key, language and character joined by a newline no name contains.

## `private void LEngineScriptStart(long entryId, string language, string character)`

Starts a fetch for the character unless one runs or the character was missed this session.
The entry id is remembered only for the bulletin the fetch raises.

## `private void LEngineScriptClear()`

Cancels every running fetch and forgets the misses, when the workspace changes or the engine closes.

## `private async Task LEngineScriptRun(long entryId, string language, string character, string key, CancellationTokenSource fetch)`

The fetch itself, admitted through the gate, storing what it found and raising the script bulletin.
A cancelled fetch stores nothing.
A character every database reached yet none drew is marked missed.
The pending entry is removed last, whatever happened, so the display can ask again after a lost fetch.
