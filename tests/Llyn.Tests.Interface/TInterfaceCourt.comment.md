# TInterfaceCourt.cs
Hash: `8e00b31340846d57`

## `internal static partial class TInterface`

The relays for the draft-folder archives.
They hand the workspace folder its draft, court and claim files and read them back.
`TCourtCreate` builds one court row, for a fake port to answer.
`TClaimCreate` builds one claim.
`TCourtArchiveRead` alone picks its row from a scan.
Every other relay passes one archive call through.
