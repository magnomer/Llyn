# LCollocationLayout.cs
Hash: `be1752976a8509f9`

## `public sealed record LCollocationLayout(IReadOnlyDictionary<long, IReadOnlyList<LCardDraft>> LCollocationLayoutHome, IReadOnlyDictionary<long, IReadOnlyList<(long LCollocationId, LStateValue LCollocationPhrase, string LMeaningPath)>> LCollocationLayoutStub, IReadOnlyList<LCardDraft> LCollocationLayoutOther, IReadOnlyDictionary<long, LStateValue> LCollocationLayoutPhrase, IReadOnlyDictionary<long, IReadOnlyList<string>> LCollocationLayoutFlag, IReadOnlyDictionary<long, int> LCollocationLayoutExample, IReadOnlyDictionary<long, IReadOnlyList<string>> LCollocationLayoutAlso)`

Where the reading view places each collocation of one entry draft.
It carries values only.
The home, stub and other rules stay on `LCollocation`, and an Application workflow applies them.
The home and stub keys are top-level meaning ids, because the reading view shows collocations under top-level meanings only.
The phrase and marker keys are collocation ids, one for every collocation of the draft.

**Parameters**

- `LCollocationLayoutHome`: the collocations shown in full under each top-level meaning, in draft order.
  A meaning with no homed collocation has no key.
- `LCollocationLayoutStub`: the stub lines under each top-level meaning a collocation links to besides its home.
  Each line names the collocation's id, its phrase and the path of its nested home.
  The id lets a click open the collocation's row.
- `LCollocationLayoutOther`: the collocations linked to no meaning of the draft, in draft order.
- `LCollocationLayoutPhrase`: the phrase each collocation's row is headed with.
- `LCollocationLayoutFlag`: the translation languages each closed row shows a flag for.
- `LCollocationLayoutExample`: how many filled examples each collocation holds.
- `LCollocationLayoutAlso`: the nested paths of each collocation's further linked meanings, empty when there are none.
