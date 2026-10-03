# TGuildScribe.cs
Hash: `a09f0539fa550a48`

## `public sealed class TGuildScribe`

Covers the authors panel's autograph desk, how it opens and how it is left, on a real workspace.
It builds each panel through `TGuild.TGuildPrepare`.
A click asks once for unsaved work, and a kept answer stays put and records no station.
A jump from another panel opens the Author without asking.
Writing with no stored Author closes the panel.
A restored scribe mode opens the autograph only over a stored Author.
A blank name is refused through the envoy, and a named one is stored and chosen.
