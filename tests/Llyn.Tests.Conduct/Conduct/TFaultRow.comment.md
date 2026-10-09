# TFaultRow.cs
Hash: `94662f8cc5a1e773`

## `internal sealed record TFaultRow(string TFaultRowGate, string TFaultRowMember, string TFaultRowKey, Func<TFaultStage, Task<Func<Task>>> TFaultRowArrange)`

One gate of the task sweep, with the fault it meets and the notice it must show.

**Parameters**

- `TFaultRowGate`: the gate's name as `Type.Method`.
- `TFaultRowMember`: the faulted member as `Interface.Member`.
- `TFaultRowKey`: the notice key the envoy must hear once.
- `TFaultRowArrange`: builds the gate's owner, meets its preconditions, and answers the gate call.
