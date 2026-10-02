# LSchemeLoader.cs

## `internal static class LSchemeLoader`

The transcription side of the pack loader, the `transcription` list.
The schemes read their sources the way a pronunciation source is read.
The language loader calls it.

## `private const string LSchemeKey = "transcription";`

The key of the scheme list.

## `public static IReadOnlyList<LScheme> LSchemePackScan(JsonElement root, IReadOnlyList<LRespellingRule> spelling)`

The pack declares its transcription schemes under `transcription`, in the order the form shows them.
A missing or empty list turns the transcription rows off for that language.
Blank and repeated names are dropped, so the form never shows a nameless or doubled scheme.

## `private static LScheme? LSchemeRowRead(JsonElement scheme, IReadOnlyList<LRespellingRule> spelling)`

A scheme is either a bare name or an object carrying a `name` and a `sources` list.
The `sources` list has the shape of the `pronunciation` list, so a scheme is looked up the way IPA is.
A bare name carries no sources, and the row for it is typed by hand.
