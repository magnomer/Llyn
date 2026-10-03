# TReceiverStub.cs
Hash: `5dcf9eb20ede4012`

## `internal sealed class TReceiverStub : LReceiver`

A receiver that records the sources started, the candidates added, and how often the lookup finished.
The lists are locked because the lookup reports from several sources at once.
