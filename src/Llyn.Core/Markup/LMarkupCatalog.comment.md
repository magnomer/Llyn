# LMarkupCatalog.cs

## `public static partial class LMarkup`

The catalog half of the markup reader.
It turns one `<catalog>` block into the rows the rest of the document points at.
Sharing is what the catalog exists for.
One example is quoted by many cards, one source is cited by many examples, one author is credited on many sources.
A row is declared once, under a key, however many citations name it.

## `public enum LMarkupRowKind`

What one declared key names.
A citation must name a row of the kind it expects, and this is what makes that checkable.
`<situation ref="oed">` where `oed` is a source is an error section 2 of the format spec states outright.

`LMarkupRowEntry` and `LMarkupRowSense` are kinds no catalog row takes.
An entry and a sense declare keys of their own, in the same namespace, and a citation may name either.
Leaving them out would make a translation naming a source look like a translation naming an entry.

## `public sealed record LMarkupCatalog`

Every row the document declared, indexed by the key it was declared under.

`LMarkupCatalogRow` is the key namespace itself, and it gives the kind of every key in the file.
It is the only member holding entry and sense keys, because those declare no row here.
The rest hold the rows themselves, one dictionary per kind.

The rows are stored as the values the rest of the program already has a shape for.
A source is an `LMarkupReference`, an example an `LExample`, a register an `LRegister`.
So the layer that saves them writes what the editor writes, through the calls the editor uses.
A row carries an empty id, because a key is not an identifier and never becomes one.
Section 2 of the format spec says a key does not survive import.

The rows are a map rather than a list because their order carries nothing.
Section 8 of the format spec makes the catalog the one place where order is not meaningful.

## `public static LMarkupCatalog LMarkupCatalogCreate()`

The empty catalog, for a document that declares no rows.
It is a value rather than a null, so a reader never asks whether a catalog was written.

## `public static string LMarkupRowFormat(LMarkupRowKind kind)`

Names one kind as the format writes its tag.
The only reader of the result is a person looking at a refusal message.
So a citation of the wrong kind can say what it found and what it wanted in the author's own words.

**Parameters**

- `kind` — The kind to name.

## `public readonly record struct LMarkupReference`

One `<source>` block as read.
`LMarkupReferenceId` is the key the block declared.
It is repeated outside the reference so a caller can index the block by it.
The caller need not unpack the value.

`LMarkupReferenceAuthor` holds the keys of the authors the block credited, in credit order.
It exists because a source's author is not part of an `LReference`.
An `LReference` records only whether an author was recorded, in `LReferenceAuthorState`.
The authors themselves are catalog rows the file declares once and credits anywhere.
Reading is pure and cannot create or match those rows.
So the credit leaves beside the reference as the keys the block wrote.
It is left for the layer that owns author identity to attach.
Dropping it here would silently lose a written credit on import.
That is the one thing the format promises not to do.

It is a list rather than one value because section 3 of the format spec allows several.
A source may credit several authors, and the format keeps their order.
The list holds only the credits that named somebody.
An `<author/>` with no `ref` contributes no key, since there is no row to point at.
That the source credits someone unreadable is already recorded in the reference's author state.
So the list is empty for a source with no `<author>`.
It is also empty for one whose only `<author>` named nobody.
The two stay apart on the reference, which is where that distinction belongs.

## `internal static LMarkupReference LMarkupReferenceRead(IReadOnlyList<LMarkupToken> tokens)`

Turns the tokens of one `<source>` block into the reference it declares, keyed by its `id`.
The `id` comes from the block's enter token, which carries every attribute the tag wrote.
A block written without one, or with an empty one, raises a `FormatException`.
Section 2 of the format spec gives every catalog row a key.
A source that has none can be cited by nothing.
It cannot be told apart from the next one that has none either.
Refusing it says so where the file is wrong.
Reading it as an empty key would let two unrelated works collapse into one.
The first would be lost without a word.

Only the block's own direct children are read, through `LMarkupLeafRead`.
So a block nested inside a source contributes none of its fields to it.

Every field runs through the same three-state reading the card fields do.
The author is the exception.
It is the one repeatable field here, and the one whose value does not live on the reference.
Its state is worked out from the tags as a whole.
It is *specified* as soon as one `<author ref="...">` names a row.
It is *unknown* when the only ones written named nobody.
It is *unspecified* when none was written.
That state is stored on the reference.
The keys leave beside it in the order the block wrote them.

