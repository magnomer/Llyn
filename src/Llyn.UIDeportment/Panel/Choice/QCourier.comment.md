# QCourier.cs
Hash: `b91ad6970486baa2`

## `internal sealed class QCourier`

The Joplin group on the settings panel's Web page.
That is the port field, the connect button, the send button and the receipt line.
The port field saves on Enter or when focus leaves, and reverts on Escape.
The ledger owns the stored port, and the courier owns the connect and the push.
This driver hears the controls and paints what those two hand back.

## `internal QCourier(FrameworkElement settings)`

Wires the field's keys, its focus loss and both buttons once, as the settings panel is built.
The field saves on Enter or when focus leaves, and reverts on Escape.
Escape is heard before Enter, as the workspace field wires them.

## `internal void QCourierIntroduce(CLedger ledger, CAtelier atelier, CEnvoy envoy)`

Puts the group to work on the ledger the panel hands over.
It creates its own courier from `atelier` through `CCourierCreate`, so both buttons share one busy mark.
It subscribes `CCourierChanged` through `QObserver`, so a state change repaints on the surface's thread.
It keeps the window's envoy, which both show a failure through.
It paints `CCourierRead` once through `QCourierRefine`, so the buttons start enabled and the line empty.

## `internal void QCourierRefine(CCourierState state)`

Paints one courier state, from the first read or from `CCourierChanged`.
Both buttons take `CCourierStateAllowed` as it stands, since the courier decides when they rest.

## `internal void QCourierOutpostRefine(string port)`

Writes the stored port into the field.
The panel calls it with the port in every ledger state, and both reverts call it too.

## `private async void QCourierSendObserve(object sender, RoutedEventArgs e)`

The send button hands the push to `CCourierSend`, which reports its own failure.

## `private async void QCourierAttachObserve(object sender, RoutedEventArgs e)`

The connect button hands the request to `CCourierAttach`, which reports its own failure.

## `private void QCourierOutpostObserve(object sender, KeyEventArgs e)`

Enter hands the raw typed port to `CLedgerOutpostSave`, which the engine judges.
The ledger raises again either way, so the field then shows the stored port.

## `private void QCourierEscapeRefine(object sender, KeyEventArgs e)`

Escape repaints the field with the port `CLedgerOutpostRead` answers.
It is wired before the Enter save, so Escape never reaches the save.

## `private void QCourierFocusObserve(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field hands the raw typed port to `CLedgerOutpostSave`.
A refusal is shown and the ledger repaints the stored port.
Connect or Send clicked right after typing therefore uses that port.
