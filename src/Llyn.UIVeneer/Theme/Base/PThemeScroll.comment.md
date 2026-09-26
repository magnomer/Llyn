# PThemeScroll.xaml

## `<Style TargetType="ScrollBar">`

Every rail in the app is one design at one size, upright or sideways.
A page, a catalog, a popup and a text field all show the same thread.
The two control templates are registered by PIndicator.
WPF requires a PART_Track name.
That name must stay out of the audited XAML naming surface.
The sideways size and template are set by `PLook` in the deportment, not by a trigger.

## `<Style TargetType="ScrollViewer">`

The lane template is the default for every scroll viewer, not an opt-in.
A viewer the app never names, inside a dropdown or a text field, would otherwise keep the platform one.
That is how a rail nobody styled used to appear over the content at a size nobody chose.
PIndicator registers the template.

## `<Style x:Key="Theme.Scroll.Gutter" TargetType="ScrollViewer">`

The name a page uses to ask for the lane it already has.
It says in the markup what the default would give it anyway.

## `<Style x:Key="Theme.Scroll.Lane" TargetType="ScrollViewer">`

A band that scrolls sideways and never up.
It holds the same rail as everything else, turned on its side.
