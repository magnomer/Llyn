# TAuditComment.cs
Hash: `3a54468c02a313ea`

## `public sealed class TAuditComment`

Keeps prose in the comment files and out of the sources.
Every rule value comes from `TAuditCommentSetting`, which mirrors scripts/AuditComments.json.
The findings match AuditComments.ps1 on the same tree, in the same order, though neither reads the other.

## `public TAuditComment(ITestOutputHelper output)`

Keeps the test output sink so a fact can write its findings beside the failure.

## `public void AuditComment_CommentLines_KeepLineRules()`

Reads every comment file under the configured roots, plus those of the listed root-level files.
A blank line is skipped, and a heading loses its `#` marks before it is judged.
Every line must be one sentence, hold at most the configured words, and carry none of the forbidden characters.
A heading counts only its words outside code spans.
Any hit names the file, line, and the rule it breaks.

## `public void AuditComment_Sources_CarryNoRemark()`

Scans every audited source and listed root-level file for the comment markers its extension declares.
In `.cs` files, string and character literals are blanked first, so a `//` inside a URL does not count.
Every line of an unclosed block comment counts until the line holding its closer.
Files named in the exempt list are skipped, root-level files too, since the generated registry carries a header.
Every extension with markers is scanned, whether or not it pairs with a comment file.

## `public void AuditComment_Sources_CarryCommentFile()`

Checks every configured source and listed root-level file has its paired comment file beside it.
The source list and generated-file exclusions match the script configuration.

## `public void AuditComment_CommentFiles_HaveSource()`

Every comment file under the roots must be the paired comment file of a source.
A comment file left behind by a deleted or renamed source is otherwise never noticed.

## `private static IEnumerable<string> TAuditOwnerRead(string repoRoot)`

Every source that needs a comment file.
That is the paired sources under the roots and the listed root-level files.

## `public void AuditComment_Headings_NameMembers()`

Every signature heading of a comment file names an identifier its source still holds.
It reads the same comment files as the line rules, so the root-level ones count too.
The source is the code or markup file the comment file sits beside, all of them when several share it.
A heading that is a markup snippet or a file name is left out.
A heading for a member that was renamed or deleted is otherwise never noticed.

## `public void AuditComment_StampedFiles_MatchSource()`

A comment file that carries a hash must carry the hash of its paired sources.
A source edit thus flags its comment file until a person rereads the prose and restamps it.
A stale hash always fails, whatever the ceiling.
Restamping is done one file at a time with scripts/StampComment.ps1.
The failure message spells out the fix.
Diff the sources, reread the touched sections, revise, then stamp.
A bare restamp is the evasion it warns against.

## `public void AuditComment_RestampedFiles_ReviseProse()`

Flags comment files restamped while their prose still matches the text they held when stale.
`TAuditCommentSnapshot` keeps that text between runs.
The hits go to the test output under a capitalized warning, and the fact still passes.
The script prints the same list as a `WARN` row.
Each side keeps its own snapshot, so each flags only the stale files it saw itself.

## `public void AuditComment_UnstampedFiles_HoldWithinCeiling()`

Comment files with no hash are backlog from before the stamp existed.
They pass as a warning while their count sits at or below the `Unstamped` ceiling.
The warning and its full list go to the test output, where the script prints a `WARN` row.
A count above the ceiling fails, so a new comment file must be stamped when it is written.

## `public void AuditComment_UnstampedCeiling_MatchesCount()`

The `Unstamped` ceiling equals the count of unstamped comment files.
A ceiling left above the count is stale and fails, so each stamp lowers it on both sides.

## `private static List<string> TAuditStampRead(string repoRoot)`

Every paired comment file whose second line holds no hash or a hash its sources no longer match.
A missing line reads as no hash, a wrong value as a changed source.

## `private static List<(string, string, string)> TAuditStateRead(string repoRoot)`

Every paired comment file with its second line and the hash its sources call for.
The comment files come from the owner list, so the root-level ones count too and come last.
A comment file whose source is exempt is skipped, as for headings.

## `private const string TAuditMissingProblem = "no hash";`

The problem text of a comment file with no hash, as the script prints it.

## `private const string TAuditChangedProblem = "source changed";`

The problem text of a comment file whose hash its sources no longer match.

## `private const string TAuditUnstampedKind = "Unstamped";`

The ceiling key of the unstamped backlog, matching `ceilings.unstamped` in the script configuration.

## `private readonly ITestOutputHelper _tAuditOutput;`

The test output that carries the backlog warning, since a passing fact shows no message.

## `private static string TAuditHashRead(IEnumerable<string> owners)`

The first 16 lowercase hex digits of the SHA-256 of the owners' text.
Each owner is read without its byte order mark, and CRLF or CR becomes LF.
The owners arrive in ordinal file-name order, so the script and the stamp tool join them alike.
Git line-ending settings or a downloaded copy therefore never change the value.

## `private static IReadOnlyList<string> TAuditScanRead(string repoRoot, TAuditScope scope)`

Every tracked file in the scope, read through `TAuditSource`.
A configured root or root-level file that does not exist fails first, as it does in the script.
An empty scope fails rather than passing vacuously.

## `private static string? TAuditMemberRead(string span)`

The identifier a heading names.
It is the last one before a parameter list, an initializer, a body or a base list.
Type arguments are dropped first.

## `private static string TAuditCommentRead(string path)`

Maps `.xaml.cs` to `.xaml.comment.md` and other sources to the matching `.comment.md` name.

## `private static readonly string[] TAuditCodeExtensions`

The files a comment file may describe: its stem itself, a source, a markup file or a code-behind.

## `private static readonly Regex TAuditHeadingPattern`

A second-level heading whose whole text is one code span.

## `private static readonly Regex TAuditFilePattern`

A heading that names a file rather than a member.

## `private static readonly Regex TAuditGenericPattern`

One innermost type argument list, dropped until none is left.

## `private static readonly Regex TAuditIdentifierPattern`

One identifier, verbatim or plain.

## `private static readonly Regex TAuditAbbreviationPattern`

A listed abbreviation with its full stop, which never ends a sentence.

## `private static readonly Regex TAuditLevelPattern`

The `#` marks that open a heading.

## `private static readonly Regex TAuditHashPattern`

The whole hash line.
It is `Hash:`, one space, then 16 lowercase hex digits in a code span.

## `private static string TAuditLineCheck(string line)`

Returns the rule problems one line breaks, joined by a comma, or an empty string.
A leading list marker is dropped before the words are counted.
Code spans are dropped before the forbidden characters and sentence marks are looked for.
A sentence mark followed by a space and a letter starts a new sentence, unless it closes an abbreviation.

## `private static string[] TAuditSourceRead(string path, bool code)`

For code, blanks every string and character literal over the whole file before splitting into lines.
Other files split into lines unchanged.
An ordinary string or character literal never crosses a line break.
A verbatim or raw literal spanning lines keeps its line count, so a hit still reports the right line.
