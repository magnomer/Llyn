# QLookDisplay.cs

## `internal static class QLookDisplay`

The rows of the display's card, usage and contents styles, kept apart so `QLookSheet` stays under the line ceiling.
`QLookSheet` spreads them at its end, so they read as one table with the rest.

## `internal static readonly IReadOnlyList<QLook.QLookSetter> QLookDisplayState =`

One row per dismantled trigger or template binding of the display's returned dictionaries.
The chip links light their edge on hover and dim while pressed.
A frame, byline, usage detail or contents number with empty text folds away through the `Empty` state.
A picture or video list with no items folds away through `Empty` too.
The `Base` rows copy the control's padding, background and border onto the template's surface, as template bindings did.
The contents row reads the `Chosen` cue, which the display's row fill sets for the current section.
These styles live in view dictionaries, which `PDisplay` and `QRepertoire` each register themselves.
