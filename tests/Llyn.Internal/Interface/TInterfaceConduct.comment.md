# TInterfaceConduct.cs

## `internal static class TInterfaceConduct`

The relays for the conduct rules the reading view and the dialogs read.
It is a class of its own rather than a part of `TInterface`.
Each relay reaches a static rule or builds a conduct over outlets, so none builds a WPF object.
Each relay is transparent and carries no test logic of its own.

## `internal static CAtelier TAtelierCreate(LEngine engine, LMediaPort media) => new(`

Builds Conduct's atelier over outlets on `engine` as Host does, with `media` standing in for the player.
The draft port is a fake that sweeps nothing, since the fake rig holds no drafts.

## `internal static CAtelier TAtelierCreate(LEngine engine, Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an atelier whose draft, entry and phonology ports answer from `answers`, for the text gates.
The settings and portrait ports are outlets on `engine`, and the player is a stub.
It adds the leftover sweep, so disposing the atelier needs no answer from the test.
