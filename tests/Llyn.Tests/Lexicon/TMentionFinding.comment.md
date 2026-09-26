# TMentionFinding.cs

## `public sealed class TMentionFinding`

Covers the engine's answer for a clicked word.
A stored Mention wins over every candidate, even one standing for nothing.
Two homograph Entries of the language both come back, in headword order, and another language's Entry does not.
Three homographs come back the same way, which is the order the word menu lists them in.
An inflected form with no headword answers its span and no candidate.
Japanese text picks the longest headword covering the click over the letter run.
It falls back to the run where nothing matches.
