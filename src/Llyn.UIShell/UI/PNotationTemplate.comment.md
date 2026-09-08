# PNotationTemplate.xaml

## `<DataTemplate x:Key="Theme.Notation.Row">`

One source: its name, small, then what it had to say.
A reading is shown in brackets beside the button that takes it.
A source with no reading shows one italic line in place of both.
The line says whether it is still searching, has no entry, or could not be retrieved.
Hiding the taking button rather than disabling it keeps a row that cannot act apart.
It never looks like one that is merely busy.
The hover highlight is conditioned on the row being takeable, so an unreachable source never lights up as a choice.
