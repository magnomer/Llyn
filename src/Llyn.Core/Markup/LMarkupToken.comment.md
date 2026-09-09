# LMarkupToken.cs

## `public static partial class LMarkup`

The scanner half of the markup reader.

It turns document text into tags and knows nothing of what a tag means.
Every reader above it works on tokens rather than on characters.
Separating the two keeps one place responsible for where a file is malformed.
It also keeps the meaning of `<use>` out of the code that finds the angle brackets.

## `internal enum LMarkupTokenKind`

What one scanned tag is, as far as the scanner can tell without knowing the format's meaning.
`LMarkupTokenText` is a leaf tag that carries text, `LMarkupTokenEnter` opens a block, and `LMarkupTokenLeave` closes the block it names.
Blocks are bounded rather than nested in the token stream.
The scan stays flat.
A caller that wants the contents of one `<sense>` reads from its enter token to its leave token.
That keeps the scanner from having to know which tags belong inside which block.
That is the format spec's business and not the scanner's.

## `internal readonly record struct LMarkupToken`

One tag, as scanned.
`LMarkupTokenName` is the tag name exactly as written.
`LMarkupTokenMark` holds the attributes the tag carried, each under its own name.
A map rather than a field per attribute, because the format names twenty-six of them.
A positional record of that width would be read by counting commas.
The scanner keeps only the attributes the format defines and drops the rest.
Section 1 of the format spec makes an unrecognised attribute ignorable rather than an error.
An attribute the tag never carried is absent from the map, and `ref=""` is present and empty.
Only that difference keeps *unspecified* apart from *unknown* on an attribute.

`LMarkupTokenText` is the literal inner text with leading and trailing whitespace trimmed.
Everything inside it is untouched.
`&`, `<` and quotes are ordinary characters, as section 7 of the format spec requires.
`LMarkupTokenEmpty` is set for the empty-element form, `<tag></tag>` or `<tag/>`.
That is how the format writes the *unknown* state.
A later reader turns that flag into `LStateValueUnknown`.
It turns the absence of the token altogether into `LStateValueUnspecified`.

## `internal string? LMarkupTokenRead(string mark)`

Reads one attribute off the token by name.
It returns `null` when the tag carried no such attribute.
It returns the empty string when the tag carried it written empty.
So every caller reads the three states through one door instead of probing the map.

**Parameters**

- `mark` — The attribute name as the format writes it, such as `ref` or `tone-local`.

## `internal static IReadOnlyList<LMarkupToken> LMarkupScan(string text)`

Turns a whole Llyn Markup document into the ordered stream of tag tokens it declares.
The scan is hand-written rather than handed to an XML reader, because Llyn Markup is not XML.
Its text is literal.
A document that says `a & b` inside a `<meaning>` is well formed here and rejected there.
Reading it as XML would force authors to escape ordinary punctuation.
Section 7 of the format spec exists to promise they never have to.

The scan walks the text once.
Outside a tag, anything that is not `<` is skipped.
So whitespace and stray prose between blocks are insignificant rather than an error.
At a `<`, the tag name is read, then its attributes.
Only the attributes the format defines are kept.
The rest are dropped.
An unrecognised attribute, like an unrecognised tag, must not break an older file.
A block tag emits an enter token and, at its `</name>`, a leave token.
The block list holds every tag the format writes children inside.
`<catalog>`, `<example>`, `<situation>`, `<register>`, `<video>`, `<inflection>`, `<pronunciation>` and `<relation>` joined it with the catalog.
`<llyn>` is the document element and is a block like any other.
`<sense>` may hold a `<sense>`, and depth counting is what keeps the inner one from closing the outer.
Any other tag is a text tag.
Its content is taken literally up to the first `</name>`.
So the one sequence an author cannot write inside text is that tag's own closing tag.
Nesting therefore goes one level for text tags — a tag inside a text tag is part of the text.

A block tag written in the empty-element form emits an enter token and a leave token at once.
That is how a card's `<situation ref="hearth"/>` reaches its reader.
A citation and a declaration share a tag name, and only the attributes tell them apart.

Structural damage is an error.
The message names the character position so an import can report where the file went wrong.
Unclosed tags and a `<` that no name follows raise a `FormatException`.
So do a closing tag matching no open block and a block still open at the end.

**Parameters**

- `text` — The whole Llyn Markup document.

**Returns** — Every tag in the order the document writes it.
Block tags are bounded by an enter and a leave token.
