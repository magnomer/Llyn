# LCapsulePress.cs
Hash: `635404bd8698189a`

## `public sealed record LCapsulePress([property: JsonPropertyName("width")] double LCapsulePressWidth, [property: JsonPropertyName("height")] double LCapsulePressHeight)`

The print window's geometry between runs, which only the GUI needs.
The print setting is kept by the engine's store, so Capsule holds no print data.
A `setting` key an older file carries is ignored on read.
The stored keys are a data contract, so they stay lowercase and independent of member names.

**Parameters**

- `LCapsulePressWidth`: the preview window's width when it closed.
- `LCapsulePressHeight`: the preview window's height when it closed.
