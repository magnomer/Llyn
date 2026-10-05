# LManifestFile.cs
Hash: `ecb22e9a41a0543f`

## `public sealed class LManifestFile : LManifestVault`

The adapter behind the manifest port, keeping the Joplin digests as JSON under the workspace root.
It reads and writes through the keep it is built over, under the name `joplin`.
The shape is an object named `notes` mapping each Joplin note id to its digest.
The two conversions are public so a test can run them on text.

## `public LManifestFile(LKeep keep)`

Binds the keep the manifest is read from and written to.

## `public LManifest LManifestRead()`

A missing file reads as an empty manifest, and so does broken JSON or a wrong shape.
A file that exists but cannot be read raises `LVaultFault` around the file system's own exception.
Reading it as empty would let a later save drop the ids of deleted notes.
Their Joplin notes would then never be trashed.

## `public void LManifestSave(LManifest manifest)`

Replaces the whole manifest kept under the name.
A keep that cannot be written raises `LVaultFault` around the file system's own exception.

## `public static LManifest LManifestFileParse(string? text)`

An entry whose digest is not a string is skipped, so one bad entry costs one extra push only.
Text that is blank or not JSON reads as empty.

## `public static string LManifestFileFormat(LManifest manifest)`

Note ids are written in a fixed order, so an unchanged manifest writes the same bytes.
