# QSCustoms.cs
Hash: `625c39ff992690e9`

## `internal sealed class QSCustoms`

The customs window's showing holds the rows it declares and the omission face.
Its two static entry points answer `CEnvoy` questions, so it only shows what Conduct handed it.
Each row's mode and target live in the `CSCustoms` gate Conduct hands over.
The window calls no other gate.
The window is the veneer's `PSCustoms` page, pulled fresh by contract ID and held in `_qsCustomsSurface`.

## `private static readonly IReadOnlyDictionary<string, CSCustomsMode> QSCustomsChoice =`

Maps each named mode row of the dropdown to the gate's mode.
The surface is handed no mode value, so the pick is read back by the row's contract ID.

## `private QSCustoms(Window owner, CSCustoms customs)`

Pulls the window over `owner`, lists the gate's entries and subscribes Accept and Cancel.
Each row carries its entry's ready target ids, so the window finds nothing itself.
Accept starts lit only when the gate finds every row ready.
A dialog is its own window, outside the main window's look reach.
So it attaches the look to itself, or its buttons show no label and no fill.

## `private ItemsControl QSCustomsList`

Each named part is pulled from the window by its contract ID.

## `internal static bool QSCustomsConsult(Window owner, CSCustoms customs)`

Shows the declaration and answers whether the user accepted it.
The declared rows stay in the gate, so Conduct reads them back itself.

## `internal static void QSCustomsOmissionConsult(Window owner, IReadOnlyList<CMarkupOmission> omissions)`

Shows the omissions the import reported on the window's second face.
Each omission arrives as ready line and text, and Conduct asks only when one exists.
The window hands its list a line and text pair of its own, so no Conduct record reaches the surface.
The declaration's default and cancel keys are released so Close answers both.
It attaches the look to its own window, like the constructor.

## `private void QSCustomsRowRefine(FrameworkElement container, object item, string? _)`

Reads the row from the gate in one call and fills it.
It runs again for every row after any change, so a new mode or loss shows at once.

## `private void QSCustomsRowRefine(FrameworkElement container, QSCustomsItem row, CSCustomsRow state)`

Fills one declaration row.
It sets the number, headword, language, both dropdowns and the loss text.
The loss looks up the wording key the gate chose and fills it with the ready card counts.
A row with no loss key shows no loss.

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

Fills an omission row with its line and its text.

## `private void QSCustomsModeObserve(object sender, SelectionChangedEventArgs e)`

Hands the picked mode to the gate and its verdict to the window's refresh.

## `private void QSCustomsTargetObserve(object sender, SelectionChangedEventArgs e)`

Hands the picked candidate's id to the gate and its verdict to the window's refresh.

## `private void QSCustomsRefine(bool ready)`

Lights or darkens Accept by the gate's verdict.
Every row is filled again, since the gate may now answer differently for it.

## `private void QSCustomsAcceptObserve(object sender, RoutedEventArgs e)`

Closes the declaration as accepted, which answers the customs question.

## `private void QSCustomsCancelObserve(object sender, RoutedEventArgs e)`

Closes the declaration as declined.

## `private static void QSCustomsCloseObserve(object sender, RoutedEventArgs e)`

Closes the omission face, whose window is found from the pressed button.
