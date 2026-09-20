# LRegisterLoader.cs

## `public static class LRegisterLoader`

Reads the Registers a language pack ships from `languages/<Name>/vocabulary.json`.
The pack is display vocabulary, so its names are what the shell shows.
A pack names its Registers and never numbers them, because the name is the identity.
A missing file, a missing `registers` section, and an unknown file all mean the same thing here.
That thing is a language shipping no Registers, never a failure the caller has to handle.

## `public static IReadOnlyList<LRegister> LRegisterLoaderLoad(string language)`

Reads one language's Registers as built in rows carrying no language.
An entry that is not a non-empty string, or repeats a name already read, is skipped.
The store keys a row by name, so two packs naming the same name share one Register.
A name `LLanguageNameValidate` refuses reads nothing.

## `private static IReadOnlyList<LRegister> LRegisterPackRead(JsonElement root)`

Reads the `registers` array of one parsed pack.

## `private static string? LRegisterTextRead(JsonElement element)`

The trimmed name one array entry gives, or `null` when the entry is not a string with text.
