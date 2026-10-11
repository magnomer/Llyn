# LFreedomKey.cs
Hash: `8e12d1a272990917`

## `public static class LFreedomKey`

The text key of each freedom, written in markup, and its localization key.
One map keeps a freedom written by an export readable by the import.

## `public static LFreedom LFreedomKeyParse(string? key)`

Reads a key regardless of case and surrounding space.
An unknown or missing key reads as `LFreedomEmpty`, so a foreign file never fails on it.

## `public static string LFreedomKeyRead(LFreedom freedom)`

The localization key that names a freedom, empty for `LFreedomEmpty`.
The editor, the reading view and the exports all name a freedom through it, so they never disagree.

## `public static string LFreedomKeyFormat(LFreedom freedom)`

Writes the key of a freedom, empty for `LFreedomEmpty`.
