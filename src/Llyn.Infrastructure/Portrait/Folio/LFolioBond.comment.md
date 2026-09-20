# LFolioBond.cs

## `public static class LFolioBond`

Builds the two parts that tie a Word package together.

## `public static string LFolioBondRead(IReadOnlyList<LPortraitAsset> plates)`

The document's own relationships, naming the styles and every embedded picture.
Picture ids start high so they never collide with the fixed ones.

## `public static string LFolioBondCreate(IReadOnlyList<LPortraitAsset> plates)`

The content types, which must declare every extension the package actually holds.
Word refuses to open a package whose picture extension is undeclared.
