# LTenor.cs

## `public sealed class LTenor`

The tenor panel's deportment: the register vista and the cohort vista it holds, and what the panel asks of them.
The register side finds rows, takes the query, order and kind filter, and creates a Register.
The cohort side finds the entries of the chosen Register, loads and deletes the chosen one, and marks it edited.
Both vistas are handed in by the window, which restores every tab's vistas together.

## `public LEditor LTenorEditor { get; }`

The entry editor's deportment on the cohort side, which takes the cohort vista when the panel's vistas are restored.

## `public LPanel LTenorPanel { get; }`

The shared panel state over the cohort vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public void LTenorEntryCreate()`

Opens a blank entry in the editor.
When a Register is chosen, the new entry starts inside that Register.

## `public LVista? LTenorCohortVista => _lTenorCohort;`

The cohort vista, exposed for the window's export dialog alone, which names its file after the vista.

## `public IReadOnlyList<LVistaRow> LTenorCohortRead()`

The entries under the chosen Register, or every entry while none is chosen, as the engine narrows them.
