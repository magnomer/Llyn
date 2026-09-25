# LFontFace.cs

## `public static class LFontFace`

Puts a language pack's declared typography onto the surfaces that show its words.
The editor field and the reading view are given the same record, so a headword looks the same in both.
A pack that declares nothing clears the local value, so the theme's own family and size stand.

### `public static void LFontApply(LWindow window, string language, params DependencyObject[] surfaces)`

Takes any element that carries text, because a text box and a text block share the same font properties.
Both read them from `TextElement`, so one attached property serves either.
The bare form asks for the headword role.
The role form serves whole controls such as the fanqie and script panels.

### `public static void LFontPlace(params FrameworkElement[] surfaces)`

Pins a headword by its baseline rather than by the middle of its line box.
Each family carries its own ascent and descent, so centering the box shifted the letters as the language changed.
Word aligns text by baseline, and a reader expects the stroke bottoms to sit at one height across languages.
The family reports its baseline as a fraction of the size, and the top margin fills up to `LFontBaseline`.
The surface must be top-aligned inside a row of fixed minimum height for the margin to mean anything.
Called after `LFontApply`, because the family it reads is the one just put on.

### `public static void LFontGlyphApply(ResourceDictionary resources, LWindow window, string language)`

Puts the typography a pack declares for its glyph row into the resources of the row's list.
Only the glyph value reads those keys, so the row's label keeps the theme face.
The shared label column then measures the same in both panels.
Setting the font on the list itself once leaked into the label.
That pushed every reading row sideways in the editor alone.
The editor field and the reading chips read the same two keys.
So both draw the characters at one size in one face.
A part the pack leaves out has its key removed, so the theme's default in `PThemeGlyph.xaml` stands.

### `public static void LFontExampleApply(ResourceDictionary resources, LWindow window, string language)`

Puts the typography a pack declares for its example sentences, and for the Glosses under them, into card resources.
Example lines are drawn inside item templates, so they are reached through resources rather than one line at a time.
The editor and the reading view hand in their own dictionaries, and both are filled the same way.
The language is the entry's, since a sentence and its Glosses stand under the entry's word.
A part the pack leaves out has its key removed, so the theme's own value stands.

### `private static FontStyle? LFontStyleRead(string? style)`

A pack writes its slant as a word, `italic` or `oblique`.
Any other word, or none, is answered with nothing, so the theme's upright stands.

### `private static LFont LFontRoleRead(LWindow window, string language, LFontRole role)`

A pack that is missing or unknown must not stop a headword from being drawn.
So a failed read is answered with a blank record and the theme's own typography.
