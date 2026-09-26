# PVideoTemplate.xaml

## `ResourceDictionary`

The video row of a card being written, as markup alone.
The Deportment class of the same name loads this markup and forwards the two clicks to its host.
The entry editor and the situation editor both merge it, and each fills the named parts.

## `<Style x:Key="Theme.Video.Preview" TargetType="Border">`

The frame around the Screen.
The fill hides it while the location names nothing to play.

## `<DataTemplate x:Key="Theme.Video.Row">`

The Screen, the location and timestamp fields with a browse button, and the remove handle in the gutter.
The two fields are named after the values they show, so a text handler knows which one it heard.
The handle panel starts hidden and shows while the pointer or the focus is inside the list.
