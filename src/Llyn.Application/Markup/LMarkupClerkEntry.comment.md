# LMarkupClerkEntry.cs
Hash: `b93c6750ca5d4671`

## `public sealed class LMarkupClerkEntry`

One entry translated into markup, with its inflections, etymology and etymons.
Cards go through the card clerk and etymology mentions through the example clerk.

## `public LMarkupClerkEntry(LRig rig, LReflexClerk reflexes, LMarkupClerkCard card, LMarkupClerkExample example)`

Reads the entry, speech and morphology ports out of `rig`.
`reflexes` is the engine's reflex clerk, kept so an export lists reflex rows in the pack's declared order.
`card` and `example` are the clerks the cards and the etymology mentions are translated through.

## `public LMarkupEntry? LMarkupLoad(long id)`

Loads the entry draft for `id` and translates every id-bearing field into names.
Null means no entry has that id.
Draft ids and positions are dropped, because list order carries the positions.
Reflex rows are written in the order the pack declares, through `LReflexClerkSort`, never in storage order.
The lexical unit is carried as the draft holds it.
Grasp, frequency, favorite and timestamps are never read.
Forms pass whole, since they hold no id worth hiding.
A reflex also drops its anatomy and anchors, since the receiving engine derives the one and owns the other.

## `private LMarkupEtymology? LMarkupEtymologyCreate(LEtymologyDraft etymology)`

The narrative etymology with each span's target named by headword and language.
Nothing when the etymology is not narrated, since only one shape travels.
A span whose target no longer stands is left out, since an etymology span always names an entry.

## `private IReadOnlyList<LMarkupEtymon> LMarkupEtymonCreate(LEtymologyDraft etymology)`

The direct source links named by headword and language, in the order the draft holds them.
A link whose target no longer stands is left out.

## `private LMarkupInflection LMarkupInflectionCreate(LInflection inflection)`

Replaces the speech value id and morphology ids with their stored names.
A morphology that no longer exists is skipped rather than written as a number.
