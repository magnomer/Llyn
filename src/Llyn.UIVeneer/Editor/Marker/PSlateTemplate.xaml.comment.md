# PSlateTemplate.xaml.cs

## `public partial class PSlateTemplate : ResourceDictionary`

This dictionary displays slate content while the editor decides how it affects the current card.

## `private readonly PEditor _pSlateHost`

The editor provides the active card context for slate interactions.

## `internal PSlateTemplate(PEditor host)`

The host connects shared slate presentation to the editor that owns its card.

## `private void PSlateHandle(object sender, MouseButtonEventArgs e)`

A slate click delegates to the editor so it can apply the action to the right card.
