# PMentionMenuTemplate.xaml.cs

## `public partial class PMentionMenuTemplate : ResourceDictionary`

This dictionary presents mention menu entries while the window owns navigation and mention state.

## `private readonly PWindow _pMentionHost`

The window resolves a shared menu click against its current navigation context.

## `internal PMentionMenuTemplate(PWindow host)`

The host connects mention menu resources to the window that owns them.

## `private void PMentionMenuHandle(object sender, MouseButtonEventArgs e)`

Menu activation delegates to the window so it can open the chosen mention target.
