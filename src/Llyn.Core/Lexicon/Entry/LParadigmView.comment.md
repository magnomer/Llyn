# LParadigmView.cs
Hash: `5d1e0fada22b91d9`

## `public sealed record LParadigmView(LParadigmTable LParadigmViewCollapsed, LParadigmTable LParadigmViewExpanded)`

The inflection box of one entry, in both views.
It is the whole answer below Conduct, so Conduct only maps it.
Toggling between the views is visual state and asks nothing new.

**Parameters**

- `LParadigmViewCollapsed`: the short view the box opens with.
- `LParadigmViewExpanded`: the full table declared by the layout.

## `public static LParadigmView? LParadigmViewScan(LInflectionLayout layout, IReadOnlyList<LParadigmSlot> slots, bool pending, bool enabled, bool held, bool custom, LInflectionBook? book, string headword)`

Builds both views from the layout and the entry's slots.
Only slots declared on the layout's part take part.
It answers null when no slot belongs to that part, so the box falls back to the list.
It reads the stored marks, and asks `book` only for the root of each custom cell.
`book` and `headword` locate that root, and a null book leaves the stored marks as they are.
`pending` and `enabled` feed the status of each cell.
`held` picks the held tip for a lost cell, true only for the editor.
`custom` picks the layout's custom pair of sheets, or the default pair when it is off.
With it off, no cell carries marks or a split.
A layout without a custom pair uses its default sheets under either setting.

## `private static LParadigmTable LParadigmViewResolve(LInflectionSheet sheet, IReadOnlyList<LParadigmSlot> slots, bool pending, bool enabled, bool held, bool custom, LInflectionBook? book, string headword)`

Builds one table from one sheet of the layout.
Each cell uses the first slot whose code sequence exactly matches the sheet's cell.
A cell with no slot has empty text, no tip and no marks.
Only text-status cells with a stored inflection expose its text and stored marks.
With `custom` off, those cells show their text without marks and without a split.
With `custom` on, a cell whose prediction has a root is divided into root and ending.
The marks then colour the root or the ending as a whole, through `LParadigmViewDivide`.
A divided cell carries its split only when both sides hold letters.
A cell without a book, a root, or an ending start past 0 keeps its stored marks.
The stored marks stay in the database either way.
Every other matched cell takes its text and tip from `LParadigmShown.LParadigmShownResolve`, without marks.
That is the one place the view applies the status rule, so every reader gets a ready cell.

## `private static IReadOnlyList<LInflectionMark> LParadigmViewDivide(string text, IReadOnlyList<LInflectionMark> marks, int split)`

Widens the letter marks to whole parts, so an irregular part reads as one unit.
The root part is marked when any mark overlaps it, and so is the ending part.
An empty ending is never marked, and the answer is empty when no mark overlaps either part.
