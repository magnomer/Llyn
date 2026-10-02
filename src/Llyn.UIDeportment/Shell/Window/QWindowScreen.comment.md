# QWindowScreen.cs

## `internal sealed class QWindowScreen`

The one browser environment the window owns, handed to every Screen that plays a web address.
The runtime allows a single environment per process, so the task is made once and shared.
It is started with autoplay allowed, since the user has already asked by the time play is called.

## `internal Task<CoreWebView2Environment> QScreenSettingRead()`

The shared environment task, started on the first ask.

## `internal void QScreenSettingReset()`

Drops the task after a failed start, so a later attempt is a real attempt.
