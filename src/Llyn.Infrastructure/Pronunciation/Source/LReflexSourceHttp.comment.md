# LReflexSourceHttp.cs

## `public sealed class LReflexSourceHttp : LReflexSource`

The adapter behind the `LReflexSource` port, bound to the shared `HttpClient` the rig builds it over.
Asks one rule's site for one character and reads the readings it answers with.
The page is matched by the rule's pattern, one reading per match, and each match becomes one draft row.
Nothing here knows the site: the address, the form, the pattern and the format come from the pack.

## `public LReflexSourceHttp(HttpClient client)`

Binds the adapter to the `client` every fetch goes through.

## `private const string LReflexSourceToken = "{word}";`

The placeholder in an address, a form value or a pattern that the character replaces.

## `private const RegexOptions LReflexSourceLoose =`

The options the rule pattern runs under: case-blind tags and dots that cross lines.

## `private static readonly Regex LReflexSourceTag = new("<[^>]+>", RegexOptions.CultureInvariant);`

Any tag inside a captured group, dropped so the stored text is plain.

## `private static readonly Regex LReflexSourceSlot =`

One `{name}` slot of the format, filled from the named group of the same name.

## `private static readonly Regex LReflexSourceOption =`

One `[[...]]` segment of the format, kept only when every slot inside it was captured.
A single bracket is literal, so `[{text}]` prints the reading in brackets.

## `private static readonly TimeSpan LReflexSourcePatience = TimeSpan.FromSeconds(2);`

How long one pack regex may run over one page before the page counts as unreadable.

## `public async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexSourceFind(`

Fetches the page for `character` and reads its rows.
The second value says whether the site was reached and read.
A page never fetched, a busy answer and an unreadable page are not reached.
So the character is asked again later.
A page reached but matching nothing is reached, and the miss is remembered for the session.

## `public static IReadOnlyList<LReflexDraft> LReflexSourceScan(LReflexRule rule, string character, string body)`

Reads the rows out of one fetched page.
Each match yields a row whose text is the format filled from the match, with every slash and bracket dropped.
Its kind, note and main mark come from their named groups, and its region is the rule's.
With `split` set the text and the note are cut into pieces.
Each reading is paired with the note at its position.
The remark of each reading is read through `LReflexRemarkRead`.
A row repeating the kind, text and note of an earlier row is dropped.
A page lists a reading once per section, and one is enough.
A match whose text comes out empty is skipped.
With `every` unset only the first match is kept.
Rows that all carry one text are all marked in common use, since the character is said one way only.
Otherwise, with `first` set and no match capturing `main`, the first row is marked as the one in common use.

## `private static bool LReflexRowMatch(LReflexDraft one, LReflexDraft other)`

True when two rows carry one kind, one text and one note, so the second adds nothing.

## `private static string LReflexDelimiterRemove(string text)`

Drops every slash and bracket, so a stored reading is bare.
A later job can then read its onset, nucleus and coda.

## `private static IReadOnlyList<string> LReflexPieceScan(LReflexRule rule, string text)`

Cuts one text on the rule's split pattern into its trimmed non-empty pieces.
Without a pattern the text is handed back whole.

## `private static string LReflexRemarkRead(LReflexRule rule, string body, Match match, string note)`

A match capturing a filled `remark` group hands that text as the remark.
Otherwise the remark is looked up after the match through `LReflexRemarkFind`.

## `private static string LReflexRemarkFind(LReflexRule rule, string body, int start, string note)`

What the page says of one reading, read from the stretch after the match up to the rule's `until` pattern.
The rule's remark pattern is filled with the escaped note and its `remark` group is the answer.
Empty without a remark pattern, without a note, or when the stretch holds no such line.

## `private static string LReflexTextResolve(LReflexRule rule, string text)`

Runs the rule's rewrites over the formatted text in pack order and trims it.
A rule with no rewrite hands the text back as it came.

## `private static string LReflexNoteFormat(LReflexRule rule, string note)`

Runs the rule's recasts over one note piece in pack order and trims it.
With `superscript` set every ASCII digit left is then raised, so `oq7` is stored as `oq⁷`.
The remark is looked for under the piece as it came, since the page never carries the recast form.

## `private static string LReflexTextFormat(LReflexRule rule, Regex line, Match match)`

Fills the rule's format from the match, a slot naming no group of the pattern left as written.
An optional segment is dropped whole when a slot inside it captured nothing, so `롱[[({initial})]]` prints `롱` alone.
Runs of whitespace collapse to one space, so a multi-line capture prints on one line.

## `private static string LReflexSlotFormat(string template, Regex line, Match match, ref bool missing)`

Fills every slot of one template piece, and raises `missing` when a named slot captured nothing.

## `private static string LReflexGroupRead(Match match, string name)`

The plain text of one named group, or empty when the group did not take part.

## `private static string LReflexTextNormalize(string raw)`

Strips tags, decodes entities, collapses whitespace and composes the text, so a stored reading compares by value.

## `private static async Task<string?> LReflexBodyRead(`

Fetches one page with a GET, or a POST when the rule lists form fields, under the rule's headers.
Not found answers an empty page, since the site has no entry and the miss is final.
Any other failure answers `null`, since the site was not reached.
