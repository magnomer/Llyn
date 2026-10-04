# PLeaf.cs
Hash: `741a1a101846e18c`

## `internal static class PLeaf`

The fills of the display's card templates, which the Veneer returned without a binding.
Each fill finds a named part and paints it from the card's presentation item, [QLeafItem](QLeafItem.comment.md).
Every text arrives looked up on the item, so the fills only paint.

## `internal static void PLeafCardRefine(FrameworkElement container, object item, string? _)`

Paints one meaning or collocation card: badge number, title or kind, expression, meaning and body.
A muted title leaves the kind caption in its place, and a muted line folds away.
The body's content control is handed the card, so its template is realized before its rows are filled.

## `private static void PLeafBodyRefine(FrameworkElement body, QLeafItem card)`

Sets the items of every body row and folds each row that has none.
The link chips arrive paired with their flags for the chip template.
The translation row takes the wider margin when no situation row stands above it.
The picture and video rows fold by the look sheet, and their lines get the shared media fills.

## `private static void PLeafListRefine<PLeafRow>(FrameworkElement body, string name, IReadOnlyList<PLeafRow> rows, Action<FrameworkElement, object, string?> fill)`

Sets one chip row's items, folds it when empty and attaches its chip fill.

## `private static void PLeafSentenceRefine(FrameworkElement container, object item, string? _)`

Paints one example line from its [QLeafLine](QLeafLine.comment.md).
The line holds the frame, the sentence, the byline and the Glosses.
The sentence draws the runs Conduct divided.
It keeps only its sentence row id, which the click hands back to the find gate.
The sentence's top margin drops it to the frame's baseline, computed by `QFontConverter`.
The margin is bound to both fonts, so a font the theme changes later moves the baseline too.
The byline binds the sentence's margin, family and size, so both keep one baseline live.
The Glosses arrive as the editor's rows and reuse its Gloss row fill, since their parts carry the same names.

## `private static void PLeafSituationRefine(FrameworkElement container, object item, string? _)`

Writes a situation chip's title.

## `private static void PLeafRegisterRefine(FrameworkElement container, object item, string? _)`

Writes a register chip's name.

## `private static void PLeafTagRefine(FrameworkElement container, object item, string? _)`

Writes a tag chip's text.

## `private static void PLeafLinkRefine(FrameworkElement container, object item, string? _)`

Writes a translation chip's flag, headword and language.
