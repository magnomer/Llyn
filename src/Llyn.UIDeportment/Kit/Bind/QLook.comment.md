# QLook.cs
Hash: `37588513698d8cdb`

## `internal static class QLook`

Maps a bare fact to the framework value a control property takes.
A veneer member may not branch, so the one `?:` each mapping needs lives here, once.
It also switches the visual states the veneer's styles no longer carry as triggers.
The item template fills live in [QLookItem](QLookItem.comment.md).

## `internal sealed record QLookSetter(string QLookSetterStyle, QLookCue QLookSetterCue, string? QLookSetterPart, DependencyProperty QLookSetterProperty, QLookValue QLookSetterValue)`

One dismantled trigger setter, holding its style, cues, template part, property and value.
A null part means the control itself.
A row holds only while every cue it names holds, as a multi-trigger did.
The value is a typed `QLookValue`, which sets itself and steps back itself.

## `internal static readonly DependencyProperty QLookCueProperty`

The cues a view gives an element, such as chosen, pending or playing, owned by the look.
It replaces the string a `Tag` carried, so the state is typed and no other meaning shares the slot.
A change runs the state handler at once.

## `internal static readonly DependencyProperty QLookIconProperty`

The icon a command, helper or navigation tab button shows in its icon part.
A button without one reads the `Bare` cue, so its icon part folds away.
A change runs the state handler at once.

## `private static readonly DependencyProperty QLookReachProperty`

Set on each attached view and inherited by every element inside it, including later rows and popups.
WPF raises load and unload only on elements that carry a handler of their own.
Class handlers do not count, so the state handler alone never reaches a bare element.
An element missed there keeps its unstyled look until the pointer first passes over it.
The inheritance reaches each element as it joins the view, before its load is raised.

## `private static readonly RoutedEventHandler QLookBroadcast = (_, _) => { };`

The empty load and unload handler a reached element carries.
It does no work itself.
It only makes WPF raise both events on the element, so the class handlers run.

## `private static readonly Dictionary<Style, string> QLookStyle = [];`

Every style object behind each row's key, found once when the handlers attach.
A dictionary merged twice yields two style objects under one key, and both map to it.

## `private static readonly ConditionalWeakTable<`

Per target, the local value each held property had before any row set it.
The value is saved once, when a row first takes the property, and never overwritten.
When no row holds the property any more, the saved value is put back and dropped.
So a view's own value survives a hover.
An element listed here already carries its enable watcher.

## `internal static void QLookStateAttach()`

Registers class handlers for load, unload, hover, press, focus and check once, for the whole program.
Press and focus are read after the input settles, since the control updates its flags in its own handlers.

## `private static readonly ConditionalWeakTable<FrameworkElement, List<DependencyPropertyDescriptor>> QLookTrack = [];`

The value-change descriptors hooked on each loaded element, so its unload can remove them.
A descriptor's watch holds the element strongly, so an unremoved watch would keep it forever.

## `internal static void QLookStyleAttach(FrameworkElement surface)`

Registers the look sheet's styles found in one view's own dictionary.
The attach at startup reads only the application resources, so a view merging local styles hands them in here.
It also marks the view for the reach, so the state handler sees every element of it load.

## `internal static QLookPart? QLookPartFind<QLookPart>(FrameworkElement container, string name)`

Finds a named part in a control's template, else in the item template of the first presenter below it.
The template is applied first, so a fill reaches its parts before the first layout.

## `private static void QLookStateRefine(object? sender, EventArgs e)`

The one handler every state change reaches.
It gathers the rows of every style in the control's chain, base first, else of the control's type name.
So a derived style's rows win over its base's rows, as its setters did.
It reads the active cues and sets the last active row on each part and property.
Every event sets the winning row again, which is harmless, so no row identity is kept per target.
A part missing from the template is looked up among the logical children, such as a menu's items.
Failing that, it is looked up in the item template of the first presenter, such as a button's icon.
A part and property with no active row is restored by that slot's value kind.
Enable, text, content, highlight, drag, items, selection, source and dropdown changes are watched per element.
A cue or icon change reaches the handler through its own property's change callback.
No routed event reports them.
The property watches are added only while the element is loaded, so its unload can take them off.
A combo box with no text is `Empty`, like a text block and a list with no items.
An image with no source and a control with empty text content are `Empty` too.
An open combo box is `Opened`, and a checked toggle is `Checked`.
A selected list item is `Selected`.
A highlighted combo item is `Highlight`, and a dragged thumb is `Drag`.

## `private static void QLookStateTeardown(object sender, RoutedEventArgs e)`

Removes an unloaded element's value-change watches, so the descriptors stop holding it.
A later load adds them again.

## `private static void QLookReachRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Gives an element joining a reached view the empty load and unload handler, once.
An element already loaded is refined at once, since its load has passed.
An element leaving the view keeps the handler, which costs nothing.

## `private static void QLookStyleScan(ResourceDictionary dictionary, HashSet<string> keys)`

Walks a dictionary and every dictionary it merges, mapping each style found under a row key.
A theme merged inside another theme holds its own copy of each style, so one lookup would miss it.

## `internal static Visibility QLookVisibleRead(bool shown)`

Visible when shown, else collapsed so the control takes no room.

## `internal static QLookChoice QLookFirstRead<QLookChoice>(bool first, QLookChoice chosen, QLookChoice other)`

One of two values by a verdict, so a tab with two edit areas picks the active one without branching.

## `internal static string QLookEpithetRead(string epithet)`

The text of a row's epithet run, led by an en space that parts it from the headword.
Every entry row reads its epithet here, so all lists keep one gap.
A plain space looks narrower in source and once drifted between lists unseen.

## `internal static void QLookPromptApply(TextBlock block, string text, string hint, string? ink)`

Shows a row's text, or the placeholder under `hint` in the muted colour while the text is blank.
A given `ink` colours the text, so a lead reading shows in the accent only while it holds text.

## `internal static bool QLookCheckedRead(bool? shown)`

A toggle's three-state check read as a plain yes or no, so a handler passes it down without comparing.

## `private static QLookCue QLookCueRead(bool shown, QLookCue cue)`

The cue when a control flag holds, else no cue.
So the gathering of cues reads as one line per flag.
