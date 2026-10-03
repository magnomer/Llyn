# TGuildMode.cs
Hash: `291531cac0f16940`

## `public sealed class TGuildMode`

Covers how the authors panel carries its side and mode across clicks, on a real workspace.
It builds each panel through `TGuild.TGuildPrepare`.
A click on no row changes nothing on either list.
A click while writing keeps the autograph over a stored Author and drops it for the orphan row.
A stored leave answer saves the draft before the clicked Author opens.
The mode toggle and the delete do nothing while a Source is in front.
Turning back to reading drops the held draft and shows the vita.
