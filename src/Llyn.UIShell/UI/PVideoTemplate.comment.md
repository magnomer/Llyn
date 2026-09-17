# PVideoTemplate.xaml.cs

## `public partial class PVideoTemplate : ResourceDictionary`

The video row of a card as a template.
It has its location field, its timestamp field, and the Screen above them.
Each binds the row's value one way through the state converter, and typing leaves through the editor's handler.
It hands the two events the row raises back through `PVideoHost` to whichever editor drew the row.
Those are the file chooser and the drop.
The Screen answers for playing itself, so nothing about playing passes through here.