**Parameters**

- `tokens` — Every token of one `<source>` block, in document order.

**Returns** — The reference, its key, and the author keys the block credited.

## `private static LReferenceKind LMarkupKindRead(LMarkupToken? token)`

Reads a source's `<kind>` into the enumeration that stores it.
An absent tag is unspecified, an empty one is unknown, and a name Llyn does not know reads as unspecified.
Section 3 of the format spec names the eight kinds a source may take.

**Parameters**

- `token` — The `<kind>` tag as scanned, or `null` when the source wrote none.

## `internal static LMarkupCatalog LMarkupCatalogRead(IReadOnlyList<LMarkupToken> tokens, int first, int last)`

Reads one `<catalog>` block into its rows.

Only the block's own direct children are read.
A nested block is handed whole to the reader for its kind, and the walk resumes after its leave token.
So a `<title>` inside a situation is never mistaken for a source's.
A tag the format does not name in the catalog is skipped, as section 1 requires.

Every row must declare a key, and a row without one raises a `FormatException`.
A row nothing can name is a row nothing can share, which is what the catalog is for.
A key declared twice raises a `FormatException` naming the key.
One key names one row, and a repeat would make the file's own citations ambiguous.
Nothing the author wrote would choose between the two, and the other row would vanish.

Citations inside the catalog are left as the keys they were written as.
A source may credit an author declared after it, and an example may cite either.
Resolving as the walk goes would refuse the sample in section 10 of the format spec.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the catalog's enter token.
- `last` — Index of its matching leave token.

**Returns** — The rows the catalog declares, each under its key.

## `private static LExample LMarkupExampleRead(LMarkupToken token, IReadOnlyList<LMarkupToken> block)`

Reads one `<example>` row.
`lang` names the language the sentence is in, and it is plain text rather than three-state.
`<text>`, `<trans>` and `src` each carry the three states, so each is read through `LMarkupStateRead`.
The `src` stays the key the file wrote, because no source row exists yet.

**Parameters**

- `token` — The example's enter token, which carries `lang` and `src`.
- `block` — Every token of the example block, in document order.

## `private static LSituation LMarkupSituationRead(IReadOnlyList<LMarkupToken> block)`

Reads one `<situation>` row.
All three of its fields carry the three states, as section 3 of the format spec says.

**Parameters**

- `block` — Every token of the situation block, in document order.

## `private static LRegister LMarkupRegisterRead(LMarkupToken token, IReadOnlyList<LMarkupToken> block)`

Reads one `<register>` row.
`lang` names the language the register belongs to, and `<name>` carries the three states.
`builtin="yes"` marks a register a language pack ships rather than one the user wrote.
Any other value is a user register, since the format states the one form that means built-in.
What that flag then does to the store is the saving layer's decision, not the reader's.

**Parameters**

- `token` — The register's enter token, which carries `lang` and `builtin`.
- `block` — Every token of the register block, in document order.

## `private static LVideo LMarkupVideoRead(IReadOnlyList<LMarkupToken> block)`

Reads one `<video>` row: its `<location>` and its `<span>`, each in the three states.

**Parameters**

- `block` — Every token of the video block, in document order.

## `private static string LMarkupRowAdd(IDictionary<string, LMarkupRowKind> rows, LMarkupToken token, LMarkupRowKind kind)`

Declares the key one row's tag carries, and returns it.
The key is read off the tag's `id`, which is where the format writes a declaration.

**Parameters**

- `rows` — The keys declared so far, and the kind of each.
- `token` — The row's tag, block or leaf.
- `kind` — What the row is.

## `private static string LMarkupRowAdd(IDictionary<string, LMarkupRowKind> rows, string? named, LMarkupRowKind kind)`

Declares one key and returns it.
The key is validated first, so a malformed one is refused where it was written.
A key already declared raises a `FormatException` naming it.
The message names the key rather than the row, because the key is what the author must change.

**Parameters**

- `rows` — The keys declared so far, and the kind of each.
- `named` — The `id` the row wrote, or `null` when it wrote none.
- `kind` — What the row is.
