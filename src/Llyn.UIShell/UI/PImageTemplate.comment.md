# PImageTemplate.xaml.cs

## `public partial class PImageTemplate : ResourceDictionary`

The picture row of a card as a template, together with the rules that hide an empty picture section and a preview there is nothing to draw. A template lives in a dictionary rather than in the panel it fills; the events a row raises belong to the editor all the same, and this class exists only to hand them back to it.
