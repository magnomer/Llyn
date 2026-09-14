# LRegisterLoader.cs

## `public static class LRegisterLoader`

Reads the Registers a language pack ships from `languages/<Name>/vocabulary.json`.
The pack is display vocabulary, so its ids are stable and its names are what the shell shows.
A missing file, a missing `registers` section, and an unknown file all mean the same thing here.
That thing is a language shipping no Registers, never a failure the caller has to handle.

## `public static IReadOnlyList<LRegister> LRegisterLoaderLoad(string language)`

Reads one language's Registers, each carrying the language it came from and the integer id the pack declared.
A row whose id is not a positive integer, or whose name is missing, is skipped.
The store keys a shipped row by language and pack id, so two packs never collide on the same number.
A name `LLanguageNameValidate` refuses reads nothing.
