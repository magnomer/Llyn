# THarvest.cs

## `public sealed class THarvest`

Covers the harvest fanning one source's readings out into recordings and narrowing them to a variety.
An untagged reading in a language with varieties becomes one recording per declared variety, all sharing the address.
Without varieties the untagged reading stays untagged, so a pack like Spanish is unchanged.
A variety the answer already tags is not fanned out again.
The returned set always holds every variety, so a later menu can be narrowed from it without a fetch.
Asking for one variety streams the tagged recording of that variety alone.
Asking for one variety streams an untagged recording as one row carrying that variety.
Asking for a variety the source did not serve still streams one recording without an address.
Narrowing a kept set keeps one row per source, an addressless one where a source served nothing fitting.
An empty variety returns the kept set itself.
A lost source yields one recording that was never reached, and the listener hears the finish once.
Two sources keep their declared order in the returned set, whatever order they answered in.
