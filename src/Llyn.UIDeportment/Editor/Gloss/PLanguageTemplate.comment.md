# PLanguageTemplate.cs

## `public class PLanguageTemplate : ResourceDictionary`

This dictionary draws a card language field while leaving language state with the editor.

The host's fill subscribes each forwarder on the realized part, where an event attribute stood before.

## `private readonly PEditor _pLanguageHost`

The editor owns speaker changes and resolves them against the card being edited.

## `internal PLanguageTemplate(PEditor host)`

The host lets a shared dictionary return language changes to the owning editor.
Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PSpeakerHandle(object sender, RoutedEventArgs e)`

Speaker selection delegates to the editor so it can update the active card's language.
