# QFontFace.cs
Hash: `affd158188696d07`

## `public static class QFontFace`

Puts a language pack's declared typography onto the surfaces that show its words.
The editor field and the reading view are given the same record, so a headword looks the same in both.
A pack that declares nothing clears the local value, so the theme's own family and size stand.

### `public static void QFontRefine(CFont font, params DependencyObject[] surfaces)`

Puts a font Conduct already read onto every surface, so a block handed its font asks nothing more.
Takes any element that carries text, because a text box and a text block share the same font properties.
Both read them from `TextElement`, so one attached property serves either.

### `private static void QFontSurfaceRefine(DependencyObject surface, CFont font)`

Puts one font's family and size onto one surface, clearing any part the pack leaves out.

### `public static void QFontBaselineRefine(params FrameworkElement[] surfaces)`

Pins a headword by its baseline rather than by the middle of its line box.
Each family carries its own ascent and descent, so centering the box shifted the letters as the language changed.
Word aligns text by baseline, and a reader expects the stroke bottoms to sit at one height across languages.
The family reports its baseline as a fraction of the size, and the top margin fills up to `QFontBaseline`.
The surface must be top-aligned inside a row of fixed minimum height for the margin to mean anything.
Called after `QFontRefine`, because the family it reads is the one just put on.

### `public static void QFontGlyphRefine(ResourceDictionary resources, CFont glyph)`

Puts a glyph font Conduct already read into the row's resources.
It puts the typography a pack declares for its glyph row into the resources of the row's list.
Only the glyph value reads those keys, so the row's label keeps the theme face.
The shared label column then measures the same in both panels.
Setting the font on the list itself once leaked into the label.
That pushed every reading row sideways in the editor alone.
The editor field and the reading chips read the same two keys.
So both draw the characters at one size in one face.
A part the pack leaves out has its key removed, so the theme's default in `PThemeGlyph.xaml` stands.

### `public static void QFontExampleRefine(ResourceDictionary resources, CFont example)`

Puts the typography Conduct read for example sentences into card resources.
Example lines are drawn inside item templates, so they are reached through resources rather than one line at a time.
The editor and the reading view hand in their own dictionaries, and both are filled the same way.
Each area reads the font in its own entry's language, so no driver hands a language back.
A part the pack leaves out has its key removed, so the theme's own value stands.

### `public static void QFontGlossRefine(ResourceDictionary resources, CFont gloss)`

Puts the typography Conduct read for the Glosses under example sentences into the same card resources.
The gloss also carries its slant, which `QFontStyleRead` maps.

### `private static FontStyle? QFontStyleRead(CFontSlant style)`

Maps the slant Conduct closed, italic or oblique, onto its WPF style.
`CFontSlantTheme` is answered with nothing, so the theme's upright stands.

