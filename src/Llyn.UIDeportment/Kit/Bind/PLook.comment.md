# PLook.cs

## `internal static class PLook`

Maps a bare fact to the framework value a control property takes.
A veneer member may not branch, so the one `?:` each mapping needs lives here, once.
It also switches the visual states the veneer's styles no longer carry as triggers.
The item template fills live in [PLookItem](PLookItem.comment.md).

## `internal sealed record PLookSetter(`

One dismantled trigger setter: the style it came from, its state, its template part, property and value.
A null part means the control itself.
A state joined by `+` holds only while every named state holds, as a multi-trigger did.
A value that is a property is copied from the control, as a template binding did.
A string value is a resource key, set as a dynamic reference.
An animation value is started on a fresh copy of the target's render transform.
The animation is stopped when its state ends.

## `private static readonly Dictionary<Style, string> PLookStyle = [];`

Every style object behind each row's key, found once when the handlers attach.
A dictionary merged twice yields two style objects under one key, and both map to it.

## `private static readonly ConditionalWeakTable<`

Per target, each property a row holds, with the local value it replaced and the row that holds it.
A left row restores the replaced local value, so a view's own value survives a hover.
An element listed here already carries its enable watcher.

## `internal static void PLookStateAttach()`

Registers class handlers for load, unload, hover, press, focus and check once, for the whole program.
Press and focus are read after the input settles, since the control updates its flags in its own handlers.

## `private static readonly ConditionalWeakTable<FrameworkElement, List<DependencyPropertyDescriptor>> PLookTrack = [];`

The value-change descriptors hooked on each loaded element, so its unload can remove them.
A descriptor's watch holds the element strongly, so an unremoved watch would keep it forever.

## `internal static void PLookStyleAttach(ResourceDictionary dictionary)`

Registers the look sheet's styles found in one view's own dictionary.
The attach at startup reads only the application resources, so a view merging local styles hands them in here.

## `internal static PLookPart? PLookPartFind<PLookPart>(FrameworkElement container, string name)`

Finds a named part in a control's template, else in the item template of the first presenter below it.
The template is applied first, so a fill reaches its parts before the first layout.

## `private static void PLookStateHandle(object? sender, EventArgs e)`

The one handler every state change reaches.
It gathers the rows of every style in the control's chain, base first, else of the control's type name.
So a derived style's rows win over its base's rows, as its setters did.
It reads the active states and sets the last active row on each part and property.
A part missing from the template is looked up among the logical children, such as a menu's items.
Failing that, it is looked up in the item template of the first presenter, such as a button's icon.
A part and property with no active row is restored.
Enable, tag, text, content, chosen, highlight, drag, source and dropdown changes are watched per element.
No routed event reports them.
The property watches are added only while the element is loaded, so its unload can take them off.
A combo box with no text is `Empty`, like a text block and a list with no items.
An image with no source and a control with empty text content are `Empty` too.
An open combo box is `Open`.
A selected list item is `Selected`.
A highlighted combo item is `Highlight`, and a dragged thumb is `Drag`.

## `private static void PLookStateDetach(object sender, RoutedEventArgs e)`

Removes an unloaded element's value-change watches, so the descriptors stop holding it.
A later load adds them again.

## `private static void PLookStyleScan(ResourceDictionary dictionary, HashSet<string> keys)`

Walks a dictionary and every dictionary it merges, mapping each style found under a row key.
A theme merged inside another theme holds its own copy of each style, so one lookup would miss it.

## `private static void PLookPartCopy(`

Binds a template part's property to the control's, the takeover of one template binding.
The binding takes the property's default mode, so a dropdown's toggle writes back as before.

## `internal static Visibility PLookVisibleRead(bool shown)`

Visible when shown, else collapsed so the control takes no room.

## `internal static bool? PLookCheckedRead(bool chosen)`

The three-state value a toggle's `IsChecked` takes, never indeterminate.

## `internal static PLookChoice PLookFirstRead<PLookChoice>(bool first, PLookChoice chosen, PLookChoice other)`

One of two values by a verdict, so a tab with two edit areas picks the active one without branching.

## `internal static void PLookPromptApply(TextBlock block, string text, string hint, string? ink)`

Shows a row's text, or the placeholder under `hint` in the muted colour while the text is blank.
A given `ink` colours the text, so a lead reading shows in the accent only while it holds text.

## `internal static bool PLookCheckedRead(bool? shown)`

A toggle's three-state check read as a plain yes or no, so a handler passes it down without comparing.
