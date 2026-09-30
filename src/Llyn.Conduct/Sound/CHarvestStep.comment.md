# CHarvestStep.cs

## `public sealed record CHarvestStep(`

One step of a recording search, as the errand hands it to the driver's thread.
It carries no kind, since a found recording or the end flag already tells the step apart.

**Parameters**

- `CHarvestStepSource`: the source being asked, for a step that starts one.
- `CHarvestStepOrder`: that source's place in the pack.
- `CHarvestStepRecording`: the recording found, or null for any other step.
- `CHarvestStepEnded`: whether the search has finished.
