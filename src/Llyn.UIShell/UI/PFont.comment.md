# PFont.cs

## `internal static class PFont`

Puts a language pack's declared typography onto the surfaces that show its words.
The editor field and the reading view are given the same record, so a headword looks the same in both.
A pack that declares nothing clears the local value, so the theme's own family and size stand.

### `internal static void PFontApply(LEngine engine, string language, params DependencyObject[] surfaces)`

Takes any element that carries text, because a text box and a text block share the same font properties.
Both read them from `TextElement`, so one attached property serves either.
The bare form asks for the headword role, and the role form serves the glyph chips with their serif face.

### `internal static void PFontExampleApply(ResourceDictionary resources, LEngine engine, string language)`

Puts the typography a pack declares for its example sentences, and for the Glosses under them, into card resources.
Example lines are drawn inside item templates, so they are reached through resources rather than one line at a time.
The editor and the reading view hand in their own dictionaries, and both are filled the same way.
The language is the entry's, since a sentence and its Glosses stand under the entry's word.
A part the pack leaves out has its key removed, so the theme's own value stands.

### `private static FontStyle? PFontStyleRead(string? style)`

A pack writes its slant as a word, `italic` or `oblique`.
Any other word, or none, is answered with nothing, so the theme's upright stands.

### `private static LFont PFontRoleRead(LEngine engine, string language, LFontRole role)`

A pack that is missing or unknown must not stop a headword from being drawn.
So a failed read is answered with a blank record and the theme's own typography.
