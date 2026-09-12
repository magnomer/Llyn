# TPronunciationHelper.cs

## `internal static class TPronunciationHelper`

The fixtures a pronunciation lookup test builds from.
A client over a stub handler stands in for the network.
A reading and an attempt are built with the fields a test names and defaults for the rest.
A source stub answers with fixed readings and a receiver stub records what it was told.

## `internal const string TPronunciationHelperUrl`

The one address every stub attempt fetches.
The handler ignores it, so a test never depends on the word being escaped.
