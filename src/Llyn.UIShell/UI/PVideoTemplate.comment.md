# PVideoTemplate.xaml.cs

## `public partial class PVideoTemplate : ResourceDictionary`

The video row of a card as a template, with its location field, its timestamp field, and the player beneath them. It hands every event the row raises — the file chooser, the drop, and the four the player raises as it is built, opened, played and taken down — back to the editor that owns them.
