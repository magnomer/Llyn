# PImageTemplate.xaml

## `ResourceDictionary`

The picture row of a card being written, as markup alone.
The Deportment class of the same name loads this markup and forwards the two clicks to its host.
The entry editor and the situation editor both merge it, and each fills the named parts.

## `<Style x:Key="Theme.Image.Preview" TargetType="Border">`

The frame around a picture preview.
The fill hides it while there is no picture to draw.

## `<DataTemplate x:Key="Theme.Image.Row">`

The preview, the location field with its browse button, and the remove handle in the gutter.
The preview sits in the lazy loader, so a picture far below the view is not decoded yet.
The handle panel starts hidden and shows while the pointer or the focus is inside the list.
Typing in the location leaves through whichever editor's text handler hears it bubble.
