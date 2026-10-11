# LFreedomClerk.cs
Hash: `6ec52b4723d6fc17`

## `public static class LFreedomClerk`

The freedoms an entry is offered and the key that names each.
Every language and every unit offers the same freedoms, because the user alone decides which one fits.

## `private static readonly LFreedom[] LFreedomClerkOffered`

Dependent and Standalone, in the order the dropdown shows them.

## `public static IReadOnlyList<LFreedom> LFreedomScan()`

The freedoms offered to every entry, in the order the dropdown shows them.

## `public static string LFreedomFormat(LFreedom freedom)`

The localization key that names a freedom, read from `LFreedomKey` for the engine.
The engine names no Core rule itself, so it reaches the key through here.
