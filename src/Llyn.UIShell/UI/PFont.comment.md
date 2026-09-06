# PFont.cs

## `internal static class PFont`

Puts a language pack's declared typography onto the surfaces that show its words.
The editor field and the reading view are given the same record, so a headword looks the same in both.
A pack that declares nothing clears the local value, so the theme's own family and size stand.

### `internal static void PFontApply(LEngine engine, string language, params DependencyObject[] surfaces)`

Takes any element that carries text, because a text box and a text block share the same font properties.
Both read them from `TextElement`, so one attached property serves either.

### `private static LFont PFontRead(LEngine engine, string language)`

A pack that is missing or unreadable must not stop a headword from being drawn.
So a failed read is answered with a blank record and the theme's own typography.
