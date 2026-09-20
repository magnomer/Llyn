# LTenor.cs

## `public sealed class LTenor`

The tenor panel's deportment: the register vista and the cohort vista it holds, and what the panel asks of them.
The register side finds rows, takes the query, order and kind filter, and creates a Register.
The cohort side finds the entries of the chosen Register, loads and deletes the chosen one, and marks it edited.
Both vistas are handed in by the window, which restores every tab's vistas together.
The panel's mode, its bin and its scribe toggle stay in the veneer until the tabs share one panel deportment.

## `public LEditor LTenorEditor { get; }`

The entry editor's deportment on the cohort side, which takes the cohort vista when the panel's vistas are restored.

## `public LVista? LTenorCohortVista => _lTenorCohort;`

The cohort vista, exposed for the window's export dialog alone, which names its file after the vista.

## `public IReadOnlyList<LVistaRow> LTenorCohortRead()`

The entries under the chosen Register, or every entry while none is chosen, as the engine narrows them.
