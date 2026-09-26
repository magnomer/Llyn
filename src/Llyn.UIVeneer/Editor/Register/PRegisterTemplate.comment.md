# PRegisterTemplate.xaml

## `ResourceDictionary`

The Register field of a card being written, as markup alone.
The Deportment class of the same name loads this markup and forwards its events to the editor.
The editor's card fill picks the chip or the entry for each item and fills the named parts.

## `Theme.Register.Chip`

Shows a committed register and the button that removes it.
The wording reads the unknown mark when the store could not read it back.
The close icon is set by the fill, since an icon is drawn by code.

## `Theme.Register.Entry`

The caret at the end of the run, where a new register is typed.
Its keys and its leaving are subscribed by the fill.

## `Theme.Register.Field`

Arranges the committed register chips with the entry after them.

## `PRegisterFrame`

The surface is the field, not the entry.
A click on empty space inside it reaches the caret through the handler the card fill subscribes.
