# TAuditComment.cs

## `public sealed class TAuditComment`

Keeps prose in the comment files and out of the sources.
Every threshold comes from `TAuditCommentSetting`, which `auditcomments.ps1` regenerates from `auditcomments.json`.

## `public void AuditComment_CommentLines_KeepLineRules()`

Reads every comment file under the configured roots.
A blank line or a heading is skipped.
Every other line must be one sentence, hold at most the configured words, and carry none of the forbidden characters.
Any hit names the file, line, and the rule it breaks.

## `public void AuditComment_Sources_CarryNoRemark()`

Scans every audited source for the comment markers its extension declares.
String and character literals are blanked first, so a `//` inside a URL does not count.
Files named in the exempt list are skipped, since a generated sidecar carries a header.

## `private static string TAuditLineCheck(string line)`

Returns the rule problems one line breaks, joined by a comma, or an empty string.
A leading list marker is dropped before the words are counted.
Code spans are dropped before the forbidden characters and sentence marks are looked for.

## `private static IEnumerable<string> TAuditCommentRead(string repoRoot)`

Yields every comment file under the configured roots, skipping the excluded directory names.
