# CDiweiPage.cs

## `public sealed record CDiweiPage(`

The page of one diwei cell, as the diwei reader prints it.
The blank page carries empty text and no section.

**Parameters**

- `CDiweiPageLanguage`: the language of the cell, which picks its font and flag.
- `CDiweiPageKey`: the cell key, printed as the headword.
- `CDiweiPageSections`: the sections, already ordered by the engine.
- `CDiweiPageEmpty`: whether the cell holds no section, as the engine judges it.
