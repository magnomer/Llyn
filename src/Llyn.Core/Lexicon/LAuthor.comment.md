# LAuthor.cs

## `public sealed record LAuthor(`

One Author — independent data owned by nothing.
No Reference contains an Author.
Any number of References *reference* it instead.
The order an Author takes lives on each reference rather than here.
So the same Author can be first on one Reference and third on another.
`LAuthorId` is the identity, an opaque and program-generated stable id.
`LAuthorName` is display text and never identity.
Renaming an Author leaves every Reference to it untouched.
Two Authors reading alike remain distinct rows.

`Anonymous` is an ordinary specified Author like any other.
It is never a stand-in for a Reference whose author is unspecified or unknown.
Those are states on the Reference itself (`LReference.LReferenceAuthorState`).

**Parameters**

- `LAuthorId` — Opaque, program-generated stable id.
- `LAuthorName` — The author's name, which is display text and never identity.
