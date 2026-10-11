# LPressFile.cs
Hash: `d65a8925b22bf89c`

## `public sealed class LPressFile : LPressVault`

The adapter behind the print setting port, keeping the setting as JSON under the workspace root.
It reads and writes through the keep it is built over, so the file is `press.json` beside the posture file.
The JSON shape is this adapter's alone, and Core holds no JSON.
The keys are the record's member names, so renaming a member drops that choice from a kept file.
The two conversions are public so a test can run them on text.

## `private const string LPressFileName = "press"`

The keep name, which the keep turns into the file name.

## `public LPressFile(LKeep keep)`

Binds the keep the setting is read from and written to.

## `public LPressSetting? LPressVaultRead()`

The kept setting, or nothing while the keep holds no such document.
A keep that cannot be read raises `LVaultFault` around the file system's own exception.

## `public void LPressVaultSave(LPressSetting setting)`

Replaces the kept setting whole.
A keep that cannot be written raises `LVaultFault` around the file system's own exception.

## `public static LPressSetting? LPressFileParse(string? text)`

Answers the setting the text describes, or null when the text is blank or malformed.
It never throws, so a damaged file only loses the remembered choices.
The result is not yet valid, so the caller clamps it against a real printer.

## `public static string LPressFileFormat(LPressSetting setting)`

Answers the JSON text of the setting exactly as given.
The caller empties the page range first, since pages belong to one document.
