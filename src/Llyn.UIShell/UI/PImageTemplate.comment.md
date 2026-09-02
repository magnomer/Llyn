# PImageTemplate.xaml.cs

## `public partial class PImageTemplate : ResourceDictionary`

The picture row of a card as a template.
It comes with the rules that hide an empty picture section.
Those rules also hide a preview there is nothing to draw.
A template lives in a dictionary rather than in the panel it fills.
The events a row raises belong to the editor all the same.
This class exists only to hand them back to it.
