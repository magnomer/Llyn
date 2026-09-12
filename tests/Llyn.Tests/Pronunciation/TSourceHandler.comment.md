# TSourceHandler.cs

## `internal sealed class TSourceHandler`

A message handler that answers every request with one body and one status.
It stands in for the network so a source test runs offline.
A failing status exercises the reader's retry and lost paths without a server.
