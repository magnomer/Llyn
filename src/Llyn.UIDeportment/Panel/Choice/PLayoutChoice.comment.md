# PLayoutChoice.cs

## `public partial class PSettings`

The linked-panels switch the user ticks in the settings panel.
It is kept in the posture so the next run opens with it.
The observer only hands the raw switch to the posture's save.
The posture raises `QPostureLinkedChanged`, and the summary row and the tabs answer it.
Ticking it sets every tab to the most recently dragged tab's widths at once, so the link is visible immediately.
Unticking it moves nothing, because each tab already holds its own widths and simply stops following.
