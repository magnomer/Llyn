# QSCustoms.cs

## `internal sealed class QSCustoms`

The customs window's showing: the rows it declares, the intakes Accept sends, and the omission face.
Each row's mode and target live in the `CSCustoms` gate, and the window only shows and relays them.
The window is the veneer's `PSCustoms` page, pulled fresh by contract ID and held in `_qsCustomsSurface`.

## `private static readonly IReadOnlyDictionary<string, CSCustomsMode> QSCustomsChoice =`

Maps each named mode row of the dropdown to the gate's mode.
The surface is handed no mode value, so the pick is read back by the row's contract ID.

## `private QSCustoms(PWindow host, CSCustoms customs, IReadOnlyList<QSCustomsItem> items)`

Pulls the window over the host, attaches both row fills and subscribes the three buttons.
Accept starts lit only when the gate finds every row ready.
The omission face passes an empty gate and no rows.

## `private ItemsControl QSCustomsList`

Each named part is pulled from the window by its contract ID.

## `internal static IReadOnlyList<LMarkupIntake>? QSCustomsShow(PWindow host, IReadOnlyList<LMarkupEntry> entries)`

Looks up each entry's candidates, so the gate starts every row defaulted before the window opens.
Every row is numbered by its place in the file, so a reader can tell twin rows apart.

## `internal static void QSCustomsOmissionShow(PWindow host, IReadOnlyList<LMarkupOmission> omissions)`

Shows the omissions the import reported on the window's second face.
Each omission becomes a line number and its text before the window sees it.
An empty list shows nothing, so a clean import ends without a further click.
The declaration's default and cancel keys are released so Close answers both.

## `private static IReadOnlyList<LMarkupIntake>? QSCustomsIntakeRead(`

Shows the declaration and reads one intake per row in file order.
Null when the window closes without Accept.
The intakes are built either way, so the dialog's answer decides only what is handed back.

## `private static IReadOnlyList<long> QSCustomsCandidateRead(IReadOnlyList<LEntry> found)`

The ids of the stored entries a lookup found, the shape the gate and the dropdown take.

## `private static LMarkupIntake QSCustomsIntakeCreate(int index, CSCustomsRow row)`

Reshapes one row of the gate into the intake the import takes.
A New row's intake carries no target, even when the row still holds an earlier pick.

## `private static IReadOnlyList<LCardDraft> QSCustomsChildRead(LCardDraft card)`

The cards nested under a card, handed to the gate's count as its way down the tree.

## `private void QSCustomsRowApply(FrameworkElement container, object item, string? _)`

Reads the row from the gate in one call and fills it.
It runs again for every row after any change, so a new mode or loss shows at once.

## `private void QSCustomsRowApply(FrameworkElement container, QSCustomsItem row, CSCustomsRow state)`

Fills one declaration row: number, headword, language, both dropdowns and the loss text.
The loss reads the entry the gate names, and a zero id reads nothing.

## `private void QSCustomsModeApply(FrameworkElement container, ComboBox mode, CSCustomsMode chosen)`

Selects the named mode row matching the gate's mode.
The handler is off while the fill selects, so a fill never writes back into the gate.

## `private void QSCustomsTargetApply(ComboBox target, QSCustomsItem row, CSCustomsRow state)`

Hands the dropdown the candidate ids and selects the gate's target, live only while the mode wants one.
The handler is off while the fill selects, so a fill never writes back into the gate.
The closed dropdown shows the chosen candidate through the same template, filled once layout has run.

## `private static void QSCustomsCandidateApply(FrameworkElement container, object item, string headword)`

Fills a candidate with the row's headword and its id, the id muted behind a hash.

## `private static void QSCustomsOmissionApply(FrameworkElement container, object item, string? _)`

Fills an omission row with its line number and its text.

## `private void QSCustomsModeHandle(object sender, SelectionChangedEventArgs e)`

Hands the picked mode to the gate, whose answer lights or darkens Accept.
Every row is filled again, since the gate may now answer differently for it.

## `private void QSCustomsTargetHandle(object sender, SelectionChangedEventArgs e)`

Hands the picked candidate's id to the gate, whose answer lights or darkens Accept.

## `private string QSCustomsLossFormat(LEntryDraft? stored)`

The meanings and collocations a Replace would drop, counted by the gate over the stored entry.
The format is the window's localized text, filled with both counts.
A missing format falls back to its own key, so a broken catalog still shows something.
Blank when no entry was read.
