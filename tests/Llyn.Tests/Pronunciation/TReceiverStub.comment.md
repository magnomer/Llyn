# TReceiverStub.cs

## `internal sealed class TReceiverStub`

A receiver that records the sources started, the candidates added, and how often the lookup finished.
The lists are locked because the lookup reports from several sources at once.
