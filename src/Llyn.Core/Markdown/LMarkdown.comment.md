# LMarkdown.cs

## `public static class LMarkdown`

The note dialect: the one reader every writer and the engine share.
It is a small CommonMark subset, so a note written here reads the same in any Markdown viewer.
Block shapes are headings, paragraphs, bullet and numbered items, quotes, fenced code and rules.
Inline shapes are those of `LMarkdownInline`.

## `public static string LMarkdownNormalize(string? text)`

Brings note text to the form the archive stores.
Line ends become `\n`, trailing spaces leave every line, and blank lines leave both ends.
Whitespace-only text becomes empty, which the engine reads as no note at all.

## `public static IReadOnlyList<LMarkdownBlock> LMarkdownParse(string? text)`

Reads `text` into flat blocks in reading order.
A blank line ends a paragraph and a line break inside one is kept, since the editor shows it.
An item's continuation is any indented line that opens no other block.

## `private static int LMarkdownCodeParse(List<LMarkdownBlock> blocks, string[] lines, int place)`

Gathers the lines between two fences verbatim and returns the line after the closing fence.
A fence never closed runs to the end.

## `private static bool LMarkdownRuleCheck(string bare)`

Reports whether the line is three or more of one rule character, spaces allowed between.

## `private static bool LMarkdownHeadingParse(List<LMarkdownBlock> blocks, string bare)`

Adds a heading when the line opens with one to six hashes and a space, and says whether it did.
Closing hashes are dropped.

## `private static int LMarkdownQuoteParse(List<LMarkdownBlock> blocks, string[] lines, int place)`

Gathers consecutive `>` lines into one quote and returns the line after them.

## `private static bool LMarkdownItemCheck(string line, out LMarkdownKind kind, out int level, out string body)`

Reports whether the line opens a list item, and if so which kind, how deep and with what text.
Depth is the leading indent in steps of two spaces.

## `private static int LMarkdownItemParse(List<LMarkdownBlock> blocks, string[] lines, int place)`

Adds one item with its indented continuation lines and returns the line after them.

## `private static bool LMarkdownContinueCheck(string line)`

Reports whether the line is an indented continuation that opens no block of its own.

## `private static int LMarkdownParagraphParse(List<LMarkdownBlock> blocks, string[] lines, int place)`

Gathers consecutive plain lines into one paragraph and returns the line after them.

## `private static bool LMarkdownPlainCheck(string line)`

Reports whether the line continues a paragraph rather than opening any other block.
