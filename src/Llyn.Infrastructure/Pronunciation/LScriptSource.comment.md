# LScriptSource.cs

## `public static class LScriptSource`

Asks one web database for the glyph pictures of one character in one style.
The database is reached by posting its own search form, since the site offers no query API.
The answer is read by the pack's pattern, and every picture it names is fetched as bytes.
Nothing here knows the site, only the shape an [LScriptStyle](../../Llyn.Core/Pronunciation/LScriptStyle.comment.md) declares.

## `private const string LScriptSourceToken`

The placeholder in a form value that the character replaces.

## `private static readonly Regex LScriptSourceTag`

Any tag, dropped from a caption so only its words remain.

## `private static readonly Regex LScriptSourceBreak`

A line break tag, turned into a space so a two-line caption reads as one line.

## `private static readonly TimeSpan LScriptSourcePatience`

How long one pack pattern may run over one answer before the answer counts as empty.

## `public static async Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LScriptSourceFind(HttpClient client, LScriptStyle style, string character, CancellationToken cancellation)`

Posts the form, reads the pictures and captions the pattern names, and fetches each picture in turn.
A picture that cannot be fetched is skipped, and the positions count only those kept.
The gloss read from the page is repeated on every picture of the style.
The second value says whether the database answered at all.
An answer naming no picture is a miss, while a database that never answered is not.

## `private static async Task<string?> LScriptBodyRead(HttpClient client, LScriptStyle style, string character, CancellationToken cancellation)`

Posts the pack's fields as a URL-encoded form, which sends the character as UTF-8 percent escapes.
The site reads a raw multibyte body as another encoding and finds nothing, so the encoding matters.
A failed request or an error status reads as null.

## `private static IReadOnlyList<(string LScriptAddress, string LScriptCaption)> LScriptHitScan(LScriptStyle style, string body)`

Every match of the pattern, as a resolved picture address and a cleaned caption.
A match whose image group is empty is dropped.

## `private static string LScriptGlossRead(LScriptStyle style, string body)`

The first group of the gloss pattern, cleaned, or empty when the style declares none or nothing matched.

## `private static string LScriptGroupRead(Match match, int group)`

The named group's text, or empty when the group number is zero or beyond the match.

## `private static string LScriptAddressResolve(LScriptStyle style, string address)`

Decodes entities, runs the rewrite rules, and prepends the prefix to a relative address.

## `private static string LScriptTextNormalize(string raw)`

Turns line breaks into spaces, drops every other tag, decodes entities and folds the whitespace.

## `private static async Task<byte[]?> LScriptDataRead(HttpClient client, string address, CancellationToken cancellation)`

Fetches one picture's bytes, or null when the request fails, errs or returns nothing.
