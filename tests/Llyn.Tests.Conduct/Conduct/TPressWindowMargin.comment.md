# TPressWindowMargin.cs
Hash: `cda21723c8181c0c`

## `public sealed class TPressWindowMargin`

Covers the four margin fields, each of which hands its own typed text to its own gate.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.

Each edge gate sets its edge from the text and takes the other three from the setting last answered.
So a typed edge changes that edge only, and the margin stays Custom.
Each edge gate raises `CSPressSettingChanged` exactly once.
The four-value gate and the presets keep their facts in `TPressWindowPrinter`.
