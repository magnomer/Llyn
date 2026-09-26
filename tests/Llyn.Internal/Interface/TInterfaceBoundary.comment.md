# TInterfaceBoundary.cs

## `public sealed class TInterfaceBoundary`

Guards the one rule the suite is built on.
A test outside `Interface` may not call or construct production code itself.

## `private static readonly Regex TInterfaceDirectCall`

Matches a call on a production name, whether it is static or made on an instance.

## `private static readonly string[] TInterfaceProjects`

Names the two behaviour projects under `tests`, which share one relay layer.
The Windows project reaches the relays through the internal project, so one guard covers both.

## `private static readonly Regex TInterfaceDirectConstruct`

Matches a production type constructed with `new`.

## `public void InterfaceBoundary_ProductionCalls_GoThroughInterface()`

Scans every test source outside an `Interface` folder in both projects.
It reports each file that reaches production directly.
The failure names the offending files so the missing relay is obvious.

## `private static string TInterfaceRootRead()`

Walks up from the test binaries to the tests folder, since the scan reads sources rather than types.
The folder counts only when it holds both projects.
