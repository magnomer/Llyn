# PThemeMention.xaml

## `<Style x:Key="Theme.Mention.Linked" TargetType="Run">`

A word that stands for an Entry is underlined in the accent colour.
A reader tells at a glance which words of a sentence lead somewhere.
The cursor turns to a hand over it, so the run itself answers the pointer and no code searches pieces.
Anywhere else the cursor is left to whoever answers next, which in the reading view is the band's beam.
An unlinked word may open an Entry too, but only the engine knows that, and the cursor does not ask.

## `<Style x:Key="Theme.Mention.Silent" TargetType="Run">`

A word marked as standing for nothing is drawn plain.
It is still a piece of its own, so a click on it is answered without asking the engine.

## `<ContextMenu x:Key="Theme.Mention.Menu">`

The menu the linking gesture is asked through, built once and attached to every sentence field by key.
Link, choose, silence and unlink each carry one of the four Mention commands.
Each command is a `PLook` row on the named item.
The command routes from the field that holds the focus, so it reaches the field's host.
Which items are enabled follows the selection, answered by the host's command checks when the menu opens.
Cut, copy and paste stand below a separator, because a custom menu replaces the field's own.

## `<Style x:Key="Theme.Mention.Line" TargetType="ItemsControl">`

The chip line under a sentence field, wrapped.
A `PLook` row collapses it while it holds nothing.
A row without Mentions is as tall as it was before the line existed.

## `<Style x:Key="Theme.Mention.Sense" TargetType="TextBlock">`

The Meaning title on a chip.
A `PLook` row hides it while empty, which is when the Mention stands for the whole Entry.

## `<DataTemplate x:Key="Theme.Mention.Chip">`

One Mention: the word, an arrow, the headword it stands for, and the Meaning when narrowed.
A word standing for nothing shows the silent mark in place of a headword.
The remove button asks the unlink command with the chip as its parameter.
Deportment fills the named parts, the command and the icon through `PLookItemAttach`.
The chip is drawn as a badge rather than a pellet.
It reports a link and is not the link itself.
