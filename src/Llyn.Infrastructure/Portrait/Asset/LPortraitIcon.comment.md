# LPortraitIcon.cs
Hash: `a1df0e263a968016`

## `public static class LPortraitIcon`

The icons an exported page draws, read out of the assembly rather than off disk.
An export then embeds them even when no icon folder stands beside the program.

## `private const string LPortraitIconPrefix`

The logical-name prefix `Llyn.Infrastructure.csproj` gives each embedded icon.

## `public static LPortraitAsset LPortraitIconRead(string name)`

The embedded icon `name` names, without its `.svg` suffix, ready to embed as a data address.
The icons are fixed at build, so an unknown name is a caller mistake and throws.
