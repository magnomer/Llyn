# PSentenceTemplate.cs

## `public class PSentenceTemplate : ResourceDictionary`

This dictionary provides sentence examples and editing commands while the editor owns sentence state.

The host's fill subscribes each forwarder on the realized part, where an event attribute stood before.

## `private readonly PEditor _pSentenceHost`

The host maps shared sentence controls to the active card and its draft.

## `internal PSentenceTemplate(PEditor host)`

The editor reference keeps sentence edits attached to the correct card.
Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PSentenceAddHandle(object sender, RoutedEventArgs e)`

Adding an example goes through the editor so the active card receives it.

## `internal void PSentenceRemoveHandle(object sender, RoutedEventArgs e)`

The editor removes an example from the card whose sentence control raised the event.

## `internal void PCitationKeyHandle(object sender, KeyEventArgs e)`

Citation keys use editor state to keep citation editing aligned with the current draft.

## `internal void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving a citation field lets the editor commit or reconcile its draft value.

## `internal void PGlossAddHandle(object sender, RoutedEventArgs e)`

The editor adds a gloss because it owns the sentence example being edited.

## `internal void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Gloss removal uses editor state to target the selected gloss in the active example.

## `internal void PSentenceLinkHandle(object sender, ExecutedRoutedEventArgs e)`

The editor applies linking to the sentence selection and its owning card.

## `internal void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)`

Sense assignment is routed through the editor to preserve card and selection context.

## `internal void PSentenceSilenceHandle(object sender, ExecutedRoutedEventArgs e)`

The editor handles silence because it owns the sentence and its playback state.

## `internal void PSentenceUnlinkHandle(object sender, ExecutedRoutedEventArgs e)`

The editor removes a link using the selection belonging to the active example.

## `internal void PSentenceLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

The editor enables linking only when the current sentence selection supports it.

## `internal void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)`

The editor checks sense availability against the active sentence selection.

## `internal void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

The editor checks whether the current selection contains a removable link.
