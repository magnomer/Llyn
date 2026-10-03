# TListenerStub.cs
Hash: `cd67f626a506e3b1`

## `internal sealed class TListenerStub : LListener`

A listener that records the sources started, the recordings added, and how often the discovery finished.
The lists are locked because the harvest reports from several sources at once.
