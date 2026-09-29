# PWindowNotice.cs

## `public partial class PWindow`

What the window says to the user in its own voice.
It is the message box the shell puts up when a request failed.

## `internal void PWindowFailureRefine(string key, Exception exception)`

Shows a failure under the localized headline `key` names, with the detail Conduct chose.
`CLedgerNoticeRead` decides the detail: a refusal's reason, or the unexpected line and the audit file.
A fault's own message is written for whoever fixes the program, so the user never sees it.
The window's envoy words the keys and places the lines, as it does for a gate's failure.
The catch sites that call it still own their failure, until each becomes a gate reporting through `CEnvoy`.
