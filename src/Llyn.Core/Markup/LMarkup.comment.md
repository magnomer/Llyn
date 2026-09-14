# LMarkup.cs

## `public static class LMarkup`

The `.llx` reader and writer, the one door between markup text and markup records.
The format is real XML with a `llyn` root and one `entry` element per entry.
It carries no id of any kind.
A file written by the program, a person or an AI reads the same.
Element names are an external contract and stay plain lowercase English.
The element tree mirrors `LMarkupEntry` one to one.

## `internal const string LMarkupRoot = "llyn";`

The name of the root element.

## `internal const string LMarkupState = "state";`

The one attribute the format knows, marking a value as unknown.

## `internal const string LMarkupUnknown = "unknown";`

The one value the state attribute takes.

## `public const int LMarkupDepthCeiling = 64;`

The deepest element a file may nest, counting the root as one.
A real entry never nears it, since only `meaning` nests at all.

## `public const int LMarkupOmissionCeiling = 1000;`

The most omissions one read reports before it stops counting.

## `public static IReadOnlyList<LMarkupEntry> LMarkupParse(string text)`

Reads `text` into entries, dropping what the format does not know without reporting it.

## `public static IReadOnlyList<LMarkupEntry> LMarkupParse(string text, out IReadOnlyList<LMarkupOmission> omissions)`

Reads `text` into entries in file order and reports what was skipped.
Malformed XML, a root that is not `llyn`, or nesting past the depth ceiling refuses with `LRefusal.LRefusalMarkup`.
An unknown element is skipped with its whole subtree and named in the omissions with its line.
Any attribute other than `state="unknown"` is skipped and named the same way, so an `id` never enters.

## `public static string LMarkupFormat(IReadOnlyList<LMarkupEntry> entries)`

Writes `entries` as indented markup text with no XML declaration.
Line ends are `\n` and the indent is two spaces, so the text is the same on every platform.
An unspecified value writes nothing and an unknown value writes an empty element marked unknown.

## `internal static LStateValue LMarkupValueParse(XElement? element)`

Reads one stated value off an element.
No element or a blank one is unspecified, `state="unknown"` is unknown, and text is specified.

## `internal static void LMarkupValueFormat(XElement parent, string name, LStateValue value)`

Writes one stated value under `parent` as the element `name`.
Unspecified writes nothing, unknown writes an empty element with the state attribute, and specified writes its text.

## `internal static string LMarkupTextParse(XElement? element)`

Reads a plain text field, empty when the element is absent.

## `internal static void LMarkupTextFormat(XElement parent, string name, string? text)`

Writes a plain text field under `parent`, and nothing at all when the text is empty.

## `internal static string LMarkupTextNormalize(string text)`

The text with every character XML 1.0 cannot carry removed.
A control character pasted into a note would otherwise make the writer throw and the export fail.
The input is returned as is when nothing has to go, so the common case allocates nothing.
