# LMarkupKey.cs

## `public static partial class LMarkup`

The key half of the markup reader.
A key is a name the author chooses to point one element at another inside one file.
This file owns the namespace those names live in and the rules a citation is held to.
It is separate from the catalog because declaring a row and checking a pointer are two responsibilities.
Every element in a document may cite, and only a few declare.

## `private static readonly IReadOnlyDictionary<string, LMarkupRowKind> LMarkupCitationList`

What kind of row each citing tag expects behind its `ref`.
A `<use ref="...">` quotes an example and a `<register ref="...">` cites a register.
The tag is what says which, because `ref` alone says only that something is cited.
A tag not listed here cites nothing the format defines, so its `ref` is ignored.
Section 1 of the format spec makes an unrecognised tag ignorable rather than an error.

## `internal static string LMarkupKeyValidate(string? named, string kind)`

Confirms one declared key is a key, and hands it back.
A key is one or more letters, digits, `-`, or `_`, which section 2 of the format spec states.
A row that declared none raises a `FormatException`, and so does one that declared an empty key.
A key is never three-state: it is present, or the element declares none.
An empty `id` is therefore a mistake rather than an unreadable key, and saying so beats guessing.
The message names the kind of row, since that is what the author is looking at.

**Parameters**

- `named` — The `id` as written, or `null` when the element wrote none.
- `kind` — The row's kind as the format names its tag, used only in the message.

## `internal static IReadOnlyDictionary<string, LMarkupRowKind> LMarkupKeyRead(IReadOnlyList<LMarkupToken> tokens, LMarkupCatalog catalog)`

Collects the whole document's key namespace: the catalog's rows, and every entry and sense key.

Keys share one namespace across the file, so an entry may not reuse a key a source took.
Section 2 of the format spec says one key names one row, and a repeat raises a `FormatException`.
Only `<entry>` and `<sense>` declare outside the catalog.
A `<pos id="verb">` names a part of speech a language pack defines, which is not a key at all.
Reading every `id` as a declaration would put that name in the namespace and collide with a real key.

**Parameters**

- `tokens` — The whole document's tokens.
- `catalog` — The rows the catalog declared, and the kind of each.

**Returns** — Every key the document declares, and the kind of the row it names.

## `internal static void LMarkupCitationValidate(IReadOnlyList<LMarkupToken> tokens, IReadOnlyDictionary<string, LMarkupRowKind> keys)`

Holds every citation in the document against the namespace, and refuses the file over a bad one.

The sweep is over the tokens rather than over the readers that consume them.
A citation is checked once, wherever it stands, and a reader never has to check its own.
It also means a citation inside a tag no reader has been written for is still checked.

`ref` is read against the kind its tag expects, and `src` names a source.
`entry` and `sense` name an entry and a sense, which are keys the file declares outside the catalog.
A citation that is absent or empty names nothing and is not checked.
Section 6 of the format spec keeps *unspecified* and *unknown* apart, and neither points anywhere.

**Parameters**

- `tokens` — The whole document's tokens.
- `keys` — Every key the document declares, and the kind of the row it names.

## `private static void LMarkupCitationValidate(string? cited, LMarkupRowKind wanted, IReadOnlyDictionary<string, LMarkupRowKind> keys)`

Holds one citation against the namespace.

A key no element declared raises a `FormatException` naming the key.
A citation pointing at nothing is a broken file, not a field to be read leniently.
Leniency is for tags the format may one day add, not for a pointer the author meant.
A key naming a row of another kind raises a `FormatException` naming the key and both kinds.
The author wrote one name for two things, and the message has to say which two.

**Parameters**

- `cited` — The citation as written, or `null` when the tag carried none.
- `wanted` — The kind the citing tag expects.
- `keys` — Every key the document declares, and the kind of the row it names.

## `public static string LMarkupKeyCreate(string? seed, LMarkupRowKind kind, ISet<string> taken)`

Derives the key one exported row is declared under, and reserves it.

A key is written for a person to read in a diff, so it comes from the row's own content.
An author's name, a source's title, an example's opening words.
The seed is lowercased, its runs of letters and digits joined by hyphens, and everything else dropped.
Five words and forty characters are enough to recognise a row and short enough to read.
A row whose seed says nothing falls back to the name of its kind.

Keys share one namespace across a whole document, so a stem already taken needs a suffix.
The smallest number that is free is appended, starting at two.
The caller reserves rows in a fixed order, so the same workspace produces the same keys twice running.
An export that reshuffled its keys would make every diff between two exports unreadable.

**Parameters**

- `seed` — The row's own content, or `null` when it has none to offer.
- `kind` — The kind of row, used to name a key nothing else could name.
- `taken` — Every key the document has already declared, which this call adds to.

**Returns** — A key no other row in the document holds.

## `private static string LMarkupKeyFormat(string? seed)`

Turns content into the stem of a key, or into nothing when the content offers no letter or digit.
A key is one or more letters, digits, `-`, or `_`, which section 2 of the format spec states.
Letters here means letters in any script, so a Korean headword keys as itself rather than as a number.
