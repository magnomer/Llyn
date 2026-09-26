# PSCustoms.cs

## `public class PSCustoms`

The customs window's behavior: the rows it declares, the answer Accept builds from them, and the omission face.
Whether Accept was pressed is remembered separately from the dialog result.
Closing the window any other way declares nothing, so a dismissed window imports nothing.
The window itself is the Veneer's markup, loaded and held in `_psCustomsSurface`.

## `private PSCustoms(Window owner, LWindow? window)`

The window deportment is null on the omission face, which reads nothing back from the workspace.
It loads the window, attaches both row fills and subscribes the three buttons.

## `private ItemsControl PSCustomsList`

The named parts are read through the window's name scope.

## `internal static IReadOnlyList<LMarkupIntake>? PSCustomsShow(Window owner, LWindow window, IReadOnlyList<LMarkupEntry> entries)`

Shows the declaration over its owner and waits for the answer.
Each entry's candidates are looked up before the window opens, so the rows arrive already defaulted.
One candidate defaults to Merge with it, none to New, and several to New with the target left to choose.
Every row is numbered by its place in the file, so a reader can tell twin rows apart.
Null when cancelled, otherwise one intake per row in file order.
A New row's intake carries no target, even when the row still holds an earlier pick.

## `internal static void PSCustomsOmissionShow(Window owner, IReadOnlyList<LMarkupOmission> omissions)`

Shows the omissions the import reported on the window's second face.
An empty list shows nothing, so a clean import ends without a further click.
The declaration's default and cancel keys are released so Close answers both.

## `private void PSCustomsRowApply(FrameworkElement container, object item, string? _)`

Fills one declaration row: number, headword, language, both dropdowns and the loss text.
It runs again on every change the row raises, so a new mode or loss shows at once.

## `private void PSCustomsModeApply(FrameworkElement container, ComboBox mode, PSCustomsItem row)`

Tags the three mode rows with the engine's mode values and selects the row's own.
The handler is subscribed after the selection, so a fill never writes back into the row.

## `private void PSCustomsTargetApply(ComboBox target, PSCustomsItem row)`

Hands the dropdown its candidates and selects the stored target, live only while the mode wants one.
The closed dropdown shows the chosen candidate through the same template, filled once layout has run.

## `private static void PSCustomsCandidateApply(FrameworkElement container, object item, string? _)`

Fills a candidate with its headword and its id, the id muted behind a hash.

## `private static void PSCustomsOmissionApply(FrameworkElement container, object item, string? _)`

Fills an omission row with its line number and its text.

## `private void PSCustomsModeHandle(object sender, SelectionChangedEventArgs e)`

Writes the picked mode back into the row before the row is re-read.

## `private void PSCustomsTargetHandle(object sender, SelectionChangedEventArgs e)`

Writes the picked candidate's id back into the row before the row is re-read.

## `private void PSCustomsRowUpdate(object sender)`

Both dropdown handlers land here: the row is read back from the dropdown's data context.
Its loss text is recomputed and the Accept button re-checked, since either change can make a row ready or unready.

## `private void PSCustomsAcceptUpdate()`

Accept stays disabled while any Merge or Replace row still points at no stored entry.
The engine would refuse such an intake, so the window refuses it first.

## `private string PSCustomsLossFormat(PSCustomsItem item)`

The loss text of one row, resolved by `LSCustoms.LSCustomsLossResolve` with the window's localized format.
A missing format falls back to its own key, so a broken catalog still shows something.
