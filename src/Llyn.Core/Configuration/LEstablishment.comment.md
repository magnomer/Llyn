# LEstablishment.cs

## `public sealed record LEstablishment(`

What the status bar at the foot of the window prints about the open workspace.
It is plain data read from the engine in one call and shown as it came.
The bar never counts drafts or rows itself, because the engine alone holds both.

**Parameters**

- `LEstablishmentUnsaved` — How many held drafts of this engine differ from the record each was started from.
- `LEstablishmentEntry` — How many entries the workspace database stores.
- `LEstablishmentSize` — The size of the workspace database file in bytes, the main file alone.
