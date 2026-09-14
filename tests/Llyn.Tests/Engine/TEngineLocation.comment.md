# TEngineLocation.cs

## `public sealed class TEngineLocation`

Covers the one rule for where a stored location may point.
A path naming a host or a device is refused, whatever spelling it takes.
A relative path resolves inside the workspace and is refused once it walks out.
A drive path, a hostless `file` address and a web address pass as written.

## Inline notes

### `public void LocationResolve_HostOrDevicePath_ReturnsNull(string location)`

Each row is a spelling Windows would resolve through a network or a device handle.
The lone leading separator is a root of the current drive, which the workspace rule cannot vouch for.
