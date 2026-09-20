# LScriptStyle.cs

## `public sealed record LScriptStyle(`

One character style a language pack lists under `script`, such as 金文, 小篆 or 隸書.
The style names the web database that draws the character in that hand and how its answer is read.
The engine knows nothing of the site, only that a form is posted and a pattern is matched.
Every URL, field and pattern is pack data, so a new database needs no code.

**Parameters**

- `LScriptStyleName` — The style's name, shown as the row's chip and stored beside every picture of it.
- `LScriptStyleUrl` — The address the search form is posted to.
- `LScriptStyleForm` — The form fields posted, in pack order, with `{word}` replaced by the character.
  A database that needs a hidden field answers an error without it, so the pack lists every field it wants.
- `LScriptStylePattern` — A .NET regex matched over the whole answer, once per glyph picture.
- `LScriptStyleImage` — The group of the pattern holding the picture's address.
- `LScriptStyleCaption` — The group holding the caption printed under the picture, or zero for none.
  Tags inside the caption are dropped and its lines joined by a space.
- `LScriptStylePrefix` — Base URL prepended to a relative picture address, or `null` when it is absolute.
- `LScriptStyleRewrite` — Ordered rewrite rules applied to the picture address before it is fetched.
  A site that draws the glyph at a requested size is asked for a large original this way.
- `LScriptStyleGloss` — A regex whose first group reads a gloss the page prints beside the pictures, or `null`.
  The 說文 entry under the small seal results is read this way.
