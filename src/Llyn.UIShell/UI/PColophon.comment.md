# PColophon.xaml

## `<UserControl.Resources>`

The title, heading, value and chip styles of the read area.
The chips are built on the speech chip, so the control merges the speech theme to reach it.
They live here rather than in a panel, because two panels read a Source through this control.

## `<StackPanel x:Name="PColophonBody" ...>`

The reading of one Source, laid out as the repertoire panel lays out a Situation.
The title stands at the head of the page.
The kind and the citation count stand as chips on the row beneath it.
The authors, the year, the address and the note are headings over their values, as a situation's description is.
No field is labeled, because the entry display labels nothing: position and dress say what a value is.
A never-written field is not drawn at all, as a situation with no description draws none.
An unknown value reads the unknown mark where the value would stand.
An unknown authorship reads it under its heading.
A never-written title reads the untitled text in the muted colour, because the head of the page cannot be empty.
The editing side still draws every slot, so the two kinds of empty are one toggle apart rather than lost.

## `<TextBlock x:Name="PColophonUnselected" ...>`

The prompt shown while no Source is chosen, centred where the reading would stand.
