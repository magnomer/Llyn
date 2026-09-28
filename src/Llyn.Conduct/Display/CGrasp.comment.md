# CGrasp.cs

## `public sealed record CGrasp(int CGraspStep, string CGraspLabel);`

The grasp stars of the chosen entry, ready to show.

**Parameters**

- `CGraspStep`: the stored step, zero when nothing is chosen or the read fails.
- `CGraspLabel`: the engine's wording of that step, empty when nothing is chosen.
