# PLayoutChoice.cs

## `public partial class PSettings`

The linked-panels switch the user ticks in the settings panel.
It is kept in the posture so the next run opens with it.
No bulletin carries the switch, so the handler rewrites the summary row and syncs the tabs itself.
Ticking it sets every tab to the most recently dragged tab's widths at once, so the link is visible immediately.
Unticking it moves nothing, because each tab already holds its own widths and simply stops following.
The sync runs either way, since an unlinked sync only stores the dragged tab's widths, which are already stored.
