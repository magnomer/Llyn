# QDuplex.cs

## `internal sealed class QDuplex`

Drives the duplex panel: it builds its two wings and puts them to work.
Searching, picking, reading and restoring live in the wing, so this file only introduces both.
The panel itself is the veneer's `PDuplex` page, which the window places.

## `internal QDuplex(UserControl surface)`

Takes the page the window pulled under the contract ID `PDuplex`.
The page places the veneer's `PWing` twice, and each place gets its own wing driver.

## `internal void QDuplexIntroduce(PWindow host)`

Puts both wings to work on the window `host`, telling each whether it is the left place.
Each wing subscribes for itself, so the panel subscribes to nothing.
