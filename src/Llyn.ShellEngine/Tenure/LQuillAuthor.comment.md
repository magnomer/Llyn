# LQuillAuthor.cs
Hash: `69ecc10a26693e88`

## `public sealed class LQuillAuthor`

The typed edit of a held Author, building exactly one request.
An Author has only its name to type, so this quill holds one member.

## `private readonly LTenure _lQuillAuthorTenure;`

The tenure every request is built for and handed to.

## `public LQuillAuthor(LTenure tenure)`

Builds the edit over one tenure, which it never swaps.

## `public void LQuillAuthorSet(string name)`

Defers the Author's typed name.
