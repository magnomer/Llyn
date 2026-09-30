# CEnsignSheet.cs

## `public sealed record CEnsignSheet<CEnsignSheetKind>(`

What an area answers once the flag fill has run: the loaded languages and its ready rows.
The rows are read only after the fill, so every row a driver paints finds its flag already drawn.
One answer carries both, so a driver Refine makes one request for its menu and its list.

**Parameters**

- `CEnsignSheetLanguages` — The languages the fill read, which a language menu offers.
  When the fill failed, they are read without flags.
- `CEnsignSheetRows` — The area's ready rows, read after the fill.
