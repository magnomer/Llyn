# PClipTemplate.xaml

## `<DataTemplate x:Key="Theme.Clip.Row">`

One source, its name set small in the fixed column the pronunciation menu gives it.
The two menus line up.
A recording itself has nothing to read, so the row says nothing about it — it is played.
The taking button carries this row's own download state, so a later arrival cannot overwrite it.
A source that offered nothing shows its one line in place of both buttons.
Hiding them rather than disabling them keeps a row that cannot act from looking like one that is merely busy.
The hover highlight is conditioned on the row being takeable and idle.
Neither an unreachable source nor a running download lights up as a choice.
