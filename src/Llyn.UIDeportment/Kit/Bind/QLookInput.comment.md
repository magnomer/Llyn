# QLookInput.cs
Hash: `09eda193c2f517a7`

## `internal static class QLookInput`

The rows of the input styles and the search bar, kept apart so `QLookSheet` stays under the line ceiling.
`QLookSheet` spreads them after its own rows, so they read as one table with the rest.

## `internal static readonly IReadOnlyList<QLook.QLookSetter> QLookInputState`

One row per dismantled trigger or template binding of the `Theme.Input` styles and the `Theme.Search` styles.
The `Base` rows copy the control's background, border, padding and content onto the template's parts, as template bindings did.
The choice box binds its button and popup to the drop-down state, and its popup takes the box's width.
Hover, press, checked and disabled rows recolour the surface by veneer resource key, so a theme switch reaches them.
The bin buttons take the warning colours, and a disabled bin drops to the disabled line and ink.
The search helper hides its mark when bare and its label when muted, taking the muted insets by key.
