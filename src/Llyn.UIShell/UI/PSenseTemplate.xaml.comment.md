# PSenseTemplate.xaml.cs

## `public partial class PSenseTemplate : ResourceDictionary`

The sense card as a template. A template lives in a dictionary rather than in the panel it fills, so the panel keeps its own layout; the events a card raises belong to the panel all the same, and this class exists only to hand them back to it.
