# LDisplay.cs

## `public sealed class LDisplay`

The reading view deportment.
It keeps the reflex fold toggle and answers whether the fetched sections are still pending.
Every other member forwards one read or one mark request to the ports, so the view never holds an engine.
The veneer still catches a refused read on its side and shows the section empty.
That is what it did over the engine.

## `private bool _lDisplayOpened;`

Whether the folded reflexes are shown, flipped by the fold toggle alone and read through its verdict.

## `public bool LDisplayFanqieCheck(long? id)`

Whether the entry's rime-book rows are still being fetched, false for no entry or when the engine refuses to say.

## `public bool LDisplayScriptCheck(long? id)`

Whether the entry's script images are still being fetched, false for no entry or when the engine refuses to say.
