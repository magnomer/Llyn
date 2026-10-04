# TNavigation.cs
Hash: `c6a168bfe7d17ce9`

## `public sealed class TNavigation`

Covers Conduct's navigation over a real posture on the fake rig, with no window.
The tabs are registered through the relay, each with a leave question that notes itself.
With nothing stored, startup paints no tab.
A stored tab opens again at startup with its editor restored, and one no longer offered falls back and hides.
A tab click asks only the open tab to be left, and a jump asks its target first.
Any refusal keeps the open tab, saves nothing and paints nothing.
A jump records the station it leaves, and the voyage steps land where they recorded.
A jump its target or the open tab declines records no station, so a later step back finds nothing.
A step whose landing tab declines stays put and keeps the trail for the next try.
Adding a station for the open tab records it and paints no tab.
A citing place opens its Example or its Entry.
The area's own open receives the landed record, and a rime cell reaches the yunjing area's attached open.
A series reaches the xiesheng area's attached open, and nothing opens before it is attached.

## `private static void TNavigationTabAdd(CNavigation navigation, string tab, List<string> asked, bool leave, long station, List<(string, long)> arrived)`

Registers `tab` with a leave question that notes the tab in `asked` and answers `leave`.
The panel stands on `station`, and its open notes each landed record in `arrived`.

## `private static List<CNavigationState> TNavigationStateAttach(CNavigation navigation)`

Collects every state the navigation raises from here on.

## `private static CUsage TNavigationUsageCreate(bool quoted)`

A citing place of Example 3 in Entry 9, quoting the Example when `quoted`.
