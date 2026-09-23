# PSCustoms.xaml.cs

## `public partial class PSCustoms : Window`

The customs window's behavior: the rows it declares, the answer Accept builds from them, and the omission face.
Whether Accept was pressed is remembered separately from the dialog result.
Closing the window any other way declares nothing, so a dismissed window imports nothing.

## `private PSCustoms(Window owner, LWindow? window)`

The window deportment is null on the omission face, which reads nothing back from the workspace.

## `internal static IReadOnlyList<LMarkupIntake>? PSCustomsShow(Window owner, LWindow window, IReadOnlyList<LMarkupEntry> entries)`

Shows the declaration over its owner and waits for the answer.
Each entry's candidates are looked up before the window opens, so the rows arrive already defaulted.
One candidate defaults to Merge with it, none to New, and several to New with the target left to choose.
Headwords shared by two rows are numbered, so a reader can tell twin rows apart.
Null when cancelled, otherwise one intake per row in file order.

## `internal static void PSCustomsOmissionShow(Window owner, IReadOnlyList<LMarkupOmission> omissions)`

Shows the omissions the import reported on the window's second face.
An empty list shows nothing, so a clean import ends without a further click.
The declaration's default and cancel keys are released so Close answers both.

## `private void PSCustomsRowUpdate(object sender)`

Both dropdown handlers land here: the row is read back from the dropdown's data context.
Its loss text is recomputed and the Accept button re-checked, since either change can make a row ready or unready.

## `private void PSCustomsAcceptUpdate()`

Accept stays disabled while any Merge or Replace row still points at no stored entry.
The engine would refuse such an intake, so the window refuses it first.

## `private string PSCustomsLossFormat(PSCustomsItem item)`

The meanings and collocations a Replace would drop, counted from the stored entry.
Nested meanings are counted too, since every one of them goes.
Blank for any other mode, and blank when the target cannot be read.
