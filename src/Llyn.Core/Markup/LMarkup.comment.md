# LMarkup.cs

## `public static class LMarkup`

The `.llx` reader and writer, the one door between the markup tree and markup records.
The format is real XML with a `llyn` root and one `entry` element per entry.
It carries no id of any kind.
A file written by the program, a person or an AI reads the same.
Element names are an external contract and stay plain lowercase English.
The element tree mirrors `LMarkupEntry` one to one.
Core sees the tree as `LMarkupNode` values and never the XML.
`LMarkupFile` in Infrastructure parses the text into the tree and formats the tree back.

## `internal const string LMarkupRoot = "llyn";`

The name of the root element.

## `internal const string LMarkupState = "state";`

The one attribute the format knows, marking a value as unknown.

## `internal const string LMarkupUnknown = "unknown";`

The one value the state attribute takes.

## `public const int LMarkupDepthCeiling = 64;`

The deepest element a file may nest, counting the root as one.
A real entry never nears it, since only `meaning` nests at all.
The adapter enforces it while streaming the text, before any tree is built.

## `public const int LMarkupOmissionCeiling = 1000;`

The most omissions one read reports before it stops counting.

## `private static readonly IReadOnlyDictionary<string, string> LMarkupUnknownMark`

The one attribute set the writer ever emits, `state="unknown"`.

## `public static IReadOnlyList<LMarkupEntry> LMarkupParse(LMarkupNode root, out IReadOnlyList<LMarkupOmission> omissions)`

Reads the tree under `root` into entries in file order and reports what was skipped.
A root that is not `llyn` refuses with `LRefusal.LRefusalMarkup`.
Malformed XML and nesting past the depth ceiling are refused by the adapter before the tree exists.
An unknown element is skipped with its whole subtree and named in the omissions with its line.
Any attribute other than `state="unknown"` is skipped and named the same way, so an `id` never enters.

## `public static LMarkupNode LMarkupFormat(IReadOnlyList<LMarkupEntry> entries)`

Writes `entries` as a tree under one `llyn` root.
An unspecified value writes nothing and an unknown value writes an empty element marked unknown.

## `internal static LStateValue LMarkupValueParse(LMarkupNode? element)`

Reads one stated value off an element.
No element or a blank one is unspecified, `state="unknown"` is unknown, and text is specified.

## `internal static void LMarkupValueFormat(List<LMarkupNode> parent, string name, LStateValue value)`

Writes one stated value into `parent` as the element `name`.
Unspecified writes nothing, unknown writes an empty element with the state attribute, and specified writes its text.

## `internal static string LMarkupTextParse(LMarkupNode? element)`

Reads a plain text field, empty when the element is absent.

## `internal static void LMarkupTextFormat(List<LMarkupNode> parent, string name, string? text)`

Writes a plain text field into `parent`, and nothing at all when the text is empty.
