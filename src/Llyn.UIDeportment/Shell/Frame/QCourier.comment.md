# QCourier.cs
Hash: `97be4a38a0ad0f71`

## `internal sealed class QCourier`

Drives the Joplin control at the left end of the status bar.
That is the connect button, the connected badge, the send button and the receipt line.
The courier owns the connect, the push and whether a token is stored.
This driver hears the two buttons and paints what the courier hands back.

## `internal QCourier(FrameworkElement surface)`

Takes the status bar `QEstablishment` drives, which builds this driver over it.
Wires both buttons once, as the window is built.

## `internal void QCourierIntroduce(QWindow host)`

Creates its own courier from the window's atelier through `CCourierCreate`, so both buttons share one busy mark.
It subscribes `CCourierChanged` through `QObserver`, so a state change repaints on the surface's thread.
That matters because a settings bulletin raises the state on the engine's thread.
It keeps the window's envoy, which both gates show a failure through.
It paints `CCourierRead` once through `QCourierRefine`, so the bar starts idle and unconnected.
`QEstablishmentAttach` introduces it before the atelier opens, so the first open repaints the connection.

## `private void QCourierRefine(CCourierState state)`

Paints one courier state, from the first read or from `CCourierChanged`.
The connect button and the badge swap on `CCourierStateAttached`, so exactly one of them shows.
Both buttons take `CCourierStateAllowed` as it stands, since the courier decides when they rest.
The slash shows only beside a line, so an idle bar ends at the send button.

## `private async void QCourierSendObserve(object sender, RoutedEventArgs e)`

The send button hands the push to `CCourierSend`, which reports its own failure.

## `private async void QCourierAttachObserve(object sender, RoutedEventArgs e)`

The connect button hands the request to `CCourierAttach`, which reports its own failure.
