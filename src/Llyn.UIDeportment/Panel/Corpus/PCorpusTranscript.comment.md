# PCorpusTranscript.cs

## `public class PCorpusTranscript : ResourceDictionary`

The transcript dictionary as a class: its markup lives in the Veneer, and this class merges it into itself.
It holds the Corpus panel it was built for and forwards every handler to it unchanged.
The templates name no handler, so the panel's fills subscribe these forwarders on each realized part.
The forwarders are internal for that one caller, and the panel's own handlers stay the panel's.
Every later template dictionary that carries code follows this shape.

## `internal PCorpusTranscript(PCorpus host)`

Keeps the host and merges the loaded Veneer dictionary, which stands where `InitializeComponent` stood.
