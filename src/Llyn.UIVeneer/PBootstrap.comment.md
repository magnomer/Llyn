# PBootstrap.xaml
Hash: `4a398625bee8ddca`

The application the host creates and runs, with the resources every window shares.
The deportment reaches it through `Application.Current` and never names it.

## `x:Class="Llyn.UIVeneer.PBootstrap"`

The shell only calls `InitializeComponent()`, so the class holds no behaviour.
The file is not named `App.xaml`, so no generated entry point competes with the host's.

## `<ResourceDictionary.MergedDictionaries>`

Merges the base theme dictionary once for every window.

## `<sys:String x:Key="PIconRoot">pack://application:,,,/Llyn.UIVeneer;component/icons/</sys:String>`

The folder the icon assets live in, under its contract ID.
The deportment joins an icon name to it, so it never names the veneer itself.

## `<veneer:PWindow x:Key="PWindow" x:Shared="False" />`

The main window under its contract ID.
`x:Shared="False"` makes every pull build a fresh window, since a window cannot be shown twice.

## `<veneer:PSCoinage x:Key="PSCoinage" x:Shared="False" />`

The dialog that asks for a new tag or register name, fresh on every pull.

## `<veneer:PSLeave x:Key="PSLeave" x:Shared="False" />`

The dialog that asks what to do with unsaved work, fresh on every pull.

## `<veneer:PSCustoms x:Key="PSCustoms" x:Shared="False" />`

The dialog that declares imported entries and reports skipped lines, fresh on every pull.
