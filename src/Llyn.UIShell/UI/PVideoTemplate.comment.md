# PVideoTemplate.xaml.cs

## `public partial class PVideoTemplate : ResourceDictionary`

The video row of a card as a template.
It has its location field, its timestamp field, and the player beneath them.
It hands every event the row raises back to the editor that owns them.
Those are the file chooser and the drop.
They also include the four the player raises as it is built, opened, played and taken down.
