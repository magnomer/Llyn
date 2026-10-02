# TRefusalHandler.cs
Hash: `650e47d07ed9c632`

## `internal sealed class TRefusalHandler`

A message handler that answers the first few requests with 429 and every later one with audio bytes.
Each refusal names a one-second Retry-After, the value Wikimedia's audio host sends.
It counts the requests it saw, so a test can read how many tries a fetch took.
It stands in for a host throttling bursts, so the workspace's retry runs offline.
