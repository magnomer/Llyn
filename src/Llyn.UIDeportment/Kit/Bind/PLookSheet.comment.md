# PLookSheet.cs

## `internal static class PLookSheet`

The table of dismantled trigger setters that `PLook` switches.
It stands apart from `PLook`, so the handler stays small while the rows grow with each returned theme.

## `internal static readonly IReadOnlyList<PLook.PLookSetter> PLookSheetState =`

One row per dismantled trigger setter, in the order the triggers stood.
A later row wins over an earlier one on the same part and property, as a later trigger did.
A row in state `Base` holds always, so it carries every template binding, icon and loaded animation.
A style key without a dot names a control type, for the implicit styles.
A style trigger outranked a template trigger, so its rows follow the template's rows.
A style based on another needs none of that style's rows, since `PLook` applies them first.
The sound themes' rows come from `PLookSound` and the display's from `PLookDisplay`, spread at the end of the table.
The slider's repeat buttons take the slider's large-step commands as `Base` rows.
A style merged by one view rather than the application is registered by that view through `PLookStyleAttach`.
The editor dictionaries are such styles, and the corpus and imprint panels merge the popup one too.
