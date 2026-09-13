# TSourceHandler.cs

## `internal sealed class TSourceHandler`

A message handler that answers every request with one body and one status.
It stands in for the network so a source test runs offline.
Built from a page map instead, it answers each absolute URL with its own body.
Any other URL gets a 404.
That shape lets a follow test serve the pointing page and the page pointed at.
A failing status exercises the reader's retry and lost paths without a server.
Given a gate task, it holds every answer until that task completes.
That lets an engine test act between a fill starting and its answer landing.
It counts every request it sees, so a test can tell one fetch from a repeat.
