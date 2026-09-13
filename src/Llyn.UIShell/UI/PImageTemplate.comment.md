# PImageTemplate.xaml.cs

## `public partial class PImageTemplate : ResourceDictionary`

The picture row of a card as a template.
It comes with the rules that hide an empty picture section.
Those rules also hide a preview there is nothing to draw.
A template lives in a dictionary rather than in the panel it fills.
The events a row raises belong to whoever drew the row all the same.
This class exists only to hand them back through `PImageHost`.
The entry editor and the situation editor both implement that seam, so one template draws a picture row in either.
