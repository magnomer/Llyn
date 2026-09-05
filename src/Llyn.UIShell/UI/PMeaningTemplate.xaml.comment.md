# PMeaningTemplate.xaml.cs

## `public partial class PMeaningTemplate : ResourceDictionary`

The meaning card as a template.
A template lives in a dictionary rather than in the panel it fills.
So the panel keeps its own layout.
The events a card raises belong to the panel all the same.
This class exists only to hand them back to it.
