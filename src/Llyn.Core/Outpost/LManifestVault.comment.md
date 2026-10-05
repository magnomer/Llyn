# LManifestVault.cs
Hash: `7c0d122f72ebe66b`

## `public interface LManifestVault`

The port that keeps the push manifest under the workspace root.
The manifest travels with the workspace, so a moved workspace remembers what it pushed.

## `LManifest LManifestRead();`

Returns the stored manifest.

## `void LManifestSave(LManifest manifest);`

Replaces the stored manifest with `manifest` whole.
