# CLookupStep.cs

## `public sealed record CLookupStep(`

One step of a reading search, as the notation receives it on the driver's thread.
It carries no kind, since a found reading or the end flag already tells the step apart.

**Parameters**

- `CLookupStepSource`: the source being asked, for a step that starts one.
- `CLookupStepOrder`: that source's place in the pack.
- `CLookupStepCandidate`: the reading found, or null for any other step.
- `CLookupStepEnded`: whether the search has finished.
