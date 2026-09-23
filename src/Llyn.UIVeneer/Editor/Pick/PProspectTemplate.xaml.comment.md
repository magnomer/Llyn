# PProspectTemplate.xaml.cs

## `public partial class PProspectTemplate : ResourceDictionary`

This dictionary presents prospects for selection while the editor owns the resulting card state.

## `private readonly PEditor _pProspectHost`

The editor determines which draft receives a prospect choice.

## `internal PProspectTemplate(PEditor host)`

The host connects shared prospect resources to their active editor context.

## `private void PProspectHandle(object sender, MouseButtonEventArgs e)`

Prospect clicks delegate selection so the editor can update the right draft.
