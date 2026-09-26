# PMarkerTemplate.cs

## `public class PMarkerTemplate : ResourceDictionary`

This dictionary draws speech markers as editor chips while speech data stays with the editor.

The host's fill subscribes each forwarder on the realized part, where an event attribute stood before.

## `private readonly PEditor _pSpeechHost`

The editor resolves marker activation against the speech attached to the active card.

## `internal PMarkerTemplate(PEditor host)`

The host keeps shared marker resources connected to their owning editor.
Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PMarkerChipHandle(object sender, RoutedEventArgs e)`

Marker activation delegates to the editor because it owns speech selection and state.
